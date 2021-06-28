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
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.Core;

namespace Hornung.ResourceToolkit.Refactoring
{
	/// <summary>
	/// Finds references to a specific resource in a text document.
	/// </summary>
	public class SpecificResourceReferenceFinder : IResourceReferenceFinder
	{
		readonly string resourceFileName;
		readonly string key;
		
		/// <summary>
		/// Gets the name of the resource file that contains the resource to find.
		/// </summary>
		public string ResourceFileName {
			get {
				return resourceFileName;
			}
		}
		
		/// <summary>
		/// Gets the resource key to find.
		/// </summary>
		public string Key {
			get {
				return key;
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Returns the offset of the next possible resource reference in the file
		/// after prevOffset.
		/// Returns -1, if there are no more possible references.
		/// </summary>
		/// <param name="fileName">The name of the file that is currently being searched in.</param>
		/// <param name="fileContent">The text content of the file.</param>
		/// <param name="prevOffset">The offset of the last found reference or -1, if this is the first call in the current file.</param>
		public int GetNextPossibleOffset(string fileName, string fileContent, int prevOffset)
		{
			string code;
			int pos = ResourceRefactoringService.FindStringLiteral(fileName, fileContent, this.Key, prevOffset+1, out code);
			if (pos == -1) {
				// if the code generator search fails, try a direct search
				pos = fileContent.IndexOf(this.Key, prevOffset+1, StringComparison.OrdinalIgnoreCase);
			}
			return pos;
		}
		
		/// <summary>
		/// Determines whether the specified ResourceResolveResult describes
		/// a resource that should be included in the search result.
		/// </summary>
		public bool IsReferenceToResource(ResourceResolveResult result)
		{
			return FileUtility.IsEqualFileName(this.ResourceFileName, result.FileName) &&
				this.Key.Equals(result.Key, StringComparison.OrdinalIgnoreCase);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SpecificResourceReferenceFinder"/> class.
		/// </summary>
		/// <param name="resourceFileName">The name of the resource file that contains the resource to find.</param>
		/// <param name="key">The resource key to find.</param>
		public SpecificResourceReferenceFinder(string resourceFileName, string key)
		{
			if (resourceFileName == null) {
				throw new ArgumentNullException("resourceFileName");
			}
			if (key == null) {
				throw new ArgumentNullException("key");
			}
			
			this.resourceFileName = resourceFileName;
			this.key = key;
		}
	}
}
