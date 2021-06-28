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
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementSolution : IPackageManagementSolution
	{
		IRegisteredPackageRepositories registeredPackageRepositories;
		IPackageManagementProjectService projectService;
		IPackageManagementProjectFactory projectFactory;
		ISolutionPackageRepositoryFactory solutionPackageRepositoryFactory;
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents)
			: this(
				registeredPackageRepositories,
				new PackageManagementProjectService(),
				new PackageManagementProjectFactory(packageManagementEvents),
				new SolutionPackageRepositoryFactory())
		{
		}
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementProjectService projectService,
			IPackageManagementProjectFactory projectFactory,
			ISolutionPackageRepositoryFactory solutionPackageRepositoryFactory)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.projectFactory = projectFactory;
			this.projectService = projectService;
			this.solutionPackageRepositoryFactory = solutionPackageRepositoryFactory;
		}
		
		public string FileName {
			get { return OpenSolution.FileName; }
		}
		
		ISolution OpenSolution {
			get { return projectService.OpenSolution; }
		}
		
		public IPackageManagementProject GetActiveProject()
		{
			if (HasActiveProject()) {
				return GetActiveProject(ActivePackageRepository);
			}
			return null;
		}
		
		bool HasActiveProject()
		{
			return GetActiveMSBuildBasedProject() != null;
		}
		
		public IProject GetActiveMSBuildProject()
		{
			return projectService.CurrentProject;
		}
		
		IPackageRepository ActivePackageRepository {
			get { return registeredPackageRepositories.ActiveRepository; }
		}
		
		public IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			MSBuildBasedProject activeProject = GetActiveMSBuildBasedProject();
			if (activeProject != null) {
				return CreateProject(sourceRepository, activeProject);
			}
			return null;
		}
		
		MSBuildBasedProject GetActiveMSBuildBasedProject()
		{
			return GetActiveMSBuildProject() as MSBuildBasedProject;
		}
		
		IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			return projectFactory.CreateProject(sourceRepository, project);
		}
		
		IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return registeredPackageRepositories.CreateRepository(source);
		}
		
		public IPackageManagementProject GetProject(PackageSource source, string projectName)
		{
			MSBuildBasedProject msbuildProject = GetMSBuildProject(projectName);
			return CreateProject(source, msbuildProject);
		}
		
		MSBuildBasedProject GetMSBuildProject(string name)
		{
			var openProjects = new OpenMSBuildProjects(projectService);
			return openProjects.FindProject(name);
		}
		
		IPackageManagementProject CreateProject(PackageSource source, MSBuildBasedProject project)
		{
			IPackageRepository sourceRepository = CreatePackageRepository(source);
			return CreateProject(sourceRepository, project);
		}
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			MSBuildBasedProject msbuildProject = GetMSBuildProject(projectName);
			return CreateProject(sourceRepository, msbuildProject);
		}
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, IProject project)
		{
			var msbuildProject = project as MSBuildBasedProject;
			return CreateProject(sourceRepository, msbuildProject);
		}
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return projectService.AllProjects.OfType<MSBuildBasedProject>();;
		}
		
		public bool IsOpen {
			get { return OpenSolution != null; }
		}
		
		public bool HasMultipleProjects()
		{
			return projectService.AllProjects.Count > 1;
		}
		
		public ISolutionPackageRepository CreateSolutionPackageRepository()
		{
			return solutionPackageRepositoryFactory.CreateSolutionPackageRepository(OpenSolution);
		}
		
		public bool IsPackageInstalled(IPackage package)
		{
			return CreateSolutionPackageRepository().IsInstalled(package);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			return repository.GetPackages();
		}
		
		public IQueryable<IPackage> GetSolutionPackages()
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			List<IPackageManagementProject> projects = GetProjects(ActivePackageRepository).ToList();
			return repository
				.GetPackages()
				.Where(package => !IsPackageInstalledInAnyProject(projects, package));
		}
		
		public IQueryable<IPackage> GetProjectPackages()
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			List<IPackageManagementProject> projects = GetProjects(ActivePackageRepository).ToList();
			return repository
				.GetPackages()
				.Where(package => IsPackageInstalledInAnyProject(projects, package));
		}
		
		bool IsPackageInstalledInAnyProject(IList<IPackageManagementProject> projects, IPackage package)
		{
			if (projects.Any(project => project.IsPackageInstalled(package))) {
				return true;
			}
			return false;
		}
		
		public string GetInstallPath(IPackage package)
		{
			return CreateSolutionPackageRepository().GetInstallPath(package);
		}
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			return CreateSolutionPackageRepository().GetPackagesByReverseDependencyOrder();
		}
		
		public IEnumerable<IPackageManagementProject> GetProjects(IPackageRepository sourceRepository)
		{
			foreach (MSBuildBasedProject msbuildProject in GetMSBuildProjects()) {
				yield return projectFactory.CreateProject(sourceRepository, msbuildProject);
			}
		}
	}
}
