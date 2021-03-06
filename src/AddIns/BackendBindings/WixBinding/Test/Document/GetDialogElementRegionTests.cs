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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests whether the dialog xml element with the specified id is correctly 
	/// located in the wix xml.
	/// </summary>
	[TestFixture]
	public class GetDialogElementRegionTests
	{
		[Test]
		public void DialogSpansTwoLines()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Dialog", "WelcomeDialog");
			DomRegion expectedRegion = new DomRegion(4, 4, 5, 12);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void DialogSpansOneLine()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"<Dialog Id='WelcomeDialog'></Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Dialog", "WelcomeDialog");
			DomRegion expectedRegion = new DomRegion(4, 1, 4, 36);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void EmptyDialogElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"<Dialog Id='WelcomeDialog'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Dialog", "WelcomeDialog");
			DomRegion expectedRegion = new DomRegion(4, 1, 4, 28);
			Assert.AreEqual(expectedRegion, region);
		}
	
		[Test]
		public void ElementStartsImmediatelyAfterDialogEndElement()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"<Dialog Id='WelcomeDialog'></Dialog><Property/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Dialog", "WelcomeDialog");
			DomRegion expectedRegion = new DomRegion(4, 1, 4, 36);
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void TwoDialogs()
		{
			string xml = "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='IgnoreThisDialog' Height='100' Width='200'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
			WixDocumentReader wixReader = new WixDocumentReader(xml);
			DomRegion region = wixReader.GetElementRegion("Dialog", "WelcomeDialog");
			DomRegion expectedRegion = new DomRegion(6, 4, 7, 12);
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
