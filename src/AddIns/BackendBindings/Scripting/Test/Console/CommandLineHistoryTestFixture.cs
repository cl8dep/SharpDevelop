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
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the CommandLineHistory class.
	/// </summary>
	[TestFixture]
	public class CommandLineHistoryTestFixture
	{
		CommandLineHistory history;
		
		[SetUp]
		public void Init()
		{
			history = new CommandLineHistory();
			history.Add("a");
			history.Add("b");
			history.Add("c");
		}
		
		[Test]
		public void LastCommandLineIsNull()
		{
			Assert.IsNull(history.Current);
		}
		
		[Test]
		public void MovePreviousOnce()
		{
			Assert.IsTrue(history.MovePrevious());
		}
		
		[Test]
		public void CurrentAfterMovePrevious()
		{
			history.MovePrevious();
			Assert.AreEqual("c", history.Current);
		}
		
		[Test]
		public void AddLineAfterMovePrevious()
		{
			history.MovePrevious();
			history.MovePrevious();
			history.Add("d");
			
			Assert.IsNull(history.Current);
		}
		
		[Test]
		public void EmptyLineIgnored()
		{
			history.Add(String.Empty);
			history.MovePrevious();
			Assert.AreEqual("c", history.Current);
		}
		
		/// <summary>
		/// After trying to move beyond the end of the list moving previous should not show the last
		/// item again.
		/// </summary>
		[Test]
		public void MovePreviousThenNextTwiceThenPreviousAgain()
		{
			history.MovePrevious();
			history.MoveNext();
			history.MoveNext();
			history.MovePrevious();
			Assert.AreEqual("b", history.Current);
		}
	}
}
