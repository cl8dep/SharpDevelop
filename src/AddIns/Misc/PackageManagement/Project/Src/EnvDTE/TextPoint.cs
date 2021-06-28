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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextPoint : MarshalByRefObject, global::EnvDTE.TextPoint
	{
		protected readonly string fileName;
		protected readonly TextLocation location;
		protected readonly IDocumentLoader documentLoader;
		
		internal TextPoint(string fileName, TextLocation location, IDocumentLoader documentLoader)
		{
			this.fileName = fileName;
			this.location = location;
			this.documentLoader = documentLoader;
		}
		
		public int LineCharOffset {
			get { return location.Column; }
		}
		
		public int Line {
			get { return location.Line; }
		}
		
		public global::EnvDTE.EditPoint CreateEditPoint()
		{
			return new EditPoint(fileName, location, documentLoader);
		}
		
		internal static TextPoint CreateStartPoint(CodeModelContext context, DomRegion region)
		{
			return new TextPoint(region.FileName, region.Begin, context.DocumentLoader);
		}
		
		internal static TextPoint CreateEndPoint(CodeModelContext context, DomRegion region)
		{
			return new TextPoint(region.FileName, region.End, context.DocumentLoader);
		}
	}
}
