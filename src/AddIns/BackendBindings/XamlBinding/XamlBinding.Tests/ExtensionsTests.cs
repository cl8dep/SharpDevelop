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
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		public void StringReplaceTest1()
		{
			string text = "Hello World!";
			int index = 0;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest2()
		{
			string text = "My Hello World!";
			int index = 3;
			int length = 5;
			string newText = "Bye";
			
			string result = text.Replace(index, length, newText);
			string expected = "My Bye World!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest3()
		{
			string text = "Hello World!";
			int index = 6;
			int length = 5;
			string newText = "Byte";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello Byte!";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		public void StringReplaceTest4()
		{
			string text = "Hello World!";
			int index = 11;
			int length = 1;
			string newText = "?";
			
			string result = text.Replace(index, length, newText);
			string expected = "Hello World?";
			
			Assert.AreEqual(expected, result);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest1()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test />";
			editor.Caret.Offset = 6;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest2()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 6;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest3()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 14;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual(string.Empty, text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest4()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test value=\"\" />";
			editor.Caret.Offset = 11;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("value", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest5()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 12;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest6()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 13;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member.", text);
		}
		
		[Test]
		[STAThread]
		public void GetWordBeforeCaretExtendedTest7()
		{
			ITextEditor editor = new AvalonEditTextEditorAdapter(new AvalonEdit.TextEditor());
			editor.Document.Text = "<Test member.value=\"\" />";
			editor.Caret.Offset = 14;
			string text = editor.GetWordBeforeCaretExtended();
			Assert.AreEqual("member.v", text);
		}
		
		[Test]
		public void MoveBeforeTest1()
		{
			string xml = "<Test value=\"A\"><Item value=\"B\" /><Item value=\"C\" /></Test>";
			XElement parent = XElement.Parse(xml);
			XElement item = parent.Elements().ElementAt(0);
			XElement itemToMove = parent.Elements().ElementAt(1);
			itemToMove.MoveBefore(item);
			Assert.AreEqual(itemToMove, parent.Elements().First());
		}
	}
}
