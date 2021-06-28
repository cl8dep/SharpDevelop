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

using Hornung.ResourceToolkit.ResourceFileContent;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Describes a reference to a resource set (this typically means a file,
	/// but a resource set reference can exist when the actual file it refers to
	/// is missing).
	/// </summary>
	public class ResourceSetReference
	{
		readonly string resourceSetName;
		readonly string fileName;
		
		/// <summary>
		/// Gets the resource set name of this reference.
		/// This typically corresponds to the manifest resource name.
		/// This property never returns <c>null</c>.
		/// </summary>
		public string ResourceSetName {
			get { return resourceSetName; }
		}
		
		/// <summary>
		/// Gets the name of the file that contains the referenced resource set.
		/// This property may return <c>null</c> if the file name cannot be
		/// determined unambiguously or if the file is missing.
		/// </summary>
		public string FileName {
			get { return fileName; }
		}
		
		/// <summary>
		/// Gets the <see cref="IResourceFileContent"/> for the referenced resource set.
		/// This property may return <c>null</c> if the file name cannot be
		/// determined unambiguously or if the file is missing.
		/// </summary>
		public IResourceFileContent ResourceFileContent {
			get {
				if (this.FileName == null) return null;
				return ResourceFileContentRegistry.GetResourceFileContent(this.FileName);
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceSetReference"/> class.
		/// </summary>
		/// <param name="resourceSetName">The resource set name of the reference.</param>
		/// <param name="fileName">The name of the file that contains the referenced resource set. May be <c>null</c>.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="resourceSetName"/> parameter is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The <paramref name="resourceSetName"/> parameter is empty.</exception>
		public ResourceSetReference(string resourceSetName, string fileName)
		{
			if (resourceSetName == null) {
				throw new ArgumentNullException("resourceSetName");
			} else if (resourceSetName.Length == 0) {
				throw new ArgumentException("The resourceSetName must not be empty.", "resourceSetName");
			}
			
			this.resourceSetName = resourceSetName;
			this.fileName = fileName;
		}
	}
}
