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
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementSolution : IPackageManagementSolution
	{
		public void AddPackageToActiveProjectLocalRepository(FakePackage package)
		{
			FakeActiveProject.FakePackages.Add(package);
		}
		
		public FakePackage AddPackageToActiveProjectLocalRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			AddPackageToActiveProjectLocalRepository(package);
			return package;
		}
		
		public int GetActiveProjectCallCount;
		public FakePackageManagementProject FakeActiveProject = new FakePackageManagementProject();
		public bool IsGetActiveProjectWithNoParametersCalled;
		
		public virtual IPackageManagementProject GetActiveProject()
		{
			GetActiveProjectCallCount++;
			IsGetActiveProjectWithNoParametersCalled = true;
			
			return FakeActiveProject;
		}
		
		public IPackageRepository RepositoryPassedToGetActiveProject;
		
		public virtual IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			RepositoryPassedToGetActiveProject = sourceRepository;
			return FakeActiveProject;
		}
		
		public FakePackageManagementProject FakeProjectToReturnFromGetProject =
			new FakePackageManagementProject();
		
		public PackageSource PackageSourcePassedToGetProject;
		public string ProjectNamePassedToGetProject;
		
		public IPackageManagementProject GetProject(PackageSource source, string projectName)
		{
			PackageSourcePassedToGetProject = source;
			ProjectNamePassedToGetProject = projectName;
			return FakeProjectToReturnFromGetProject;
		}
		
		public IPackageRepository RepositoryPassedToGetProject;
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			RepositoryPassedToGetProject = sourceRepository;
			ProjectNamePassedToGetProject = projectName;
			return FakeProjectToReturnFromGetProject;
		}
		
		public IProject ProjectPassedToGetProject;
		public List<IProject> ProjectsPassedToGetProject = new List<IProject>();
		public Dictionary<string, FakePackageManagementProject> FakeProjectsToReturnFromGetProject
			= new Dictionary<string, FakePackageManagementProject>();
		
		public virtual IPackageManagementProject GetProject(IPackageRepository sourceRepository, IProject project)
		{
			RepositoryPassedToGetProject = sourceRepository;
			ProjectPassedToGetProject = project;
			ProjectsPassedToGetProject.Add(project);
			FakePackageManagementProject fakeProject = null;
			if (FakeProjectsToReturnFromGetProject.TryGetValue(project.Name, out fakeProject)) {
				return fakeProject;
			}
			return FakeProjectToReturnFromGetProject;
		}
		
		public IProject FakeActiveMSBuildProject;
		
		public IProject GetActiveMSBuildProject()
		{
			return FakeActiveMSBuildProject;
		}
		
		public List<IProject> FakeMSBuildProjects = new List<IProject>();
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return FakeMSBuildProjects;
		}
		
		public bool IsOpen { get; set; }
		
		public bool HasMultipleProjects()
		{
			return FakeMSBuildProjects.Count > 1;
		}
		
		public string FileName { get; set; }
		
		public List<FakePackage> FakeInstalledPackages = new List<FakePackage>();
		
		public bool IsPackageInstalled(IPackage package)
		{
			return FakeInstalledPackages.Contains(package);
		}
		
		public ISolutionPackageRepository CreateSolutionPackageRepository()
		{
			return null;
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			var allPackages = new List<FakePackage>();
			allPackages.AddRange(FakeInstalledPackages);
			allPackages.AddRange(PackagesOnlyInPackagesFolder);
			return allPackages.AsQueryable();
		}
		
		public void NoProjectsSelected()
		{
			FakeActiveProject = null;
			FakeActiveMSBuildProject = null;
		}
		
		public FakePackageManagementProject AddFakeProjectToReturnFromGetProject(string name)
		{
			var project = new FakePackageManagementProject(name);
			FakeProjectsToReturnFromGetProject.Add(name, project);
			return project;
		}
		
		public List<FakePackage> FakePackagesInReverseDependencyOrder = 
			new List<FakePackage>();
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			return FakePackagesInReverseDependencyOrder;
		}
		
		public List<FakePackageManagementProject> FakeProjects =
			new List<FakePackageManagementProject>();
		
		public IPackageRepository SourceRepositoryPassedToGetProjects;
		
		public IEnumerable<IPackageManagementProject> GetProjects(IPackageRepository sourceRepository)
		{
			SourceRepositoryPassedToGetProjects = sourceRepository;
			return FakeProjects;
		}
		
		public FakePackageManagementProject AddFakeProject(string projectName)
		{
			var project = new FakePackageManagementProject(projectName);
			FakeProjects.Add(project);
			return project;
		}
		
		public FakePackage AddPackageToSharedLocalRepository(string packageId, string version)
		{
			var package = new FakePackage(packageId, version);
			FakeInstalledPackages.Add(package);
			return package;
		}
		
		public List<FakePackage> PackagesOnlyInPackagesFolder = new List<FakePackage>();
		
		public FakePackage AddPackageToSharedLocalRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			FakeInstalledPackages.Add(package);
			return package;
		}
		
		public string GetInstallPath(IPackage package)
		{
			throw new NotImplementedException();
		}
		
		public IQueryable<IPackage> GetSolutionPackages()
		{
			throw new NotImplementedException();
		}

		public IQueryable<IPackage> GetProjectPackages()
		{
			return FakeInstalledPackages.AsQueryable();
		}
	}
}
