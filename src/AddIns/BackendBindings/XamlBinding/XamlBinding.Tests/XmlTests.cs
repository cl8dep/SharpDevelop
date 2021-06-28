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

using NUnit.Framework;
using System;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class XmlTests
	{
		XmlElementPath elementPath;
		XmlElementPath expectedElementPath;
		string namespaceURI = "http://foo.com/foo.xsd";

		[Test]
		public void PathTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void ComplexPathNewLineTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\n</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void ComplexPathTabTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\t</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void InMarkupExtensionTest()
		{
			string xaml = "<Test val1=\"{Binding Value}\" />";
			int offset = "<Test val1=\"{Bin".Length;
			
			Assert.AreEqual(true, XmlParser.IsInsideAttributeValue(xaml, offset));
			Assert.AreEqual("{Binding Value}", XmlParser.GetAttributeValueAtIndex(xaml, offset));
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void InMarkupExtensionNamedParameterTest()
		{
			string xaml = "<Test val1=\"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1=\"{Binding Value, Path=".Length;
			
			Assert.AreEqual(true, XmlParser.IsInsideAttributeValue(xaml, offset));
			Assert.AreEqual("{Binding Value, Path=Control}", XmlParser.GetAttributeValueAtIndex(xaml, offset));
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest1()
		{
			string xaml = "<Test val1 = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1 =".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest2()
		{
			string xaml = "<Test val1 = \"{Binding Value, Path=Control}\" />";
			int offset = "<Te".Length;
			
			Assert.AreEqual("", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest3()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest4()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1  ".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest5()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path=".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest6()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path=C".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
	}
}
