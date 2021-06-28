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
using System.IO;

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class TwoRegisteredSchemasTestFixture
	{
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		string addinSchemaFileName;
		string testSchemaFileName;
		int userSchemaRemovedEventFiredCount;
		
		[SetUp]
		public void Init()
		{
			string sharpDevelopSchemaFolder = @"c:\sharpdevelop\schemas";
			string[] schemaFolders = new string[] { @"c:\sharpdevelop\schemas" };
			string userDefinedSchemaFolder = @"c:\users\user\schemas";
			
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(schemaFolders, userDefinedSchemaFolder, fileSystem, factory);
			
			fileSystem.AddExistingFolders(schemaFolders);
			fileSystem.AddExistingFolder(userDefinedSchemaFolder);
			
			addinSchemaFileName = Path.Combine(sharpDevelopSchemaFolder, "addin.xsd");
			string[] sharpDevelopSchemaFiles = new string[] { addinSchemaFileName };
			fileSystem.AddDirectoryFiles(sharpDevelopSchemaFolder, sharpDevelopSchemaFiles);
			
			testSchemaFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			string[] userDefinedSchemaFiles = new string[] { testSchemaFileName };
			fileSystem.AddDirectoryFiles(userDefinedSchemaFolder, userDefinedSchemaFiles);
			
			factory.AddSchemaXml(addinSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://addin' />");
			factory.AddSchemaXml(testSchemaFileName,
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />");
			
			fileSystem.AddExistingFile(addinSchemaFileName, true);
			fileSystem.AddExistingFile(testSchemaFileName, false);
			
			registeredXmlSchemas.ReadSchemas();
			
			userSchemaRemovedEventFiredCount = 0;
			registeredXmlSchemas.UserDefinedSchemaRemoved += UserSchemaRemoved;
		}
		
		[Test]
		public void TwoSchemasRegistered()
		{
			Assert.AreEqual(2, registeredXmlSchemas.Schemas.Count);
		}
		
		[Test]
		public void SharpDevelopSchemaIsReadOnly()
		{
			Assert.IsTrue(registeredXmlSchemas.Schemas["http://addin"].IsReadOnly);
		}
		
		[Test]
		public void UserDefinedSchemaIsNotReadOnly()
		{
			Assert.IsFalse(registeredXmlSchemas.Schemas["http://test"].IsReadOnly);
		}
		
		[Test]
		public void BaseUriUsedWhenCreatingXmlSchemaCompletionData()
		{
			string baseUri = XmlSchemaCompletion.GetUri(addinSchemaFileName);
			Assert.AreEqual(baseUri, factory.GetBaseUri(addinSchemaFileName));
		}
		
		[Test]
		public void RemoveAddInSchemaRemovesSchemaFromSchemaCollection()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://addin");
			Assert.IsFalse(registeredXmlSchemas.SchemaExists("http://addin"));
		}
		
		[Test]
		public void RemoveUnknownSchemaDoesNothing()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("Unknown-schema-namespace");
		}
		
		[Test]
		public void RemoveAddInSchemaCausesUserDefinedSchemaFileToBeDeleted()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://addin");
			string[] expectedDeletedFiles = new string[] { addinSchemaFileName };

			Assert.AreEqual(expectedDeletedFiles, fileSystem.DeletedFiles);
		}
		
		[Test]
		public void RemoveAddInSchemaChecksIfUserDefinedSchemaFileExists()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://addin");
			string[] expectedFiles = new string[] { addinSchemaFileName };

			Assert.AreEqual(expectedFiles, fileSystem.FilesCheckedThatTheyExist);
		}
		
		[Test]
		public void RemoveTestSchemaDoesNotDeleteNonExistentFile()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://test");
			Assert.AreEqual(0, fileSystem.DeletedFiles.Length);
		}
		
		[Test]
		public void RemoveTestSchemaRemovesSchemaFromSchemaCollection()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://test");
			Assert.IsFalse(registeredXmlSchemas.SchemaExists("http://test"));
		}
		
		[Test]
		public void RemoveTestSchemaFiresUserSchemaRemovedEvent()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://test");
			Assert.AreEqual(1, userSchemaRemovedEventFiredCount);
		}
		
		[Test]
		public void TrytoRemoveTestSchemaTwiceButSchemaRemovedEventShouldOnlyFireOnce()
		{
			registeredXmlSchemas.RemoveUserDefinedSchema("http://test");
			registeredXmlSchemas.RemoveUserDefinedSchema("http://test");
			Assert.AreEqual(1, userSchemaRemovedEventFiredCount);
		}
		
		void UserSchemaRemoved(object source, EventArgs e)
		{
			userSchemaRemovedEventFiredCount++;
		}
	}
}
