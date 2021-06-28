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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the loading of a simple Wix dialog that has two buttons.
	/// </summary>
	[TestFixture]
	public class LabelTestFixture : DialogLoadingTestFixtureBase
	{
		string labelName;
		string labelText;
		Size labelSize;
		Point labelLocation;
		string labelFontName;
		double labelFontSize;
		bool labelFontBold;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CreatedComponents.Clear();
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Label label = (Label)dialog.Controls[0];
				labelName = label.Name;
				labelText = label.Text;
				labelLocation = label.Location;
				labelSize = label.Size;
				labelFontName = label.Font.Name;
				labelFontSize = label.Font.Size;
				labelFontBold = label.Font.Bold;
			}
		}
		
		[Test]
		public void LabelName()
		{
			Assert.AreEqual("Title", labelName);
		}
		
		[Test]
		public void TwoControlsCreated()
		{
			Assert.AreEqual(2, CreatedComponents.Count);
		}
		
		[Test]
		public void LabelText()
		{
			Assert.AreEqual("{\\BigFont}Welcome to the [ProductName] installation", labelText);
		}
		
		[Test]
		public void LabelLocation()
		{
			int expectedX = Convert.ToInt32(135 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(20 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, labelLocation);
		}
		
		[Test]
		public void LabelSize()
		{
			int expectedWidth = Convert.ToInt32(220 * WixDialog.InstallerUnit);
			int expectedHeight = Convert.ToInt32(60 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, labelSize);
		}
		
		[Test]
		public void LabelFontName()
		{
			Assert.AreEqual("Verdana", labelFontName);
		}
		
		[Test]
		public void LabelFontSize()
		{
			Assert.AreEqual(13.0, labelFontSize);
		}
		
		[Test]
		public void LabelFontIsBold()
		{
			Assert.AreEqual(true, labelFontBold);
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<TextStyle Id='BigFont' FaceName='Verdana' Size='13' Bold='yes' />\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Title' Type='Text' X='135' Y='20' Width='220' Height='60' Transparent='yes' NoPrefix='yes'>\r\n" +
				"\t\t\t\t\t<Text>{\\BigFont}Welcome to the [ProductName] installation</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
