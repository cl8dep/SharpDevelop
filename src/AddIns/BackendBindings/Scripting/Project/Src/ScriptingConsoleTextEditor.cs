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
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleTextEditor : IScriptingConsoleTextEditor
	{
		TextEditor textEditor;
		BeginReadOnlySectionProvider readOnlyRegion;
		CompletionWindow completionWindow;
		double? characterWidth;
		
		public ScriptingConsoleTextEditor(TextEditor textEditor)
		{
			this.textEditor = textEditor;
			readOnlyRegion = new BeginReadOnlySectionProvider();
			textEditor.TextArea.ReadOnlySectionProvider = readOnlyRegion;
			textEditor.PreviewKeyDown += OnTextEditorPreviewKeyDown;
		}
		
		void OnTextEditorPreviewKeyDown(object source, KeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, new ScriptingConsoleTextEditorKeyEventArgs(e));
			}
		}
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public void Dispose()
		{
			textEditor.PreviewKeyDown -= OnTextEditorPreviewKeyDown;
		}
		
		public void Append(string text)
		{
			textEditor.AppendText(text);
		}
		
		public int Column {
			get { return textEditor.TextArea.Caret.Column - 1; }
			set { textEditor.TextArea.Caret.Column = value + 1; }
		}
		
		public int SelectionStart {
			get { return textEditor.SelectionStart; }
		}
		
		public int SelectionLength {
			get { return textEditor.SelectionLength; }
		}

		public int Line {
			get { return textEditor.TextArea.Caret.Line - 1; }
			set { textEditor.TextArea.Caret.Line = value + 1; }
		}

		/// <summary>
		/// Gets the total number of lines in the text editor.
		/// </summary>
		public int TotalLines {
			get { return textEditor.Document.LineCount; }
		}

		/// <summary>
		/// Gets the text for the specified line.
		/// </summary>
		public string GetLine(int index)
		{
			DocumentLine line = textEditor.Document.GetLineByNumber(index + 1);
			return textEditor.Document.GetText(line);
		}
		
		/// <summary>
		/// Replaces the text at the specified index on the current line with the specified text.
		/// </summary>
		public void Replace(int index, int length, string text)
		{
			DocumentLine line = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
			int offset = line.Offset + index;
			textEditor.Document.Replace(offset, length, text);
		}
				
		/// <summary>
		/// Makes the current text read only. Text can still be entered at the end.
		/// </summary>
		public void MakeCurrentContentReadOnly()
		{
			readOnlyRegion.EndOffset = textEditor.Document.TextLength;
		}
		
		public void ShowCompletionWindow(ScriptingConsoleCompletionDataProvider completionDataProvider)
		{
			ICompletionData[] items = completionDataProvider.GenerateCompletionData(this);
			if (items.Length > 0) {
				ShowCompletionWindow(items);
			}
		}
		
		void ShowCompletionWindow(ICompletionData[] items)
		{
			completionWindow = new CompletionWindow(textEditor.TextArea);
			completionWindow.Closed += CompletionWindowClosed;
			foreach (ICompletionData item in items) {
				completionWindow.CompletionList.CompletionData.Add(item);
			}
			completionWindow.ExpectInsertionBeforeStart = true;
			completionWindow.Show();
		}
		
		void ShowCompletionWindow(CompletionWindow window)
		{
			if (completionWindow == window) {
				window.Show();
			}
		}
		
		public bool IsCompletionWindowDisplayed {
			get { return completionWindow != null; }
		}	
		
		void CompletionWindowClosed(object source, EventArgs e)
		{
			if (completionWindow != null) {
				completionWindow.Closed -= CompletionWindowClosed;
				completionWindow = null;
			}
		}
		
		public void ScrollToEnd()
		{
			textEditor.ScrollToEnd();
		}
		
		public int GetMaximumVisibleColumns()
		{
			return (int)((textEditor.ViewportWidth + CharacterWidth - 1) / CharacterWidth);
		}
		
		double CharacterWidth {
			get {
				if (!characterWidth.HasValue) {
					GetCharacterWidth();
				}
				return characterWidth.Value;
			}
		}
		
		void GetCharacterWidth()
		{
			FormattedText formattedText = CreateFormattedTextForSingleCharacter();
			characterWidth = formattedText.Width;
		}
		
		FormattedText CreateFormattedTextForSingleCharacter()
		{
			return new FormattedText(
				"W",
				CultureInfo.InvariantCulture, 
				textEditor.FlowDirection, 
				CreateTextEditorTypeFace(),
				textEditor.FontSize,
				textEditor.Foreground);
		}
		
		Typeface CreateTextEditorTypeFace()
		{
			return new Typeface(
				textEditor.FontFamily,
				textEditor.FontStyle,
				textEditor.FontWeight,
				textEditor.FontStretch);
		}
		
		public void Clear()
		{
			textEditor.Clear();
		}
	}
}
