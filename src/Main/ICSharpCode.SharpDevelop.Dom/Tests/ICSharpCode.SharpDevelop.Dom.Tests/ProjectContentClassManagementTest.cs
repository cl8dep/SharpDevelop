// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections;
using System.IO;

using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	/// <summary>
	/// Tests updates to the class and namespace list.
	/// </summary>
	[TestFixture]
	public class ProjectContentClassManagementTest
	{
		DefaultProjectContent pc;

		[SetUp]
		public void SetUp()
		{
			pc = new DefaultProjectContent();
		}
		
		ICompilationUnit ParseCSharp(ICompilationUnit oldUnit, string fileContent)
		{
			ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(ICSharpCode.NRefactory.SupportedLanguage.CSharp, new StringReader(fileContent));
			p.ParseMethodBodies = false;
			p.Parse();
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(pc, ICSharpCode.NRefactory.SupportedLanguage.CSharp);
			visitor.VisitCompilationUnit(p.CompilationUnit, null);
			Assert.AreEqual(0, p.Errors.Count, String.Format("Parse error preparing compilation unit: {0}", p.Errors.ErrorOutput));
			visitor.Cu.ErrorsDuringCompile = p.Errors.Count > 0;
			pc.UpdateCompilationUnit(oldUnit, visitor.Cu, null);
			
			return visitor.Cu;
		}
		
		[Test]
		public void TestEmptyProjectContent()
		{
			Assert.AreEqual(0, pc.Classes.Count);
			Assert.AreEqual(0, pc.NamespaceNames.Count);
		}
		
		void AssertSequenceSame(IEnumerable expected, IEnumerable actual)
		{
			// we cannot use NUnit's AreEqual because that uses value comparison for the elements
			ArrayList list1 = new ArrayList();
			foreach (object o in expected) list1.Add(o);
			ArrayList list2 = new ArrayList();
			foreach (object o in actual) list2.Add(o);
			if (list1.Count != list2.Count)
				Assert.AreEqual(list1, list2); // NUnit AreEqual gives nice error message
			for (int i = 0; i < list1.Count; i++) {
				Assert.AreSame(list1[i], list2[i], "At index " + i);
			}
		}
		
		[Test]
		public void InsertClass()
		{
			ICompilationUnit cu = ParseCSharp(null, "class TopLevelClass {}");
			Assert.AreEqual(1, pc.Classes.Count);
			Assert.AreEqual(1, pc.NamespaceNames.Count);
			AssertSequenceSame(cu.Classes, pc.Classes);
			AssertSequenceSame(cu.Classes, pc.GetNamespaceContents(""));
		}
		
		[Test]
		public void ReplaceClass()
		{
			ICompilationUnit cu = ParseCSharp(null, "class TopLevelClass {}");
			cu = ParseCSharp(cu, "class TopLevelClass { public int NewMember; }");
			Assert.AreEqual(1, pc.Classes.Count);
			Assert.AreEqual(1, pc.NamespaceNames.Count);
			AssertSequenceSame(cu.Classes, pc.Classes);
			AssertSequenceSame(cu.Classes, pc.GetNamespaceContents(""));
		}
		
		[Test]
		public void ReplacePartOfPartialClass()
		{
			ICompilationUnit part1 = ParseCSharp(null, "partial class PartialClass { public int Part1; }");
			ICompilationUnit part2 = ParseCSharp(null, "partial class PartialClass { public int Part2; }");
			part1 = ParseCSharp(part1, "partial class PartialClass { public string Part1; }");
			Assert.AreEqual(1, pc.Classes.Count);
			Assert.AreEqual(1, pc.NamespaceNames.Count);
			CompoundClass c = (CompoundClass)pc.GetClass("PartialClass", 0);
			Assert.IsNotNull(c);
			AssertSequenceSame(new[] { c }, pc.Classes);
			AssertSequenceSame(new[] { c }, pc.GetNamespaceContents(""));
		}
	}
}
