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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests that the TestProject is correctly updated after the inner class name changes.
	/// </summary>
	[TestFixture]
	public class InnerClassNameChangesTestFixture : NUnitTestProjectFixtureBase
	{
		NUnitTestClass originalA;
		
		public override void SetUp()
		{
			base.SetUp();
			AddCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerTest {
			[Test]
			public void M() {}
		}
	}
}
");
			
			originalA = testProject.GetTestClass(new FullTypeName("MyTests.A"));
			
			UpdateCodeFile("test.cs", @"
using NUnit.Framework;
namespace MyTests {
	class A {
		class InnerTestMod {
			[Test]
			public void M() {}
		}
	}
}
");
		}
		
		[Test]
		public void OuterClassNotChanged()
		{
			Assert.IsNotNull(originalA);
			Assert.AreSame(originalA, testProject.GetTestClass(new FullTypeName("MyTests.A")));
		}
		
		[Test]
		public void InnerClassRenamed()
		{
			Assert.AreEqual("InnerTestMod", originalA.NestedTests.Single().DisplayName);
		}
	}
}
