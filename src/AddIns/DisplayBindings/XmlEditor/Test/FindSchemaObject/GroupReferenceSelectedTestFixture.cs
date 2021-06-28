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
using System.IO;
using System.Xml.Schema;
using XmlEditor.Tests.Schema;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.FindSchemaObject
{
	/// <summary>
	/// Tests that an xs:group/@ref is located in the schema.
	/// </summary>
	[TestFixture]
	public class GroupReferenceSelectedTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaGroup schemaGroup;
		
		public override void FixtureInit()
		{
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			schemas.Add(SchemaCompletion);
			XmlSchemaCompletion xsdSchemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			schemas.Add(xsdSchemaCompletion);
			
			string xml = GetSchema();			
			int index = xml.IndexOf("ref=\"block\"");
			index = xml.IndexOf("block", index);
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, SchemaCompletion);
			schemaGroup = (XmlSchemaGroup)schemaDefinition.GetSelectedSchemaObject(xml, index);
		}
		
		[Test]
		public void GroupName()
		{
			Assert.AreEqual("block", schemaGroup.QualifiedName.Name);
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
