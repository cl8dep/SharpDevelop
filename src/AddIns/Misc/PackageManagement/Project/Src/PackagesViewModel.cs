﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class PackagesViewModel : ViewModelBase<PackagesViewModel>, IDisposable, IPackageViewModelParent
	{
		Pages pages = new Pages();
		
		protected IPackageManagementEvents packageManagementEvents;
		protected IPackageManagementSolution solution;
		protected IPackageManagementProject project;
		protected string errorMessage = string.Empty;

		IRegisteredPackageRepositories registeredPackageRepositories;
		IPackageViewModelFactory packageViewModelFactory;
		ITaskFactory taskFactory;
		IEnumerable<IPackage> allPackages;
		ITask<PackagesForSelectedPageResult> task;
		bool includePrerelease;
		PackagesForSelectedPageQuery packagesForSelectedPageQuery;
		
		public PackagesViewModel(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
		{
			this.solution = solution;
			this.packageManagementEvents = packageManagementEvents;
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.packageViewModelFactory = packageViewModelFactory;
			this.taskFactory = taskFactory;
			this.solution = solution;
			
			PackageViewModels = new ObservableCollection<PackageViewModel>();

			CreateCommands();
			TryGetActiveProject();
		}
		
		void CreateCommands()
		{
			ShowNextPageCommand = new DelegateCommand(param => ShowNextPage());
			ShowPreviousPageCommand = new DelegateCommand(param => ShowPreviousPage());
			ShowPageCommand = new DelegateCommand(param => ExecuteShowPageCommand(param));
			SearchCommand = new DelegateCommand(param => Search());
			UpdateAllPackagesCommand = new DelegateCommand(param => UpdateAllPackages());
		}
		
		void TryGetActiveProject()
		{
			try {
				project = solution.GetActiveProject();
			} catch (Exception ex) {
				errorMessage = ex.Message;
			}
		}

		public ICommand ShowNextPageCommand { get; private set; }
		public ICommand ShowPreviousPageCommand { get; private set; }
		public ICommand ShowPageCommand { get; private set; }
		public ICommand SearchCommand { get; private set; }
		public ICommand UpdateAllPackagesCommand { get; private set; }
		
		public void Dispose()
		{
			OnDispose();
			IsDisposed = true;
		}
		
		protected virtual void OnDispose()
		{
		}
		
		public bool IsDisposed { get; private set; }
		
		public bool HasError { get; private set; }
		public string ErrorMessage { get; private set; }
		
		public ObservableCollection<PackageViewModel> PackageViewModels { get; set; }
		
		public IRegisteredPackageRepositories RegisteredPackageRepositories {
			get { return registeredPackageRepositories; }
		}
		public bool IsReadingPackages { get; private set; }
		
		protected void OnPackageChanged(object sender, EventArgs e)
		{
			if (IsReadingPackages || (PackageViewModels == null)) {
				return;
			}
			
			// refresh all because we don't know if any dependent package is (un)installed
			foreach (PackageViewModel packageViewModel in PackageViewModels) {
				if (!IsReadingPackages) {
					packageViewModel.PackageChanged();
				}
			}
		}
		
		public void ReadPackages()
		{
			allPackages = null;
			pages.SelectedPageNumber = 1;
			UpdateRepositoryBeforeReadPackagesTaskStarts();
			StartReadPackagesTask();
		}
		
		void StartReadPackagesTask()
		{
			IsReadingPackages = true;
			HasError = false;
			ClearPackages();
			CancelReadPackagesTask();
			CreateReadPackagesTask();
			task.Start();
		}
		
		protected virtual void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
		}
		
		void CancelReadPackagesTask()
		{
			if (task != null) {
				task.Cancel();
			}
		}
		
		void CreateReadPackagesTask()
		{
			var query = new PackagesForSelectedPageQuery(this, allPackages, GetSearchCriteria());
			packagesForSelectedPageQuery = query;
			
			task = taskFactory.CreateTask(
				() => GetPackagesForSelectedPageResult(query),
				OnPackagesReadForSelectedPage);
		}

		PackagesForSelectedPageResult GetPackagesForSelectedPageResult(PackagesForSelectedPageQuery query)
		{
			IEnumerable<IPackage> packages = GetPackagesForSelectedPage(query);
			return new PackagesForSelectedPageResult(packages, query);
		}
		
		void OnPackagesReadForSelectedPage(ITask<PackagesForSelectedPageResult> task)
		{
			IsReadingPackages = false;
			if (task.IsFaulted) {
				SaveError(task.Exception);
			} else if (task.IsCancelled) {
				// Ignore
			} else if (!IsCurrentQuery(task.Result)) {
				// Ignore.
			} else {
				UpdatePackagesForSelectedPage(task.Result);
			}
			base.OnPropertyChanged(null);
		}
		
		bool IsCurrentQuery(PackagesForSelectedPageResult result)
		{
			return packagesForSelectedPageQuery == result.Query;
		}
		
		void SaveError(AggregateException ex)
		{
			HasError = true;
			ErrorMessage = GetErrorMessage(ex);
			ICSharpCode.Core.LoggingService.Debug(ex);
		}
		
		string GetErrorMessage(AggregateException ex)
		{
			var errorMessage = new AggregateExceptionErrorMessage(ex);
			return errorMessage.ToString();
		}

		void UpdatePackagesForSelectedPage(PackagesForSelectedPageResult result)
		{
			pages.TotalItems = result.TotalPackages;
			pages.TotalItemsOnSelectedPage = result.TotalPackagesOnPage;
			TotalItems = result.TotalPackages;
			allPackages = result.AllPackages;
			UpdatePackageViewModels(result.Packages);
		}
		
		void PagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			StartReadPackagesTask();
			base.OnPropertyChanged(null);
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage(PackagesForSelectedPageQuery query)
		{
			IEnumerable<IPackage> filteredPackages = GetFilteredPackagesBeforePagingResults(query);
			return GetPackagesForSelectedPage(filteredPackages, query);
		}
		
		IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(PackagesForSelectedPageQuery query)
		{
			if (query.AllPackages == null) {
				IQueryable<IPackage> packages = GetPackagesFromPackageSource(query.SearchCriteria);
				query.TotalPackages = packages.Count();
				query.AllPackages = GetFilteredPackagesBeforePagingResults(packages);
			}
			return query.AllPackages;
		}
		
		/// <summary>
		/// Returns the queryable object that will be used to query the NuGet online feed.
		/// </summary>
		public IQueryable<IPackage> GetPackagesFromPackageSource()
		{
			return GetPackagesFromPackageSource(GetSearchCriteria());
		}
		
		IQueryable<IPackage> GetPackagesFromPackageSource(string searchCriteria)
		{
			IQueryable<IPackage> packages = GetAllPackages(searchCriteria);
			return OrderPackages(packages);
		}
		
		protected virtual IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages
				.OrderBy(package => package.Id);
		}
		
		string GetSearchCriteria()
		{
			if (String.IsNullOrWhiteSpace(SearchTerms)) {
				return null;
			}
			return SearchTerms;
		}
		
		IEnumerable<IPackage> GetPackagesForSelectedPage(IEnumerable<IPackage> allPackages, PackagesForSelectedPageQuery query)
		{
			return allPackages
				.Skip(query.Skip)
				.Take(query.Take);
		}
		
		/// <summary>
		/// Returns all the packages.
		/// </summary>
		protected virtual IQueryable<IPackage> GetAllPackages(string searchCriteria)
		{
			return null;
		}
		
		/// <summary>
		/// Allows filtering of the packages before paging the results. Call base class method
		/// to run default filtering.
		/// </summary>
		protected virtual IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			IEnumerable<IPackage> bufferedPackages = GetBufferedPackages(allPackages);
			return bufferedPackages;
		}
		
		IEnumerable<IPackage> GetBufferedPackages(IQueryable<IPackage> allPackages)
		{
			return allPackages.AsBufferedEnumerable(30);
		}
		
		void UpdatePackageViewModels(IEnumerable<IPackage> packages)
		{
			IEnumerable<PackageViewModel> currentViewModels = ConvertToPackageViewModels(packages);
			UpdatePackageViewModels(currentViewModels);
		}
		
		void UpdatePackageViewModels(IEnumerable<PackageViewModel> newPackageViewModels)
		{
			ClearPackages();
			PackageViewModels.AddRange(newPackageViewModels);
		}
		
		void ClearPackages()
		{
			PackageViewModels.Clear();
		}
		
		public IEnumerable<PackageViewModel> ConvertToPackageViewModels(IEnumerable<IPackage> packages)
		{
			foreach (IPackage package in packages) {
				yield return CreatePackageViewModel(package);
			}
		}
		
		PackageViewModel CreatePackageViewModel(IPackage package)
		{
			var repository = registeredPackageRepositories.ActiveRepository;
			var packageFromRepository = new PackageFromRepository(package, repository);
			return packageViewModelFactory.CreatePackageViewModel(this, packageFromRepository);
		}
		
		public int SelectedPageNumber {
			get { return pages.SelectedPageNumber; }
			set {
				if (pages.SelectedPageNumber != value) {
					pages.SelectedPageNumber = value;
					StartReadPackagesTask();
					base.OnPropertyChanged(null);
				}
			}
		}
		
		public int PageSize {
			get { return pages.PageSize; }
			set { pages.PageSize = value; }
		}
		
		public int ItemsBeforeFirstPage {
			get { return pages.ItemsBeforeFirstPage; }
		}
		
		public bool IsPaged {
			get { return pages.IsPaged; }
		}
		
		public ObservableCollection<Page> Pages {
			get { return pages; }
		}
		
		public bool HasPreviousPage {
			get { return pages.HasPreviousPage; }
		}
		
		public bool HasNextPage {
			get { return pages.HasNextPage; }
		}
		
		public int MaximumSelectablePages {
			get { return pages.MaximumSelectablePages; }
			set { pages.MaximumSelectablePages = value; }
		}
		
		public int TotalItems { get; private set; }
		
		public void ShowNextPage()
		{
			SelectedPageNumber += 1;
		}
		
		public void ShowPreviousPage()
		{
			SelectedPageNumber -= 1;
		}
		
		void ExecuteShowPageCommand(object param)
		{
			int pageNumber = (int)param;
			ShowPage(pageNumber);
		}
		
		public void ShowPage(int pageNumber)
		{
			SelectedPageNumber = pageNumber;
		}
		
		public bool IsSearchable { get; set; }
		
		public string SearchTerms { get; set; }
		
		public void Search()
		{
			ReadPackages();
			OnPropertyChanged(null);
		}
		
		public bool ShowPackageSources { get; set; }
		
		public IEnumerable<PackageSource> PackageSources {
			get {
				foreach (PackageSource packageSource in registeredPackageRepositories.PackageSources.GetEnabledPackageSources()) {
					yield return packageSource;
				}
				if (registeredPackageRepositories.PackageSources.HasMultipleEnabledPackageSources) {
					yield return RegisteredPackageSourceSettings.AggregatePackageSource;
				}
			}
		}
		
		public PackageSource SelectedPackageSource {
			get { return registeredPackageRepositories.ActivePackageSource; }
			set {
				if (registeredPackageRepositories.ActivePackageSource != value) {
					registeredPackageRepositories.ActivePackageSource = value;
					ReadPackages();
					OnPropertyChanged(null);
				}
			}
		}
		
		public bool ShowUpdateAllPackages { get; set; }
		
		public bool IsUpdateAllPackagesEnabled {
			get {
				return ShowUpdateAllPackages && (TotalItems > 1);
			}
		}
		
		void UpdateAllPackages()
		{
			try {
				packageViewModelFactory.PackageManagementEvents.OnPackageOperationsStarting();
				TryUpdatingAllPackages();
			} catch (Exception ex) {
				ReportError(ex);
				LogError(ex);
			}
		}
		
		void LogError(Exception ex)
		{
			packageViewModelFactory
				.Logger
				.Log(MessageLevel.Error, ex.ToString());
		}
		
		void ReportError(Exception ex)
		{
			packageViewModelFactory
				.PackageManagementEvents
				.OnPackageOperationError(ex);
		}
		
		protected virtual void TryUpdatingAllPackages()
		{
		}
		
		protected IPackageActionRunner ActionRunner {
			get { return packageViewModelFactory.PackageActionRunner; }
		}
		
		public bool IncludePrerelease {
			get { return includePrerelease; }
			set {
				if (includePrerelease != value) {
					includePrerelease = value;
					ReadPackages();
					OnPropertyChanged(null);
				}
			}
		}
		
		public bool ShowPrerelease { get; set; }
	}
}
