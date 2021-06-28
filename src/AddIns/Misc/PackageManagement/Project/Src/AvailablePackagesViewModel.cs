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
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class AvailablePackagesViewModel : PackagesViewModel
	{
		IPackageRepository repository;
		
		public AvailablePackagesViewModel(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents,
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageViewModelFactory packageViewModelFactory,
			ITaskFactory taskFactory)
			: base(
				solution,
				packageManagementEvents,
				registeredPackageRepositories, 
				packageViewModelFactory, 
				taskFactory)
		{
			IsSearchable = true;
			ShowPackageSources = true;
			ShowPrerelease = true;

			this.packageManagementEvents = packageManagementEvents;
			RegisterEvents();
		}
		
		void RegisterEvents()
		{
			packageManagementEvents.ParentPackageInstalled += OnPackageChanged;
			packageManagementEvents.ParentPackageUninstalled += OnPackageChanged;
			packageManagementEvents.ParentPackagesUpdated += OnPackageChanged;
		}
		
		protected override void OnDispose()
		{
			packageManagementEvents.ParentPackageInstalled -= OnPackageChanged;
			packageManagementEvents.ParentPackageUninstalled -= OnPackageChanged;
			packageManagementEvents.ParentPackagesUpdated -= OnPackageChanged;
		}
		
		protected override void UpdateRepositoryBeforeReadPackagesTaskStarts()
		{
			try {
				repository = RegisteredPackageRepositories.ActiveRepository;
			} catch (Exception ex) {
				repository = null;
				errorMessage = ex.Message;
			}
		}
		
		protected override IQueryable<IPackage> GetAllPackages(string searchCriteria)
		{
			if (repository == null) {
				throw new ApplicationException(errorMessage);
			}
			
			if (IncludePrerelease) {
				return repository
					.Search(searchCriteria, IncludePrerelease)
					.Where(package => package.IsAbsoluteLatestVersion);
			}
			return repository
				.Search(searchCriteria, IncludePrerelease)
				.Where(package => package.IsLatestVersion);
		}
		
		/// <summary>
		/// Order packages by most downloaded first.
		/// </summary>
		protected override IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages.OrderByDescending(package => package.DownloadCount);
		}
		
		protected override IEnumerable<IPackage> GetFilteredPackagesBeforePagingResults(IQueryable<IPackage> allPackages)
		{
			if (IncludePrerelease) {
				return base.GetFilteredPackagesBeforePagingResults(allPackages)
					.DistinctLast<IPackage>(PackageEqualityComparer.Id);
			}
			return base.GetFilteredPackagesBeforePagingResults(allPackages)
				.Where(package => package.IsReleaseVersion())
				.DistinctLast<IPackage>(PackageEqualityComparer.Id);
		}

	}
}
