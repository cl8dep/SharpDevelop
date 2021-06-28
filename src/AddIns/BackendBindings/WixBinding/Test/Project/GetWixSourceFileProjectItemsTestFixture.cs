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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProject.WixSourceFiles property only returns
	/// FileProjectItems that have a .wxs file extension.
	/// </summary>
	[TestFixture]
	public class GetWixSourceFileProjectItemsTestFixture
	{
		FileProjectItem wixSetupFile;
		FileProjectItem wixDialogsFile;
		int wixFileProjectItemCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			FileProjectItem item = new FileProjectItem(p, ItemType.None);
			item.Include = "readme.txt";
			ProjectService.AddProjectItem(p, item);
			
			ReferenceProjectItem referenceItem = new ReferenceProjectItem(p);
			referenceItem.Include = "System.Windows.Forms";
			ProjectService.AddProjectItem(p, referenceItem);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "setup.wxs";
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "test.wxi";
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "dialogs.wxs";
			ProjectService.AddProjectItem(p, item);
			
			wixFileProjectItemCount = 0;
			
			foreach (FileProjectItem fileItem in p.WixSourceFiles) {
				wixFileProjectItemCount++;
			}
			
			wixSetupFile = p.WixSourceFiles[0];
			wixDialogsFile = p.WixSourceFiles[1];
		}
		
		[Test]
		public void TwoWixSourceFiles()
		{
			Assert.AreEqual(2, wixFileProjectItemCount);
		}
		
		[Test]
		public void WixSetupFileProjectItemInclude()
		{
			Assert.AreEqual("setup.wxs", wixSetupFile.Include);
		}
		
		[Test]
		public void WixDialogsFileProjectItemInclude()
		{
			Assert.AreEqual("dialogs.wxs", wixDialogsFile.Include);
		}
	}
}
