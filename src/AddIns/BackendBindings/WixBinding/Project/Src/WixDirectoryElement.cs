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
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.WixBinding
{
	public class WixDirectoryElement : WixDirectoryElementBase
	{
		public const string DirectoryElementName = "Directory";
		public const string RootDirectoryId = "TARGETDIR";
		
		public WixDirectoryElement(WixDocument document) 
			: base(DirectoryElementName, document)
		{
		}
		
		/// <summary>
		/// Returns the last directory specified in the path 
		/// </summary>
		public static string GetLastFolderInDirectoryName(string path)
		{
			int index = path.LastIndexOf(Path.DirectorySeparatorChar);
			return path.Substring(index + 1);
		}
		
		/// <summary>
		/// Creates the directory element and sets its Id and SourceName.
		/// </summary>
		public static WixDirectoryElement CreateRootDirectory(WixDocument document)
		{
			WixDirectoryElement rootDirectory = new WixDirectoryElement(document);
			rootDirectory.Id = RootDirectoryId;
			rootDirectory.SourceName = "SourceDir";
			return rootDirectory;
		}
			
		/// <summary>
		/// Adds a new component element to this directory element.
		/// </summary>
		public WixComponentElement AddComponent(string fileName)
		{
			WixComponentElement componentElement = new WixComponentElement((WixDocument)OwnerDocument);
			componentElement.GenerateUniqueIdFromFileName(fileName);
			componentElement.GenerateNewGuid();
			AppendChild(componentElement);
			return componentElement;
		}
		
		public string SourceName {
			get { return GetAttribute("SourceName"); }
			set { SetAttribute("SourceName", value); }
		}
			
		/// <summary>
		/// Gets the directory name.
		/// </summary>
		/// <returns>
		/// Returns the directory name. If the directory Id
		/// is a special case (e.g. "ProgramFilesFolder") it returns this id slightly 
		/// modified (e.g. "Program Files").
		/// </returns>
		public string DirectoryName {
			get {
				string name = GetSystemDirectory(Id);
				if (name != null) {
					return name;
				}
				return GetAttribute("Name");
			}
			set { SetAttribute("Name", value); }
		}
		
		/// <summary>
		///	Returns whether the specified name maps to a system directory.
		/// </summary>
		public static string GetSystemDirectory(string id)
		{
			switch (id) {
				case "ProgramFilesFolder":
				case "AdminToolsFolder":
				case "AppDataFolder":
				case "CommonAppDataFolder":
				case "CommonFiles64Folder":
				case "CommonFilesFolder":
				case "DesktopFolder":
				case "FavoritesFolder":
				case "FontsFolder":
				case "LocalAppDataFolder":
				case "MyPicturesFolder":
				case "PersonalFolder":
				case "ProgramFiles64Folder":
				case "ProgramMenuFolder":
				case "SendToFolder":
				case "StartMenuFolder":
				case "StartupFolder":
				case "System16Folder":
				case "System64Folder":
				case "SystemFolder":
				case "TempFolder":
				case "TemplateFolder":
				case "WindowsVolume":
					return StringParser.Parse(String.Concat("${res:ICSharpCode.WixBinding.WixDirectoryElement.", id, "}"));
				case "WindowsFolder":
					return "Windows";
			}
			return null;
		}
	}
}
