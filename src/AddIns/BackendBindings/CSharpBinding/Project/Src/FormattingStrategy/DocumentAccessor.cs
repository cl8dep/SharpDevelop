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
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// Adapter IDocumentAccessor -> IDocument
	/// </summary>
	public sealed class DocumentAccessor : IDocumentAccessor
	{
		readonly IDocument doc;
		readonly int minLine;
		readonly int maxLine;
		
		/// <summary>
		/// Creates a new DocumentAccessor.
		/// </summary>
		public DocumentAccessor(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			doc = document;
			this.minLine = 1;
			this.maxLine = doc.LineCount;
		}
		
		/// <summary>
		/// Creates a new DocumentAccessor that indents only a part of the document.
		/// </summary>
		public DocumentAccessor(IDocument document, int minLine, int maxLine)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			doc = document;
			this.minLine = minLine;
			this.maxLine = maxLine;
		}
		
		int num;
		string text;
		IDocumentLine line;
		
		/// <inheritdoc/>
		public bool IsReadOnly {
			get {
				return num < minLine;
			}
		}
		
		/// <inheritdoc/>
		public int LineNumber {
			get {
				return num;
			}
		}
		
		bool lineDirty;
		
		/// <inheritdoc/>
		public string Text {
			get { return text; }
			set {
				if (num < minLine) return;
				text = value;
				lineDirty = true;
			}
		}
		
		/// <inheritdoc/>
		public bool MoveNext()
		{
			if (lineDirty) {
				DocumentUtilities.SmartReplaceLine(doc, line, text);
				lineDirty = false;
			}
			++num;
			if (num > maxLine) return false;
			line = doc.GetLineByNumber(num);
			text = doc.GetText(line);
			return true;
		}
	}
}
