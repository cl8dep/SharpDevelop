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
using System.Xml;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogXmlGeneration
{
	[TestFixture]
	public class DialogXmlWritingTestFixture
	{
		WixDialogElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			dialogElement = (WixDialogElement)doc.SelectSingleNode("//w:Dialog", new WixNamespaceManager(doc.NameTable));
			dialogElement.SetAttribute("Id", "id");
			dialogElement.SetAttribute("Title", "title");
			XmlElement controlElement = doc.CreateElement("Control", WixNamespaceManager.Namespace);
			dialogElement.AppendChild(controlElement);
		}
		
		[Test]
		public void WixDocumentGetXmlWithTabs()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 4;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			
			string outputXml = dialogElement.GetXml(wixWriter);
			string expectedXml = 
				"<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\r\n" +
				"\t<Control />\r\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		[Test]
		public void WixDocumentGetXmlWithSpaces()
		{
			MockTextEditorOptions options = new MockTextEditorOptions();
			options.ConvertTabsToSpaces = true;
			options.IndentationSize = 4;
			
			WixTextWriter wixWriter = new WixTextWriter(options);
			
			string outputXml = dialogElement.GetXml(wixWriter);
			string expectedXml = 
				"<Dialog Id=\"id\" Height=\"270\" Width=\"370\" Title=\"title\">\r\n" +
				"    <Control />\r\n" +
				"</Dialog>";
			Assert.AreEqual(expectedXml, outputXml);
		}
		
		string GetWixXml()
		{
			return 
				"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
