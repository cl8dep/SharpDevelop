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
using Microsoft.Win32;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Description of Registrar.
	/// </summary>
	static class Registrar
	{
		/// <summary>
		/// Registers a COM component in the Windows Registry.
		/// </summary>
		/// <param name="guid">the GUID of the Component.</param>
		/// <param name="libraryId">The ID-string of the library.</param>
		/// <param name="classId">The ID-string of the class.</param>
		/// <param name="path">The path the to the DLL file containing the class. If the file does not exist FileNotFoundException is thrown.</param>
		/// <param name="is64Bit">Defines whether to register the component as 32- or 64-bit component.</param>
		/// <returns>true, if the component could be registered correctly, otherwise false.</returns>
		public static bool Register(string guid, string libraryId, string classId, string path, bool is64Bit)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (classId == null)
				throw new ArgumentNullException("classId");
			if (libraryId == null)
				throw new ArgumentNullException("libraryId");
			if (guid == null)
				throw new ArgumentNullException("guid");
			
			if (!File.Exists(path))
				throw new FileNotFoundException("DLL not found!");
			
			return Register(is64Bit ? RegistryView.Registry64 : RegistryView.Registry32, guid, libraryId, classId, path);
		}

		static bool Register(RegistryView view, string guid, string libraryId, string classId, string path)
		{
			try {
				CreateKeys(RegistryHive.LocalMachine, view, guid, libraryId, classId, path);
			} catch (UnauthorizedAccessException) {
				try {
					CreateKeys(RegistryHive.CurrentUser, view, guid, libraryId, classId, path);
				} catch (UnauthorizedAccessException) {
					return false;
				}
			}
			
			return true;
		}
		
		static void CreateKeys(RegistryHive hive, RegistryView view, string guid, string libraryId, string classId, string path)
		{
			using (RegistryKey key = RegistryKey.OpenBaseKey(hive, view)) {
				using (RegistryKey subKey = key.CreateSubKey("Software\\Classes\\CLSID\\" + guid))
					subKey.SetValue(null, classId + " Class");
				using (RegistryKey subKey = key.CreateSubKey("Software\\Classes\\CLSID\\" + guid + "\\InProcServer32"))
					subKey.SetValue(null, path);
				using (RegistryKey subKey = key.CreateSubKey("Software\\Classes\\CLSID\\" + guid + "\\ProgId"))
					subKey.SetValue(null, libraryId + "." + classId);
			}
		}
		
		static void DeleteKey(RegistryHive hive, RegistryView view, string guid)
		{
			using (RegistryKey key = RegistryKey.OpenBaseKey(hive, view)) {
				key.DeleteSubKey("Software\\Classes\\CLSID\\" + guid + "\\ProgId", false);
				key.DeleteSubKey("Software\\Classes\\CLSID\\" + guid + "\\InProcServer32", false);
				key.DeleteSubKey("Software\\Classes\\CLSID\\" + guid, false);
			}
		}
		
		/// <summary>
		/// Removes the registration of a COM component from the Windows Registry.
		/// </summary>
		/// <param name="guid">The GUID of the component to remove.</param>
		/// <param name="is64Bit">Defines whether to remove the component from 32- or 64-bit registry.</param>
		/// <returns>true, if the component could be removed from the registry correctly, otherwise false.</returns>
		public static bool Deregister(string guid, bool is64Bit)
		{
			if (guid == null)
				throw new ArgumentNullException("guid");
			
			RegistryView view = is64Bit ? RegistryView.Registry64 : RegistryView.Registry32;
			try {
				DeleteKey(RegistryHive.LocalMachine, view, guid);
				DeleteKey(RegistryHive.CurrentUser, view, guid);
			} catch (UnauthorizedAccessException) {
				try {
					DeleteKey(RegistryHive.CurrentUser, view, guid);
				} catch (UnauthorizedAccessException) {
					return false;
				}
			}
			
			return true;
		}
	}
}
