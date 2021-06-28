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
	/// Tests complex content restriction elements.
	/// </summary>
	[TestFixture]
	public class RestrictionElementTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection childElements;
		XmlCompletionItemCollection attributes;
		XmlCompletionItemCollection annotationChildElements;
		XmlCompletionItemCollection choiceChildElements;
		
		public override void FixtureInit()
		{			
			XmlElementPath path = new XmlElementPath();
			path.AddElement(new QualifiedName("group", "http://www.w3.org/2001/XMLSchema"));
			childElements = SchemaCompletion.GetChildElementCompletion(path);
			attributes = SchemaCompletion.GetAttributeCompletion(path);
		
			// Get annotation child elements.
			path.AddElement(new QualifiedName("annotation", "http://www.w3.org/2001/XMLSchema"));
			annotationChildElements = SchemaCompletion.GetChildElementCompletion(path);
			
			// Get choice child elements.
			path.Elements.RemoveLast();
			path.AddElement(new QualifiedName("choice", "http://www.w3.org/2001/XMLSchema"));
			choiceChildElements = SchemaCompletion.GetChildElementCompletion(path);
		}

		[Test]
		public void GroupChildElementIsAnnotation()
		{
			Assert.IsTrue(childElements.Contains("annotation"), 
			              "Should have a child element called annotation.");
		}
		
		[Test]
		public void GroupChildElementIsChoice()
		{
			Assert.IsTrue(childElements.Contains("choice"), 
			              "Should have a child element called choice.");
		}		
		
		[Test]
		public void GroupChildElementIsSequence()
		{
			Assert.IsTrue(childElements.Contains("sequence"), 
			              "Should have a child element called sequence.");
		}		
		
		[Test]
		public void GroupAttributeIsName()
		{
			Assert.IsTrue(attributes.Contains("name"),
			              "Should have an attribute called name.");			
		}
		
		[Test]
		public void AnnotationChildElementIsAppInfo()
		{
			Assert.IsTrue(annotationChildElements.Contains("appinfo"), 
			              "Should have a child element called appinfo.");
		}	
		
		[Test]
		public void AnnotationChildElementIsDocumentation()
		{
			Assert.IsTrue(annotationChildElements.Contains("documentation"), 
			              "Should have a child element called appinfo.");
		}	
		
		[Test]
		public void ChoiceChildElementIsSequence()
		{
			Assert.IsTrue(choiceChildElements.Contains("element"), 
			              "Should have a child element called element.");
		}	
		
		protected override string GetSchema()
		{
			return "<xs:schema targetNamespace=\"http://www.w3.org/2001/XMLSchema\" blockDefault=\"#all\" elementFormDefault=\"qualified\" version=\"1.0\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xml:lang=\"EN\" xmlns:hfp=\"http://www.w3.org/2001/XMLSchema-hasFacetAndProperty\">\r\n" +
					"\r\n" +
					" <xs:element name=\"group\" type=\"xs:namedGroup\" id=\"group\">\r\n" +
					" </xs:element>\r\n" +
					"\r\n" +
					" <xs:element name=\"annotation\" id=\"annotation\">\r\n" +
					"   <xs:complexType>\r\n" +
					"      <xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n" +
					"       <xs:element name=\"appinfo\"/>\r\n" +
					"       <xs:element name=\"documentation\"/>\r\n" +
					"      </xs:choice>\r\n" +
					"      <xs:attribute name=\"id\" type=\"xs:ID\"/>\r\n" +
					"   </xs:complexType>\r\n" +
					" </xs:element>\r\n" +
					"\r\n" +
					"\r\n" +
					" <xs:complexType name=\"namedGroup\">\r\n" +
					"  <xs:complexContent>\r\n" +
					"   <xs:restriction base=\"xs:realGroup\">\r\n" +
					"    <xs:sequence>\r\n" +
					"     <xs:element ref=\"xs:annotation\" minOccurs=\"0\"/>\r\n" +
					"     <xs:choice minOccurs=\"1\" maxOccurs=\"1\">\r\n" +
					"      <xs:element ref=\"xs:choice\"/>\r\n" +
					"      <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +
					"    </xs:sequence>\r\n" +
					"    <xs:attribute name=\"name\" use=\"required\" type=\"xs:NCName\"/>\r\n" +
					"    <xs:attribute name=\"ref\" use=\"prohibited\"/>\r\n" +
					"    <xs:attribute name=\"minOccurs\" use=\"prohibited\"/>\r\n" +
					"    <xs:attribute name=\"maxOccurs\" use=\"prohibited\"/>\r\n" +
					"    <xs:anyAttribute namespace=\"##other\" processContents=\"lax\"/>\r\n" +
					"   </xs:restriction>\r\n" +
					"  </xs:complexContent>\r\n" +
					" </xs:complexType>\r\n" +
					"\r\n" +
					" <xs:complexType name=\"realGroup\">\r\n" +
					"    <xs:sequence>\r\n" +
					"     <xs:element ref=\"xs:annotation\" minOccurs=\"0\"/>\r\n" +
					"     <xs:choice minOccurs=\"0\" maxOccurs=\"1\">\r\n" +
					"      <xs:element name=\"all\"/>\r\n" +
					"      <xs:element ref=\"xs:choice\"/>\r\n" +
					"      <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +
					"    </xs:sequence>\r\n" +
					"    <xs:anyAttribute namespace=\"##other\" processContents=\"lax\"/>\r\n" +
					" </xs:complexType>\r\n" +
					"\r\n" +
					" <xs:element name=\"choice\" id=\"choice\">\r\n" +
					"   <xs:complexType>\r\n" +
					"     <xs:choice minOccurs=\"0\" maxOccurs=\"1\">\r\n" +
					"       <xs:element name=\"element\"/>\r\n" +
				    "       <xs:element name=\"sequence\"/>\r\n" +
					"     </xs:choice>\r\n" +					
					"   </xs:complexType>\r\n" +
					" </xs:element>\r\n" +
					"</xs:schema>";
		}
	}
}
