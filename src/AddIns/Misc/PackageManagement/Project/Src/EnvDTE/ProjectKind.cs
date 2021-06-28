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
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectKind
	{
		public ProjectKind(Project project)
		{
			this.Kind = GetProjectKind(project);
		}
		
		string GetProjectKind(Project project)
		{
			string type = ProjectType.GetProjectType(project);
			if (type == ProjectType.CSharp) {
				return GetProjectKind(SD.ProjectTypeGuids.CSharp);
			} else if (type == ProjectType.VB) {
				return GetProjectKind(SD.ProjectTypeGuids.VB);
			}
			return String.Empty;
		}
		
		string GetProjectKind(Guid guid)
		{
			return "{" + guid.ToString().ToUpperInvariant() + "}";
		}
		
		public string Kind { get; private set; }
	}
}
