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
using System.Runtime.Versioning;

using ICSharpCode.PackageManagement.EnvDTE;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementProject
	{
		event EventHandler<PackageOperationEventArgs> PackageInstalled;
		event EventHandler<PackageOperationEventArgs> PackageUninstalled;
		event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;
		event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;
		
		string Name { get; }
		FrameworkName TargetFramework { get; }
		ILogger Logger { get; set; }
		IPackageRepository SourceRepository { get; }
		
		Project ConvertToDTEProject();
		
		IPackageConstraintProvider ConstraintProvider { get; }
		
		bool IsPackageInstalled(IPackage package);
		bool IsPackageInstalled(string packageId);
		bool HasOlderPackageInstalled(IPackage package);
		
		IQueryable<IPackage> GetPackages();
		IEnumerable<IPackage> GetPackagesInReverseDependencyOrder();
		
		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction installAction);
		IEnumerable<PackageOperation> GetUpdatePackagesOperations(IEnumerable<IPackage> packages, IUpdatePackageSettings settings);
		
		void InstallPackage(IPackage package, InstallPackageAction installAction);
		void UpdatePackage(IPackage package, UpdatePackageAction updateAction);
		void UninstallPackage(IPackage package, UninstallPackageAction uninstallAction);
		void UpdatePackages(UpdatePackagesAction action);
		
		void UpdatePackageReference(IPackage package, IUpdatePackageSettings settings);
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
		UpdatePackagesAction CreateUpdatePackagesAction();
		ReinstallPackageAction CreateReinstallPackageAction();
		
		void RunPackageOperations(IEnumerable<PackageOperation> expectedOperations);
		
		IPackage FindPackage(string packageId, SemanticVersion version);
	}
}
