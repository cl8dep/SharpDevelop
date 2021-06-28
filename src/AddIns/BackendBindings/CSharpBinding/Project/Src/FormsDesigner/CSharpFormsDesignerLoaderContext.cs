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
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	class CSharpFormsDesignerLoaderContext : ICSharpDesignerLoaderContext
	{
		readonly FormsDesignerViewContent viewContent;

		public CSharpFormsDesignerLoaderContext(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public IDocument PrimaryFileDocument {
			get {
				return viewContent.PrimaryFileDocument;
			}
		}
		
		public IDocument DesignerCodeFileDocument {
			get {
				return viewContent.DesignerCodeFileDocument;
			}
		}
		
		public CSharpFullParseInformation GetPrimaryFileParseInformation()
		{
			return SD.ParserService.Parse(viewContent.PrimaryFileName, viewContent.PrimaryFileDocument)
				as CSharpFullParseInformation;
		}
		
		public ICompilation GetCompilation()
		{
			return SD.ParserService.GetCompilationForFile(viewContent.PrimaryFileName);
		}
		
		public IDocument GetDocument(FileName fileName)
		{
			foreach (var pair in viewContent.SourceFiles) {
				if (pair.Key.FileName == fileName)
					return pair.Value;
			}
			throw new InvalidOperationException("Designer file not found");
		}
		
		public void ShowSourceCode(int lineNumber = 0)
		{
			viewContent.ShowSourceCode(lineNumber);
		}
	}
}
