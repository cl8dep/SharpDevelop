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
	/// Tests element group refs
	/// </summary>
	[TestFixture]
	public class GroupRefTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection childElements;
		XmlCompletionItemCollection paraAttributes;
		
		public override void FixtureInit()
		{
			XmlElementPath path = new XmlElementPath();
			
			path.AddElement(new QualifiedName("html", "http://foo/xhtml"));
			path.AddElement(new QualifiedName("body", "http://foo/xhtml"));
			
			childElements = SchemaCompletion.GetChildElementCompletion(path);
			
			path.AddElement(new QualifiedName("p", "http://foo/xhtml"));
			paraAttributes = SchemaCompletion.GetAttributeCompletion(path);
		}
		
		[Test]
		public void BodyHasFourChildElements()
		{
			Assert.AreEqual(4, childElements.Count, 
			                "Should be 4 child elements.");
		}
		
		[Test]
		public void BodyChildElementForm()
		{
			Assert.IsTrue(childElements.Contains("form"), 
			              "Should have a child element called form.");
		}
		
		[Test]
		public void BodyChildElementPara()
		{
			Assert.IsTrue(childElements.Contains( "p"), 
			              "Should have a child element called p.");
		}		
		
		[Test]
		public void BodyChildElementTest()
		{
			Assert.IsTrue(childElements.Contains("test"), 
			              "Should have a child element called test.");
		}		
		
		[Test]
		public void BodyChildElementId()
		{
			Assert.IsTrue(childElements.Contains("id"), 
			              "Should have a child element called id.");
		}		
		
		[Test]
		public void ParaElementHasIdAttribute()
		{
			Assert.IsTrue(paraAttributes.Contains("id"), 
			              "Should have an attribute called id.");			
		}
		
		protected override string GetSchema()
		{
			return "<xs:schema version=\"1.0\" xml:lang=\"en\"\r\n" +
				"    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"    targetNamespace=\"http://foo/xhtml\"\r\n" +
				"    xmlns=\"http://foo/xhtml\"\r\n" +
				"    elementFormDefault=\"qualified\">\r\n" +
				"\r\n" +
				"  <xs:element name=\"html\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:sequence>\r\n" +
				"        <xs:element ref=\"head\"/>\r\n" +
				"        <xs:element ref=\"body\"/>\r\n" +
				"      </xs:sequence>\r\n" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +
				"\r\n" +
				"  <xs:element name=\"head\" type=\"xs:string\"/>\r\n" +
				"  <xs:element name=\"body\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:sequence>\r\n" +
				"        <xs:group ref=\"block\"/>\r\n" +
				"        <xs:element name=\"form\"/>\r\n" +
				"      </xs:sequence>\r\n" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +
				"\r\n" +
				"\r\n" +
				"  <xs:group name=\"block\">\r\n" +
				"    <xs:choice>\r\n" +
				"      <xs:element ref=\"p\"/>\r\n" +
				"      <xs:group ref=\"heading\"/>\r\n" +
				"    </xs:choice>\r\n" +
				"  </xs:group>\r\n" +
				"\r\n" +
				"  <xs:element name=\"p\">\r\n" +
				"    <xs:complexType>\r\n" +
				"      <xs:attribute name=\"id\"/>" +
				"    </xs:complexType>\r\n" +
				"  </xs:element>\r\n" +				
				"\r\n" +
				"  <xs:group name=\"heading\">\r\n" +
				"    <xs:choice>\r\n" +
				"      <xs:element name=\"test\"/>\r\n" +
				"      <xs:element name=\"id\"/>\r\n" +
				"    </xs:choice>\r\n" +
				"  </xs:group>\r\n" +
				"\r\n" +
				"</xs:schema>";
		}
	}
}
