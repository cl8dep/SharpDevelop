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
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageActionRunnerTests
	{
		FakePackageActionRunner fakeConsoleActionRunner;
		PackageActionRunner runner;
		FakeInstallPackageAction fakeAction;
		FakePowerShellDetection powerShellDetection;
		FakePackageManagementEvents fakeEvents;
		List<FakeInstallPackageAction> fakeActions;
		
		void CreateRunner()
		{
			fakeConsoleActionRunner = new FakePackageActionRunner();
			powerShellDetection = new FakePowerShellDetection();
			fakeEvents = new FakePackageManagementEvents();
			fakeActions = new List<FakeInstallPackageAction>();
			runner = new PackageActionRunner(fakeConsoleActionRunner, fakeEvents, powerShellDetection);
		}
		
		void CreateInstallActionWithNoPowerShellScripts()
		{
			var fakeProject = new FakePackageManagementProject();
			fakeAction = new FakeInstallPackageAction(fakeProject);
			fakeAction.Operations = new PackageOperation[0];
			fakeActions.Add(fakeAction);
		}
		
		void CreateInstallActionWithOnePowerShellScript()
		{
			CreateInstallActionWithNoPowerShellScripts();
			
			fakeAction.Operations =
				PackageOperationHelper.CreateListWithOneInstallOperationWithFile(@"tools\init.ps1");
			fakeActions.Add(fakeAction);
		}
		
		void Run()
		{
			runner.Run(fakeAction);
		}
		
		void RunMultipleActions()
		{
			runner.Run(fakeActions);
		}
		
		[Test]
		public void Run_InstallActionHasNoPowerShellScripts_ActionIsExecutedDirectly()
		{
			CreateRunner();
			CreateInstallActionWithNoPowerShellScripts();
			Run();
			
			bool executed = fakeAction.IsExecuteCalled;
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_InstallActionHasOnePowerShellScript_ActionIsPassedToConsoleToRun()
		{
			CreateRunner();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = true;
			Run();
			
			IPackageAction action = fakeConsoleActionRunner.ActionPassedToRun;
			
			Assert.AreEqual(fakeAction, action);
		}
		
		[Test]
		public void Run_InstallActionHasOnePowerShellScript_ActionIsNotExecutedDirectly()
		{
			CreateRunner();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = true;
			Run();
			
			bool executed = fakeAction.IsExecuteCalled;
			
			Assert.IsFalse(executed);
		}
		
		[Test]
		public void Run_InstallActionHasOnePowerShellScriptAndPowerShellIsNotInstalled_ActionIsExecutedDirectly()
		{
			CreateRunner();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = false;
			
			Run();
			
			bool executed = fakeAction.IsExecuteCalled;
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_InstallActionHasOnePowerShellScriptAndPowerShellIsNotInstalled_MessageIsReportedThatPowerShellScriptsCannotBeRun()
		{
			CreateRunner();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = false;
			
			Run();
			
			string message = fakeEvents.FormattedStringPassedToOnPackageOperationMessageLogged;
			string expectedMessage = 
				"PowerShell is not installed. PowerShell scripts will not be run for the package.";
			
			Assert.AreEqual(expectedMessage, message);
		}
		
		[Test]
		public void Run_TwoInstallActionsWithoutPowerShellScripts_ActionsAreExecutedDirectly()
		{
			CreateRunner();
			CreateInstallActionWithNoPowerShellScripts();
			CreateInstallActionWithNoPowerShellScripts();
			RunMultipleActions();
			
			bool firstActionIsExecuted = fakeActions[0].IsExecuteCalled;
			bool secondActionIsExecuted = fakeActions[1].IsExecuteCalled;
			
			Assert.IsTrue(firstActionIsExecuted);
			Assert.IsTrue(secondActionIsExecuted);
		}
		
		[Test]
		public void Run_TwoInstallActionsAndSecondHasOnePowerShellScript_AllActionsPassedToConsoleToRun()
		{
			CreateRunner();
			CreateInstallActionWithNoPowerShellScripts();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = true;
			RunMultipleActions();
			
			IEnumerable<IPackageAction> actions = fakeConsoleActionRunner.ActionsRunInOneCall;
			
			CollectionAssert.AreEqual(fakeActions, actions);
		}
		
		[Test]
		public void Run_TwoInstallActionsBothWithOnePowerShellScriptsAndPowerShellIsNotInstalled_ActionsAreExecutedDirectly()
		{
			CreateRunner();
			CreateInstallActionWithOnePowerShellScript();
			CreateInstallActionWithOnePowerShellScript();
			powerShellDetection.IsPowerShell2InstalledReturnValue = false;
			
			RunMultipleActions();
			
			bool firstActionIsExecuted = fakeActions[0].IsExecuteCalled;
			bool secondActionIsExecuted = fakeActions[1].IsExecuteCalled;
			
			Assert.IsTrue(firstActionIsExecuted);
			Assert.IsTrue(secondActionIsExecuted);
		}
	}
}
