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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializeScriptTests
	{
		PackageInitializeScript script;
		FakePackageScriptSession fakeSession;
		FakePackageScriptFileName fakeScriptFileName;
		FakePackage fakePackage;
		
		void CreateScript()
		{
			fakeScriptFileName = new FakePackageScriptFileName();
			fakeSession = new FakePackageScriptSession();
			fakePackage = new FakePackage();
			script = new PackageInitializeScript(fakePackage, fakeScriptFileName);
		}
		
		void AssertSessionVariableIsRemoved(string variableName)
		{
			bool removed = fakeSession.VariablesRemoved.Contains(variableName);
			Assert.IsTrue(removed);			
		}
		
		void RunScript()
		{
			script.Run(fakeSession);
		}
		
		[Test]
		public void Run_ExistingEnvironmentPathIsEmptyString_PathToScriptAddedToEnvironmentPath()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			fakeSession.SetEnvironmentPath(String.Empty);
			
			RunScript();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Run_OneItemInOriginalEnvironmentPath_PathToScriptAppendedToEnvironmentPath()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			fakeSession.SetEnvironmentPath(@"c:\users\sharpdevelop\ps;");
			
			RunScript();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"c:\users\sharpdevelop\ps;d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Run_OneItemInOriginalEnvironmentPathMissingSemiColonAtEnd_PathToScriptAppendedToEnvironmentPathWithSemiColonAtStart()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			fakeSession.SetEnvironmentPath(@"c:\users\sharpdevelop\ps");
			
			RunScript();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"c:\users\sharpdevelop\ps;d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Run_OriginalEnvironmentPathIsNull_PathToScriptAppendedToEnvironmentPath()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			fakeSession.SetEnvironmentPath(null);
			
			RunScript();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = @"d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Run_ScriptDirectoryDoesNotExist_PathIsNotUpdated()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			fakeScriptFileName.ScriptDirectoryExistsReturnValue = false;
			fakeSession.SetEnvironmentPath(String.Empty);
			
			RunScript();
			
			string actualEnvironmentPath = fakeSession.GetEnvironmentPath();
			string expectedEnvironmentPath = String.Empty;
			
			Assert.AreEqual(expectedEnvironmentPath, actualEnvironmentPath);
		}
		
		[Test]
		public void Run_PackageIsSet_PackageSessionVariableIsSet()
		{
			CreateScript();
			var expectedPackage = new FakePackage("Test");
			script.Package = expectedPackage;
			RunScript();
			
			var actualPackage = fakeSession.VariablesAdded["__package"];
			
			Assert.AreEqual(expectedPackage, actualPackage);
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_RootPathSessionVariableIsSet()
		{
			CreateScript();
			string expectedRootPath = @"d:\projects\myproject\packages\test";
			fakeScriptFileName.PackageInstallDirectory = expectedRootPath;
			RunScript();
			
			var rootPath = fakeSession.VariablesAdded["__rootPath"];
			
			Assert.AreEqual(expectedRootPath, rootPath);
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_ToolsPathSessionVariableIsSet()
		{
			CreateScript();
			fakeScriptFileName.GetScriptDirectoryReturnValue = @"d:\projects\myproject\packages\test\tools";
			RunScript();
			
			var toolsPath = fakeSession.VariablesAdded["__toolsPath"];
			string expectedToolsPath = @"d:\projects\myproject\packages\test\tools";
			
			Assert.AreEqual(expectedToolsPath, toolsPath);
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_ProjectSessionVariableIsSet()
		{
			CreateScript();
			RunScript();
			
			var project = fakeSession.VariablesAdded["__project"];
			
			Assert.IsNull(project);
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_ScriptIsInvoked()
		{
			CreateScript();
			fakeScriptFileName.ToStringReturnValue = @"d:\projects\myproject\packages\test\tools\init.ps1";
			RunScript();
			
			string actualScript = fakeSession.ScriptPassedToInvokeScript;
			
			string expectedScript = 
				"& 'd:\\projects\\myproject\\packages\\test\\tools\\init.ps1' $__rootPath $__toolsPath $__package $__project";
			
			Assert.AreEqual(expectedScript, actualScript);
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_PackageSessionVariableIsRemoved()
		{
			CreateScript();
			RunScript();
			
			AssertSessionVariableIsRemoved("__package");
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_RootPathSessionVariableIsRemoved()
		{
			CreateScript();
			RunScript();
			
			AssertSessionVariableIsRemoved("__rootPath");
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_ToolsPathSessionVariableIsRemoved()
		{
			CreateScript();
			RunScript();
			
			AssertSessionVariableIsRemoved("__toolsPath");
		}
		
		[Test]
		public void Run_PackageInstallDirectoryIsSet_ProjectSessionVariableIsRemoved()
		{
			CreateScript();
			RunScript();
			
			AssertSessionVariableIsRemoved("__project");
		}
		
		[Test]
		public void Package_PackagePassedToConstructor_ReturnsPackagePassedToConstructor()
		{
			CreateScript();
			
			IPackage package = script.Package;
			
			Assert.AreEqual(fakePackage, package);
		}
	}
}
