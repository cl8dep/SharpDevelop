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

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class XsltSchemaTests
	{
		string namespaceURI = "http://www.w3.org/1999/XSL/Transform";
		XmlSchemaCompletion schemaCompletion;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			schemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXsltSchema());
		}
		
		[Test]
		public void GetChildElementCompletion_StylesheetElement_SubstitutionGroupUsedForTemplateAndTemplateElementReturned()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("stylesheet", namespaceURI));
			
			XmlCompletionItemCollection completionItems = schemaCompletion.GetChildElementCompletion(path);
			bool contains = completionItems.Contains("template");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void GetAttributeCompletion_TemplateElementIsChildOfStylesheetElement_SubstitutionGroupUsedForTemplateAndMatchAttributeReturned()
		{
			var path = new XmlElementPath();
			path.AddElement(new QualifiedName("stylesheet", namespaceURI));
			path.AddElement(new QualifiedName("template", namespaceURI));
			
			XmlCompletionItemCollection completionItems = schemaCompletion.GetAttributeCompletion(path);
			bool contains = completionItems.Contains("match");
			
			Assert.IsTrue(contains);
		}
	}
}
