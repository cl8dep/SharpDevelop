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
using System.Collections.Generic;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// If there are two namespaces that start the same and match up
	/// to at least the start of the dot character then the 
	/// CodeCoverageMethod.GetChildNamespaces fails to correctly identify
	/// child namespaces.
	/// 
	/// For example:
	/// 
	/// Root.Tests
	/// RootBar
	/// 
	/// If we look for child namespaces using the string "Root" the
	/// code should only return "Tests", but it will also return
	/// "Bar" due to a bug matching only the start of the class namespace
	/// without taking into account the dot character.
	/// </summary>
	[TestFixture]
	public class SimilarRootNamespaceTestFixture
	{
		List<string> childNamespaces = new List<string>();
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CodeCoverageModule module = new CodeCoverageModule("Root.Tests");
		
			// Add two methods in namespaces that start with the
			// same initial characters.
			CodeCoverageMethod rootTestsMethod = new CodeCoverageMethod("RunThis", "Root.Tests.MyTestFixture");
			module.Methods.Add(rootTestsMethod);
			CodeCoverageMethod rootBarMethod = new CodeCoverageMethod("RunThis", "RootBar.MyTestFixture");
			module.Methods.Add(rootBarMethod);
			
			childNamespaces = CodeCoverageMethod.GetChildNamespaces(module.Methods, "Root");	
		}
		
		[Test]
		public void RootNamespaceHasOneChildNamespace()
		{
			Assert.AreEqual(1, childNamespaces.Count);
		}
		
		[Test]
		public void RootNamespaceChildNamespaceIsTests()
		{
			Assert.AreEqual("Tests", childNamespaces[0]);
		}
	}
}
