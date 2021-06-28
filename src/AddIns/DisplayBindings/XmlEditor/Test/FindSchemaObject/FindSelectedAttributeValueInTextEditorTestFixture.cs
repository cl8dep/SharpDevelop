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
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	[TestFixture]
	public class FindSelectedAttributeValueInTextEditorTestFixture
	{
		SelectedXmlElement selectedElement;
		
		[SetUp]
		public void Init()
		{
			string xml = "<parent a='attributeValue'></parent>";
			int index = 15;
			
			MockTextEditor textEditor = new MockTextEditor();
			textEditor.Document.Text = xml;
			textEditor.Caret.Offset = index;
			
			selectedElement = new SelectedXmlElement(textEditor);
		}
		
		[Test]
		public void ElementPathContainsSingleRootElement()
		{
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("parent", String.Empty));
			
			Assert.AreEqual(path, selectedElement.Path);
		}
		
		[Test]
		public void HasSelectedAttribute()
		{
			Assert.IsTrue(selectedElement.HasSelectedAttribute);
		}
		
		[Test]
		public void HasSelectedAttributeValue()
		{
			Assert.IsTrue(selectedElement.HasSelectedAttributeValue);
		}
		
		[Test]
		public void SelectedAttributeIsFirstAttribute()
		{
			Assert.AreEqual("a", selectedElement.SelectedAttribute);
		}
		
		[Test]
		public void SelectedAttributeValueIsFirstAttributeValue()
		{
			Assert.AreEqual("attributeValue", selectedElement.SelectedAttributeValue);
		}
	}
}
