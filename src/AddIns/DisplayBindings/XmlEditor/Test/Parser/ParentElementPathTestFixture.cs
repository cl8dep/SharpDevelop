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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Parser
{
	[TestFixture]
	public class ParentElementPathTestFixture 
	{		
		XmlElementPath elementPath;
		XmlElementPath expectedElementPath;
		string namespaceURI = "http://foo/foo.xsd";

		[Test]
		public void GetParentElementPathForRootElement()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ><";
			elementPath = XmlParser.GetParentElementPath(text);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void GetParentElementPathForRootElementAfterChildElementEnd()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ><bar></bar><";			
			elementPath = XmlParser.GetParentElementPath(text);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void GetParentElementPathForRootElementAfterEmptyChildElementEnd()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ><bar/><";
			elementPath = XmlParser.GetParentElementPath(text);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void GetParentElementPathForTwoElementsInDifferentNamespaces()
		{
			string text = "<bar xmlns='http://test.com'><foo xmlns='" + namespaceURI + "' ><";
			elementPath = XmlParser.GetParentElementPath(text);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("bar", "http://test.com"));
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
	}
}
