// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class RegisteredSchemaWithEmptyNamespaceTestFixture
	{
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string testSchemaFileName;

		[SetUp]
		public void Init()
		{
			string userDefinedSchemaFolder = @"c:\users\user\schemas";
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], userDefinedSchemaFolder, fileSystem, factory);
			
			fileSystem.AddExistingFolder(userDefinedSchemaFolder);
						
			testSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			string[] userDefinedSchemaFiles = new string[] { testSchemaFileName };
			fileSystem.AddDirectoryFiles(userDefinedSchemaFolder, userDefinedSchemaFiles);
			
			factory.AddSchemaXml(testSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' />");
			
			fileSystem.AddExistingFile(testSchemaFileName, false);
			
			registeredXmlSchemas.ReadSchemas();
		}
		
		[Test]
		public void SchemaWithNoNamespaceRecordedAsError()
		{
			string message = "Ignoring schema with no namespace '" + testSchemaFileName + "'.";
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError(message);
			RegisteredXmlSchemaError[] expectedErrors = new RegisteredXmlSchemaError[] { error };
			Assert.AreEqual(expectedErrors, registeredXmlSchemas.GetSchemaErrors());
		}
	}
}
