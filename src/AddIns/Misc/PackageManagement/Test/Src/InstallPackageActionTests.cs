// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstallPackageActionTests
	{
		FakePackageManagementEvents fakePackageManagementEvents;
		FakePackageManagementProject fakeProject;
		TestableInstallPackageAction action;
		InstallPackageHelper installPackageHelper;
		FakeFileService fileService;

		void CreateAction()
		{
			fakePackageManagementEvents = new FakePackageManagementEvents();
			fakeProject = new FakePackageManagementProject();
			action = new TestableInstallPackageAction(fakeProject, fakePackageManagementEvents);
			installPackageHelper = new InstallPackageHelper(action);
		}
		
		FakePackage AddOnePackageToProjectSourceRepository(string packageId)
		{
			return fakeProject.FakeSourceRepository.AddFakePackage(packageId);
		}
		
		void AddInstallOperationWithFile(string fileName)
		{
			action.Operations =
				PackageOperationHelper.CreateListWithOneInstallOperationWithFile(fileName);
		}
		
		IOpenPackageReadMeMonitor CreateReadMeMonitor(string packageId)
		{
			fileService = new FakeFileService(null);
			return new OpenPackageReadMeMonitor(packageId, fakeProject, fileService);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageIntoProject()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var actualPackage = fakeProject.PackagePassedToInstallPackage;
			var expectedPackage = installPackageHelper.TestPackage;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageUsingPackageOperations()
		{
			CreateAction();
			var expectedOperations = new List<PackageOperation>();
			installPackageHelper.PackageOperations = expectedOperations;
			installPackageHelper.InstallTestPackage();
			
			var actualOperations = fakeProject.PackageOperationsPassedToInstallPackage;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageIsSet_InstallsPackageNotIgnoringDependencies()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallTestPackage();
			
			bool ignored = fakeProject.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsFalse(ignored);
		}
		
		[Test]
		public void Execute_PackageIsSetAndIgnoreDependenciesIsTrue_InstallsPackageIgnoringDependencies()
		{
			CreateAction();
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallTestPackage();
			
			bool ignored = fakeProject.IgnoreDependenciesPassedToInstallPackage;
			
			Assert.IsTrue(ignored);
		}
		
		[Test]
		public void Execute_PackageIsSetAndAllowPrereleaseVersionsIsTrue_InstallsPackageAllowingPrereleaseVersions()
		{
			CreateAction();
			installPackageHelper.AllowPrereleaseVersions = true;
			installPackageHelper.InstallTestPackage();
			
			bool allowed = fakeProject.AllowPrereleaseVersionsPassedToInstallPackage;
			
			Assert.IsTrue(allowed);
		}
		
		[Test]
		public void Execute_PackageIsSetAndAllowPrereleaseVersionsIsFalse_InstallsPackageWithoutAllowingPrereleaseVersions()
		{
			CreateAction();
			installPackageHelper.AllowPrereleaseVersions = false;
			installPackageHelper.InstallTestPackage();
			
			bool allowed = fakeProject.AllowPrereleaseVersionsPassedToInstallPackage;
			
			Assert.IsFalse(allowed);
		}
		
		[Test]
		public void IgnoreDependencies_DefaultValue_IsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.IgnoreDependencies);
		}
		
		[Test]
		public void AllowPrereleaseVersion_DefaultValue_IsFalse()
		{
			CreateAction();
			Assert.IsFalse(action.AllowPrereleaseVersions);
		}
		
		[Test]
		public void Execute_PackageAndPackageRepositoryPassed_PackageInstallNotificationRaisedWithInstalledPackage()
		{
			CreateAction();
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			var actualPackage = fakePackageManagementEvents.PackagePassedToOnParentPackageInstalled;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassed_PackageOperationsRetrievedFromProject()
		{
			CreateAction();
			fakeProject.AddFakeInstallOperation();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualOperations = action.Operations;
			var expectedOperations = fakeProject.FakeInstallOperations;
			
			Assert.AreEqual(expectedOperations, actualOperations);
		}
		
		[Test]
		public void Execute_PackageSpecifiedButNoPackageOperations_PackageUsedWhenPackageOperationsRetrievedForProject()
		{
			CreateAction();
			installPackageHelper.PackageOperations = null;
			installPackageHelper.InstallTestPackage();
			
			var expectedPackage = installPackageHelper.TestPackage;
			
			var actualPackage = fakeProject.PackagePassedToGetInstallPackageOperations;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsTrue_DependenciesIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			installPackageHelper.IgnoreDependencies = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndAllowPrereleaseVersionsIsTrue_PrereleaseVersionsAllowedWhenGettingPackageOperations()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			installPackageHelper.AllowPrereleaseVersions = true;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void InstallPackage_PackageIdAndSourceAndProjectPassedAndIgnoreDependenciesIsFalse_DependenciesNotIgnoredWhenGettingPackageOperations()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			installPackageHelper.IgnoreDependencies = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.IgnoreDependenciesPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Execute_PackageIdAndSourceAndProjectPassedAndAllowPrereleaseVersionsIsFalse_PrereleaseVersionsNotAllowedWhenGettingPackageOperations()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("PackageId");
			installPackageHelper.AllowPrereleaseVersions = false;
			installPackageHelper.InstallPackageById("PackageId");
			
			bool result = fakeProject.AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
			
			Assert.IsFalse(result);
		}

		[Test]
		public void Execute_VersionSpecified_VersionUsedWhenSearchingForPackage()
		{
			CreateAction();
			
			var recentPackage = AddOnePackageToProjectSourceRepository("PackageId");
			recentPackage.Version = new SemanticVersion("1.2.0");
			
			var oldPackage = AddOnePackageToProjectSourceRepository("PackageId");
			oldPackage.Version = new SemanticVersion("1.0.0");
			
			var package = AddOnePackageToProjectSourceRepository("PackageId");
			var version = new SemanticVersion("1.1.0");
			package.Version = version;
			
			installPackageHelper.Version = version;
			installPackageHelper.InstallPackageById("PackageId");
			
			var actualPackage = fakeProject.PackagePassedToInstallPackage;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasInitPowerShellScript_ReturnsTrue()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			AddInstallOperationWithFile(@"tools\init.ps1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasNoFiles_ReturnsFalse()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			action.Operations = new List<PackageOperation>();
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsFalse(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasInitPowerShellScriptInUpperCase_ReturnsTrue()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			AddInstallOperationWithFile(@"tools\INIT.PS1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasInstallPowerShellScriptInUpperCase_ReturnsTrue()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			AddInstallOperationWithFile(@"tools\INSTALL.PS1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_OnePackageInOperationsHasUninstallPowerShellScriptInUpperCase_ReturnsTrue()
		{
			CreateAction();
			fakeProject.AddFakePackageToSourceRepository("Test");
			action.PackageId = "Test";
			AddInstallOperationWithFile(@"tools\UNINSTALL.PS1");
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			Assert.IsTrue(hasPackageScripts);
		}
		
		[Test]
		public void HasPackageScriptsToRun_ProjectHasOnePackageOperation_DoesNotThrowNullReferenceException()
		{
			CreateAction();
			FakePackage package = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("Test", "1.0");
			var operation = new FakePackageOperation(package, PackageAction.Install);
			action.PackageId = package.Id;
			action.PackageVersion = package.Version;
			fakeProject.FakeInstallOperations.Add(operation);
			
			bool hasPackageScripts = false;
			Assert.DoesNotThrow(() => hasPackageScripts = action.HasPackageScriptsToRun());
		}
		
		[Test]
		public void HasPackageScriptsToRun_ProjectHasOnePackageOperation_PackageLocated()
		{
			CreateAction();
			FakePackage expectedPackage = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("Test", "1.0");
			var operation = new FakePackageOperation(expectedPackage, PackageAction.Install);
			action.PackageId = expectedPackage.Id;
			action.PackageVersion = expectedPackage.Version;
			fakeProject.FakeInstallOperations.Add(operation);
			
			bool hasPackageScripts = action.HasPackageScriptsToRun();
			
			IPackage actualPackage = action.Package;
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_InstallPrereleasePackageAndAllowPreleasePackagesIsFalse_DoesNotFindPreleasePackage()
		{
			CreateAction();
			FakePackage package = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("Prerelease", "1.0-beta");
			action.PackageId = "Prerelease";
			action.AllowPrereleaseVersions = false;
			
			Exception ex = Assert.Throws(typeof(ApplicationException), () => action.Execute());
			
			Assert.AreEqual("Unable to find package 'Prerelease'.", ex.Message);
		}
		
		[Test]
		public void Execute_InstallPrereleasePackageAndAllowPreleasePackagesIsTrue_InstallsPackageIntoProject()
		{
			CreateAction();
			FakePackage expectedPackage = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("Prerelease", "1.0-beta");
			action.PackageId = "Prerelease";
			action.AllowPrereleaseVersions = true;
			
			action.Execute();
			
			IPackage actualPackage = fakeProject.PackagePassedToInstallPackage;
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_InstallUnlistedPackage_InstallsPackageIntoProject()
		{
			CreateAction();
			FakePackage expectedPackage = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("test", "1.0");
			expectedPackage.Listed = false;
			action.PackageId = "test";
			action.PackageVersion = new SemanticVersion("1.0");
			
			action.Execute();
			
			IPackage actualPackage = fakeProject.PackagePassedToInstallPackage;
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Execute_InstallUnlistedPackageWithoutVersion_DoesNotInstallPackageIntoProject()
		{
			CreateAction();
			FakePackage expectedPackage = fakeProject.FakeSourceRepository.AddFakePackageWithVersion("test", "1.0");
			expectedPackage.Listed = false;
			action.PackageId = "test";
			
			Exception ex = Assert.Throws(typeof(ApplicationException), () => action.Execute());
			
			Assert.AreEqual("Unable to find package 'test'.", ex.Message);
		}
		
		[Test]
		public void Execute_PackageIdSpecifiedButDoesNotExistInRepository_ExceptionThrown()
		{
			CreateAction();
			action.PackageId = "UnknownId";
			
			Exception ex = Assert.Throws(typeof(ApplicationException), () => action.Execute());
			
			Assert.AreEqual("Unable to find package 'UnknownId'.", ex.Message);
		}
		
		[Test]
		public void Execute_PackageInstalledSuccessfully_OpenPackageReadmeMonitorCreated()
		{
			CreateAction();
			installPackageHelper.TestPackage.Id = "Test";
			installPackageHelper.InstallTestPackage();
			
			Assert.AreEqual("Test", action.OpenPackageReadMeMonitor.PackageId);
			Assert.IsTrue(action.OpenPackageReadMeMonitor.IsDisposed);
		}
		
		[Test]
		public void Execute_PackageInstalledSuccessfullyWithReadmeTxt_ReadmeTxtFileIsOpened()
		{
			CreateAction();
			installPackageHelper.TestPackage.Id = "Test";
			installPackageHelper.TestPackage.AddFile("readme.txt");
			action.CreateOpenPackageReadMeMonitorAction = packageId => {
				return CreateReadMeMonitor(packageId);
			};
			string installPath = @"d:\projects\myproject\packages\Test.1.0";
			string readmeFileName = Path.Combine(installPath, "readme.txt");
			fakeProject.InstallPackageAction = (package, installAction) => {
				var eventArgs = new PackageOperationEventArgs(package, null, installPath);
				fakeProject.FirePackageInstalledEvent(eventArgs);
				fileService.ExistingFileNames.Add(readmeFileName);
			};
			installPackageHelper.InstallTestPackage ();

			Assert.IsTrue(fileService.IsOpenFileCalled);
			Assert.AreEqual(readmeFileName, fileService.FileNamePassedToOpenFile);
		}

		[Test]
		public void Execute_PackageWithReadmeTxtIsInstalledButExceptionThrownWhenAddingPackageToProject_ReadmeFileIsNotOpened()
		{
			CreateAction();
			installPackageHelper.TestPackage.Id = "Test";
			installPackageHelper.TestPackage.AddFile("readme.txt");
			OpenPackageReadMeMonitor monitor = null;
			action.CreateOpenPackageReadMeMonitorAction = packageId => {
				monitor = CreateReadMeMonitor(packageId) as OpenPackageReadMeMonitor;
				return monitor;
			};
			string installPath = @"d:\projects\myproject\packages\Test.1.0";
			string readmeFileName = Path.Combine(installPath, "readme.txt");
			fakeProject.InstallPackageAction = (package, installAction) => {
				var eventArgs = new PackageOperationEventArgs(package, null, installPath);
				fakeProject.FirePackageInstalledEvent(eventArgs);
				fileService.ExistingFileNames.Add(readmeFileName);
				throw new ApplicationException();
			};
			Assert.Throws<ApplicationException> (() => {
				installPackageHelper.InstallTestPackage ();
			});

			Assert.IsFalse(fileService.IsOpenFileCalled);
			Assert.IsTrue(monitor.IsDisposed);
		}
		
		[Test]
		public void DependencyVersion_DefaultValue_IsLowest()
		{
			CreateAction();
			Assert.AreEqual(DependencyVersion.Lowest, action.DependencyVersion);
		}
		
		[Test]
		public void OpenReadMeText_DefaultValue_IsTrue()
		{
			CreateAction();
			Assert.IsTrue(action.OpenReadMeText);
		}

		[Test]
		public void Execute_OpenReadMeTextSetToFalse_NullOpenPackageReadmeMonitorCreated()
		{
			CreateAction();
			action.OpenReadMeText = false;
			installPackageHelper.TestPackage.Id = "Test";
			installPackageHelper.InstallTestPackage();

			Assert.IsTrue(action.NullOpenPackageReadMeMonitorIsCreated);
		}
	}
}
