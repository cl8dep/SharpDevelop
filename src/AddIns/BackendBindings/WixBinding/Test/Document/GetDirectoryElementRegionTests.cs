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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that we can determine the target directory element's region in the Wix 
	/// document.
	/// </summary>
	[TestFixture]
	public class GetDirectoryElementRegionTests
	{
		[Test]
		public void SingleDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 4, 14);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void OneNestedDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFiles' Name='PFiles'>\r\n" +
				"\t\t\t</Directory>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 6, 14);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void OneNestedEmptyDirectoryElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Directory Id='TARGETDIR' Name='SourceDir'>\r\n" +
				"\t\t\t<Directory Id='ProgramFiles' Name='PFiles'/>\r\n" +
				"\t\t</Directory>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Directory", "TARGETDIR");
			DomRegion expectedRegion = new DomRegion(3, 3, 5, 14);
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
