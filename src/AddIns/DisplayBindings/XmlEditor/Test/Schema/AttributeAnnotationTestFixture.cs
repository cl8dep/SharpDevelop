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
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the completion data retrieves the annotation documentation
	/// that an attribute may have.
	/// </summary>
	[TestFixture]
	public class AttributeAnnotationTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection fooAttributeCompletionData;
		XmlCompletionItemCollection barAttributeCompletionData;
		
		public override void FixtureInit()
		{			
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("foo", "http://foo.com"));
			
			fooAttributeCompletionData = SchemaCompletion.GetAttributeCompletion(path);

			path.AddElement(new QualifiedName("bar", "http://foo.com"));
			barAttributeCompletionData = SchemaCompletion.GetAttributeCompletion(path);
		}
				
		[Test]
		public void FooAttributeDocumentation()
		{
			Assert.AreEqual("Documentation for foo attribute.", fooAttributeCompletionData[0].Description);
		}
		
		[Test]
		public void BarAttributeDocumentation()
		{
			Assert.AreEqual("Documentation for bar attribute.", barAttributeCompletionData[0].Description);
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://foo.com\" xmlns=\"http://foo.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:sequence>\t\r\n" +
				"\t\t\t\t<xs:element name=\"bar\" type=\"bar\">\r\n" +
				"\t\t\t</xs:element>\r\n" +
				"\t\t\t</xs:sequence>\r\n" +
				"\t\t\t<xs:attribute name=\"id\">\r\n" +
				"\t\t\t\t\t<xs:annotation>\r\n" +
				"\t\t\t\t\t\t<xs:documentation>Documentation for foo attribute.</xs:documentation>\r\n" +
				"\t\t\t\t</xs:annotation>\t\r\n" +
				"\t\t\t</xs:attribute>\t\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"\t<xs:complexType name=\"bar\">\r\n" +
				"\t\t<xs:attribute name=\"name\">\r\n" +
				"\t\t\t<xs:annotation>\r\n" +
				"\t\t\t\t<xs:documentation>Documentation for bar attribute.</xs:documentation>\r\n" +
				"\t\t\t</xs:annotation>\t\r\n" +
				"\t\t</xs:attribute>\t\r\n" +
				"\t</xs:complexType>\r\n" +
				"</xs:schema>";
		}		
	}
}
