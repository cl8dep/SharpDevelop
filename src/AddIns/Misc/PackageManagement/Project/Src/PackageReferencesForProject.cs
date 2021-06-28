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
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferencesForProject : IPackageReferencesForProject
	{
		MSBuildBasedProject project;
		IPackageReferenceFileFactory packageReferenceFileFactory;
		IPackageReferenceInstaller packageReferenceInstaller;
		IPackageReferenceFile packageReferenceFile;
		List<PackageReference> packageReferences;

		public PackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageRepositoryCache packageRepositoryCache)
			: this(
				project,
				new PackageReferenceInstaller(packageRepositoryCache),
				new PackageReferenceFileFactory())
		{
		}
		
		public PackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageReferenceInstaller packageReferenceInstaller,
			IPackageReferenceFileFactory packageReferenceFileFactory)
		{
			this.project = project;
			this.packageReferenceFileFactory = packageReferenceFileFactory;
			this.packageReferenceInstaller = packageReferenceInstaller;
		}
		
		public void RemovePackageReferences()
		{
			GetPackageReferences();
			IPackageReferenceFile file = PackageReferenceFile;
			file.Delete();
		}
		
		void GetPackageReferences()
		{
			if (packageReferences == null) {
				IEnumerable<PackageReference> packageReferencesInFile = PackageReferenceFile.GetPackageReferences();
				packageReferences = new List<PackageReference>(packageReferencesInFile);
			}
		}
		
		IPackageReferenceFile PackageReferenceFile {
			get {
				if (packageReferenceFile == null) {
					packageReferenceFile = GetPackageReferenceFile();
				}
				return packageReferenceFile;
			}
		}
		
		IPackageReferenceFile GetPackageReferenceFile()
		{
			var fileName = new PackageReferenceFileNameForProject(project);
			return packageReferenceFileFactory.CreatePackageReferenceFile(fileName.ToString());
		}
		
		public void InstallPackages()
		{
			GetPackageReferences();
			packageReferenceInstaller.InstallPackages(packageReferences, project);
		}
	}
}
