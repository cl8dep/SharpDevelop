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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateTestProjectWithOneTestMethodTestFixture
	{
		TestProject testProject;
		TestClass testClass;
		TestMember testMethod;
		
		[SetUp]
		public void Init()
		{
			testProject = 
				TestProjectHelper.CreateTestProjectWithTestClassAndSingleTestMethod("MyNamespace.MyClass", "MyTestMethod");
			
			if (testProject.TestClasses.Count > 0) {
				testClass = testProject.TestClasses[0];
				if (testClass.Members.Count > 0) {
					testMethod = testClass.Members[0];
				}
			}	
		}
		
		[Test]
		public void TestProjectNameIsTestProject()
		{
			string expectedName = "TestProject";
			Assert.AreEqual(expectedName, testProject.Name);
		}
		
		[Test]
		public void TestProjectNameMatchesProjectName()
		{
			Assert.AreEqual(testProject.Project.Name, testProject.Name);
		}
		
		[Test]
		public void TestProjectHasOneTestClass()
		{
			Assert.AreEqual(1, testProject.TestClasses.Count);
		}
		
		[Test]
		public void TestClassDotNetNameIsMyNamespaceMyClass()
		{
			string expectedName = "MyNamespace.MyClass";
			Assert.AreEqual(expectedName, testClass.Class.DotNetName);
		}
		
		[Test]
		public void TestClassCompilationUnitFileNameIsProjectsTestsMyTestsCs()
		{
			string fileName = @"c:\projects\tests\MyTests.cs";
			Assert.AreEqual(fileName, testClass.Class.CompilationUnit.FileName);
		}
		
		[Test]
		public void TestClassHasOneTestMethod()
		{
			Assert.AreEqual(1, testClass.Members.Count);
		}
		
		[Test]
		public void TestMethodNameIsMyTestMethod()
		{
			Assert.AreEqual("MyTestMethod", testMethod.Name);
		}
		
		[Test]
		public void TestMethodDeclaringTypeIsNotNull()
		{
			Assert.IsNotNull(testMethod.Member.DeclaringType);
		}
		
		[Test]
		public void TestMethodDeclaringTypeEqualsTestClass()
		{
			Assert.AreEqual(testClass.Class, testMethod.Member.DeclaringType);
		}
		
		[Test]
		public void TestMethodRegionIsLine4Column20()
		{
			DomRegion expectedRegion = new DomRegion(4, 20);
			Assert.AreEqual(expectedRegion, testMethod.Member.Region);
		}
		
		[Test]
		public void ClassHasOneMethod()
		{
			Assert.AreEqual(1, testClass.Class.Methods.Count);
		}
		
		[Test]
		public void ClassMethodMatchesTestMethodMethod()
		{
			Assert.AreEqual(testClass.Class.Methods[0], testMethod.Member);
		}
	}
}
