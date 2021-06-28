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
	[TestFixture]
	public class ReferencedElementsTestFixture : SchemaTestFixtureBase
	{
		XmlCompletionItemCollection shipOrderAttributes;
		XmlCompletionItemCollection shipToAttributes;
		XmlElementPath shipToPath;
		XmlElementPath shipOrderPath;
		
		public override void FixtureInit()
		{
			// Get shipto attributes.
			shipToPath = new XmlElementPath();
			QualifiedName shipOrderName = new QualifiedName("shiporder", "http://www.w3schools.com");
			shipToPath.AddElement(shipOrderName);
			shipToPath.AddElement(new QualifiedName("shipto", "http://www.w3schools.com"));

			shipToAttributes = SchemaCompletion.GetAttributeCompletion(shipToPath);
			
			// Get shiporder attributes.
			shipOrderPath = new XmlElementPath();
			shipOrderPath.AddElement(shipOrderName);
			
			shipOrderAttributes = SchemaCompletion.GetAttributeCompletion(shipOrderPath);
			
		}
		
		[Test]
		public void OneShipOrderAttribute()
		{
			Assert.AreEqual(1, shipOrderAttributes.Count, "Should only have one shiporder attribute.");
		}		
		
		[Test]
		public void ShipOrderAttributeName()
		{
			Assert.IsTrue(shipOrderAttributes.Contains("id"),
			                "Incorrect shiporder attribute name.");
		}

		[Test]
		public void OneShipToAttribute()
		{
			Assert.AreEqual(1, shipToAttributes.Count, "Should only have one shipto attribute.");
		}
		
		[Test]
		public void ShipToAttributeName()
		{
			Assert.IsTrue(shipToAttributes.Contains("address"),
			                "Incorrect shipto attribute name.");
		}					
		
		[Test]
		public void ShipOrderChildElementsCount()
		{
			Assert.AreEqual(1, SchemaCompletion.GetChildElementCompletion(shipOrderPath).Count, 
			                "Should be one child element.");
		}
		
		[Test]
		public void ShipOrderHasShipToChildElement()
		{
			XmlCompletionItemCollection data = SchemaCompletion.GetChildElementCompletion(shipOrderPath);
			Assert.IsTrue(data.Contains("shipto"), 
			                "Incorrect child element name.");
		}
		
		[Test]
		public void ShipToChildElementsCount()
		{
			Assert.AreEqual(2, SchemaCompletion.GetChildElementCompletion(shipToPath).Count, 
			                "Should be 2 child elements.");
		}		
		
		[Test]
		public void ShipToHasNameChildElement()
		{
			XmlCompletionItemCollection data = SchemaCompletion.GetChildElementCompletion(shipToPath);
			Assert.IsTrue(data.Contains("name"), 
			                "Incorrect child element name.");
		}		
		
		[Test]
		public void ShipToHasAddressChildElement()
		{
			XmlCompletionItemCollection data = SchemaCompletion.GetChildElementCompletion(shipToPath);
			Assert.IsTrue(data.Contains("address"), 
			                "Incorrect child element name.");
		}		
		
		protected override string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\"  xmlns=\"http://www.w3schools.com\">\r\n" +
				"\r\n" +
				"<!-- definition of simple elements -->\r\n" +
				"<xs:element name=\"name\" type=\"xs:string\"/>\r\n" +
				"<xs:element name=\"address\" type=\"xs:string\"/>\r\n" +
				"\r\n" +
				"<!-- definition of complex elements -->\r\n" +
				"<xs:element name=\"shipto\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"name\"/>\r\n" +
				"   <xs:element ref=\"address\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"address\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"<xs:element name=\"shiporder\">\r\n" +
				" <xs:complexType>\r\n" +
				"  <xs:sequence>\r\n" +
				"   <xs:element ref=\"shipto\"/>\r\n" +
				"  </xs:sequence>\r\n" +
				"  <xs:attribute name=\"id\"/>\r\n" +
				" </xs:complexType>\r\n" +
				"</xs:element>\r\n" +
				"\r\n" +
				"</xs:schema>";
		}
	}
}
