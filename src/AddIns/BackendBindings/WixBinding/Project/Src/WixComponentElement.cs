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
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixComponentElement : WixElementBase
	{
		public const string ComponentElementName = "Component";
		
		public WixComponentElement(WixDocument document) 
			: base(ComponentElementName, document)
		{
		}
		
		public string Guid {
			get { return GetAttribute("Guid"); }
			set { SetAttribute("Guid", value); }
		}
		
		public string DiskId {
			get { return GetAttribute("DiskId"); }
			set { SetAttribute("DiskId", value); }
		}
		
		public bool HasDiskId {
			get { return HasAttribute("DiskId"); }
		}
		
		public void GenerateNewGuid()
		{
			Guid = System.Guid.NewGuid().ToString().ToUpperInvariant();
		}
		
		public WixFileElement[] GetFiles()
		{
			List<WixFileElement> files = new List<WixFileElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixFileElement childElement = childNode as WixFileElement;
				if (childElement != null) {
					files.Add(childElement);
				}
			}
			return files.ToArray();
		}
		
		/// <summary>
		/// Creates a new file element with the specified filename.
		/// </summary>
		public WixFileElement AddFile(string fileName)
		{
			WixFileElement fileElement = new WixFileElement(this, fileName);
			AppendChild(fileElement);
			return fileElement;
		}
		
		/// <remarks>
		/// Takes the filename, removes all periods, and 
		/// capitalises the first character and first extension character.
		/// </remarks>
		public void GenerateUniqueIdFromFileName(string fileName)
		{
			Id = GenerateIdFromFileName(fileName);
			if (!OwnerWixDocument.ComponentIdExists(Id)) {
				return;
			}
			
			Id = GenerateIdFromParentDirectoryAndFileName(fileName, Id);
			if (!OwnerWixDocument.ComponentIdExists(Id)) {
				return;
			}
			
			Id = GenerateUniqueIdByAppendingNumberToEnd(Id);
		}
		
		/// <summary>
		/// Creates an id from the filename.
		/// </summary>
		/// <remarks>
		/// Takes the filename, removes all periods, and 
		/// capitalises the first character and first extension character.
		/// </remarks>
		public string GenerateIdFromFileName(string fileName)
		{
			string fileNameWithoutExtension = UpperCaseFirstCharacterOfFileNameWithoutExtension(fileName);
			fileNameWithoutExtension = RemoveDotCharacters(fileNameWithoutExtension);
			
			string extension = GetFileExtensionWithoutDotCharacter(fileName);
			extension = UpperCaseFirstCharacter(extension);
			
			string modifiedFileName = String.Concat(fileNameWithoutExtension, extension);
			return WixFileElement.GenerateId(modifiedFileName);
		}
		
		string UpperCaseFirstCharacterOfFileNameWithoutExtension(string fileName)
		{
			string fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
			if (fileNameNoExtension.Length > 0) {
				return UpperCaseFirstCharacter(fileNameNoExtension);
			}
			return String.Empty;
		}
		
		string GetFileExtensionWithoutDotCharacter(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (!String.IsNullOrEmpty(extension)) {
				return extension.Substring(1);
			}
			return String.Empty;
		}
		
		string UpperCaseFirstCharacter(string s)
		{
			if (!String.IsNullOrEmpty(s)) {
				string firstCharacter = s.Substring(0, 1);
				string restOfString = s.Substring(1);
				return String.Concat(firstCharacter.ToUpperInvariant(), restOfString);
			}
			return String.Empty;
		}
		
		string GenerateIdFromParentDirectoryAndFileName(string fileName, string idGeneratedFromFileName)
		{
			string id = GenerateIdFromParentDirectory(fileName);
			return String.Concat(id, idGeneratedFromFileName);
		}
		
		string GenerateIdFromParentDirectory(string fileName)
		{
			string fullParentDirectory = Path.GetDirectoryName(fileName);
			string lastFolder = WixDirectoryElement.GetLastFolderInDirectoryName(fullParentDirectory);
			string id = UpperCaseFirstCharacter(lastFolder);
			id = WixFileElement.GenerateId(id);
			id = RemoveDotCharacters(id);
			return id;
		}
		
		string RemoveDotCharacters(string text)
		{
			return text.Replace(".", String.Empty);
		}
		
		string GenerateUniqueIdByAppendingNumberToEnd(string id)
		{
			int count = 0;
			string baseId = id;
			do {
				++count;
				id = String.Concat(baseId, count);
			} while (OwnerWixDocument.ComponentIdExists(id));
			
			return id;
		}
	}
}
