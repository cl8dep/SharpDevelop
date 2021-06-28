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

using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that WixBinaries class does not try to add the same binary id to the hash table.
	/// </summary>
	[TestFixture]
	public class DuplicateBinaryIdTestFixture : ITextFileReader
	{
		WixBinaries binaries;
		string projectDirectory;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			projectDirectory = p.Directory;
			p.Name = "MySetup";
			
			FileProjectItem item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Setup.wxs";
			string docFileName = item.FileName;
			ProjectService.AddProjectItem(p, item);
						
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Binaries.wxs";
			ProjectService.AddProjectItem(p, item);

			WixDocument doc = new WixDocument(p);
			doc.FileName = docFileName;
			doc.LoadXml(GetMainWixXml());
			
			binaries = new WixBinaries(doc, this);
		}
		
		
		[Test]
		public void GetDialogBitmapFileName()
		{
			string expectedFileName = Path.Combine(projectDirectory, "Bitmaps/Dialog.bmp");
			Assert.AreEqual(expectedFileName, binaries.GetBinaryFileName("Dialog"));
		}
		
		
		public TextReader Create(string fileName)
		{
			fileName = Path.GetFileName(fileName);
			if (fileName == "Binaries.wxs") {
				return new StringReader(GetWixFragmentXml());
			}
			return new StringReader(GetMainWixXml());
		}
		
		string GetMainWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Product Name=\"MySetup\" \r\n" +
				"\t         Manufacturer=\"\" \r\n" +
				"\t         Id=\"F4A71A3A-C271-4BE8-B72C-F47CC956B3AA\" \r\n" +
				"\t         Language=\"1033\" \r\n" +
				"\t         Version=\"1.0.0.0\">\r\n" +
				"\t\t<Package Id=\"6B8BE64F-3768-49CA-8BC2-86A76424DFE9\"/>\r\n" +
				"\t</Product>\r\n" +
				"</Wix>";
		}
		
		string GetWixFragmentXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='Dialog' SourceFile='Bitmaps/Dialog.bmp'/>\r\n" +
				"\t\t<Binary Id='Dialog' SourceFile='Bitmaps/Dialog.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
