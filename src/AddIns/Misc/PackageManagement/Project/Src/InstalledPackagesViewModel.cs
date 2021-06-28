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
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class InstalledPackagesViewModel : PackagesViewModel
	{
		public InstalledPackagesViewModel(
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
			RegisterEvents();
		}
		
		void RegisterEvents()
		{
			packageManagementEvents.ParentPackageInstalled += InstalledPackagesChanged;
			packageManagementEvents.ParentPackageUninstalled += InstalledPackagesChanged;
			packageManagementEvents.ParentPackagesUpdated += InstalledPackagesChanged;
		}
		
		protected override void OnDispose()
		{
			packageManagementEvents.ParentPackageInstalled -= InstalledPackagesChanged;
			packageManagementEvents.ParentPackageUninstalled -= InstalledPackagesChanged;
			packageManagementEvents.ParentPackagesUpdated -= InstalledPackagesChanged;
		}
		
		void InstalledPackagesChanged(object sender, EventArgs e)
		{
			ReadPackages();
		}
		
		protected override IQueryable<IPackage> GetAllPackages(string searchCriteria)
		{
			if (!string.IsNullOrEmpty(errorMessage)) {
				ThrowOriginalExceptionWhenTryingToGetProjectManager();
			}
			if (project != null) {
				return project.GetPackages();
			}
			return solution.GetPackages();
		}
		
		void ThrowOriginalExceptionWhenTryingToGetProjectManager()
		{
			throw new ApplicationException(errorMessage);
		}
	}
}
