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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests the WixProject.AddLibrary method.
	/// </summary>
	[TestFixture]
	public class AddWixLibraryTestFixture
	{
		WixProject project;
		int wixLibraryProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			string fileName1 = @"C:\Projects\Test\wixlibs\test.wixlib";
			string fileName2 = @"C:\Projects\Test\mainlibs\main.wixlib";
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.AddWixLibraries(new string[] {fileName1, fileName2});
			
			wixLibraryProjectItemCount = 0;
			foreach (ProjectItem item in project.Items) {
				if (item is WixLibraryProjectItem) {
					++wixLibraryProjectItemCount;
				}
			}
		}
		
		[Test]
		public void TwoWixLibraryItemsAdded()
		{
			Assert.AreEqual(2, wixLibraryProjectItemCount);
		}
		
		[Test]
		public void FirstWixLibraryItemInclude()
		{
			Assert.AreEqual(@"wixlibs\test.wixlib", project.Items.First().Include);
		}
		
		[Test]
		public void SecondWixLibraryItemInclude()
		{
			Assert.AreEqual(@"mainlibs\main.wixlib", project.Items.Skip(1).First().Include);
		}
	}
}
