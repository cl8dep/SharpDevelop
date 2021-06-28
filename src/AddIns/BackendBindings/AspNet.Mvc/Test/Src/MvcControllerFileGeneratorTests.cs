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
using System.CodeDom.Compiler;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcControllerFileGeneratorTests : MvcTestsBase
	{
		MvcControllerFileGenerator generator;
		FakeMvcProject projectUsedByGenerator;
		FakeMvcTextTemplateHostFactory fakeHostFactory;
		FakeMvcTextTemplateHost fakeHost;
		FakeMvcTextTemplateAppDomainFactory fakeAppDomainFactory;
		IMvcFileGenerationErrorReporter fakeErrorReporter;
		
		void CreateGenerator()
		{
			fakeHostFactory = new FakeMvcTextTemplateHostFactory();
			fakeHost = fakeHostFactory.FakeMvcTextTemplateHost;
			fakeAppDomainFactory = new FakeMvcTextTemplateAppDomainFactory();
			fakeErrorReporter = MockRepository.GenerateStub<IMvcFileGenerationErrorReporter>();
			generator = new MvcControllerFileGenerator(fakeHostFactory, fakeAppDomainFactory, fakeErrorReporter);
			projectUsedByGenerator = new FakeMvcProject();
			generator.Project = projectUsedByGenerator;
			ProjectPassedToGeneratorIsCSharpProject();
		}
		
		void ProjectPassedToGeneratorIsCSharpProject()
		{
			projectUsedByGenerator.SetCSharpAsTemplateLanguage();
			generator.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
		}
		
		void ProjectPassedToGeneratorIsVisualBasicProject()
		{
			projectUsedByGenerator.SetVisualBasicAsTemplateLanguage();
			generator.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
		}
		
		void GenerateFile()
		{
			GenerateFile(@"d:\projects\myproject\Controllers", "Home");
		}
		
		void GenerateFile(string folder, string name)
		{
			var fileName = new MvcControllerFileName();
			fileName.Folder = folder;
			fileName.ControllerName = name;
			GenerateFile(fileName);
		}
		
		void GenerateFile(MvcControllerFileName fileName)
		{
			generator.GenerateFile(fileName);
		}
		
		MvcControllerTextTemplate CreateControllerTemplate(string fileName)
		{
			return new MvcControllerTextTemplate() {
				FileName = fileName
			};
		}
		
		CompilerError AddCompilerErrorToTemplateHost()
		{
			return fakeHost.AddCompilerError();
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostIsCreated()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			IMvcProject project = fakeHostFactory.ProjectPassedToCreateMvcTextTemplateHost;
			
			Assert.AreEqual(projectUsedByGenerator, project);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostIsDisposed()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			bool disposed = fakeHost.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_OutputFileGeneratedUsingFileNamePassedToGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "HomeController";
			GenerateFile(controllerFolder, controllerName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Controllers\HomeController.cs";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostControllerNameIsSet()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "AboutController";
			GenerateFile(controllerFolder, controllerName);
		
			Assert.AreEqual("AboutController", fakeHost.ControllerName);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostNamespaceIsTakenFromProject()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			projectUsedByGenerator.RootNamespace = "MyProjectNamespace";
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "AboutController";
			GenerateFile(controllerFolder, controllerName);
			
			string expectedNamespace = "MyProjectNamespace.Controllers";
			
			Assert.AreEqual(expectedNamespace, fakeHost.Namespace);
		}
		
		[Test]
		public void GenerateFile_ControllerTemplateIsSet_ControllerTemplateFileNameUsedWhenGeneratingNewFile()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			string expectedFileName = @"d:\templates\controller.tt";
			MvcControllerTextTemplate template = CreateControllerTemplate(expectedFileName);
			generator.Template = template;
			GenerateFile();
			
			string fileName = fakeHost.InputFilePassedToProcessTemplate;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GenerateFile_ControllerTemplateHasAddActionsSetToTrue_MvcTextTemplateHostAddActionMethodsIsTrue()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			MvcControllerTextTemplate template = CreateControllerTemplate(@"d:\templates\controller.tt");
			template.AddActionMethods = true;
			generator.Template = template;
			GenerateFile();
			
			bool addActionMethods = fakeHost.AddActionMethods;
			
			Assert.IsTrue(addActionMethods);
		}
		
		[Test]
		public void GenerateFile_ControllerTemplateHasAddActionsSetToFalse_MvcTextTemplateHostAddActionMethodsIsFalse()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			MvcControllerTextTemplate template = CreateControllerTemplate(@"d:\templates\controller.tt");
			template.AddActionMethods = false;
			generator.Template = template;
			GenerateFile();
			
			bool addActionMethods = fakeHost.AddActionMethods;
			
			Assert.IsFalse(addActionMethods);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_AppDomainIsDisposed()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			bool disposed = fakeAppDomainFactory.FakeAppDomain.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostIsCreatedWithAppDomain()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			
			IMvcTextTemplateHostAppDomain appDomain = fakeHostFactory.AppDomainPassedToCreateMvcTextTemplateHost;
			FakeMvcTextTemplateAppDomain expectedAppDomain = fakeAppDomainFactory.FakeAppDomain;
			
			Assert.AreEqual(expectedAppDomain, appDomain);
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedWithCompilerError_ErrorsSavedByGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			CompilerError error = AddCompilerErrorToTemplateHost();
			
			GenerateFile();
			
			Assert.AreEqual(1, generator.Errors.Count);
			Assert.AreEqual(error, generator.Errors[0]);
		}
		
		[Test]
		public void HasErrors_NoErrors_ReturnsFalse()
		{
			CreateGenerator();
			GenerateFile();
			
			bool result = generator.HasErrors;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasErrors_OneError_ReturnsTrue()
		{
			CreateGenerator();
			AddCompilerErrorToTemplateHost();
			GenerateFile();
			
			bool result = generator.HasErrors;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedWithCompilerError_ErrorsReported()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			CompilerError error = AddCompilerErrorToTemplateHost();
			
			GenerateFile();
			
			fakeErrorReporter.AssertWasCalled(reporter => reporter.ShowErrors(generator.Errors));
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedNoErrors_NoErrorsReported()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			GenerateFile();
			
			fakeErrorReporter.AssertWasNotCalled(reporter => reporter.ShowErrors(Arg<CompilerErrorCollection>.Is.Anything));
		}
	}
}
