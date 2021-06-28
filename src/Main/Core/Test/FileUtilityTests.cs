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
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class FileUtilityTests
	{
		#region NormalizePath
		[Test]
		public void NormalizePath()
		{
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\project\..\test.txt"));
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\project\.\..\test.txt"));
			Assert.AreEqual(@"c:\temp\test.txt", FileUtility.NormalizePath(@"c:\temp\\test.txt")); // normalize double backslash
			Assert.AreEqual(@"c:\temp", FileUtility.NormalizePath(@"c:\temp\."));
			Assert.AreEqual(@"c:\temp", FileUtility.NormalizePath(@"c:\temp\subdir\.."));
		}
		
		[Test]
		public void NormalizePath_DriveRoot()
		{
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:\"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/."));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/.."));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/./"));
			Assert.AreEqual(@"C:\", FileUtility.NormalizePath(@"C:/..\"));
		}
		
		[Test]
		public void NormalizePath_UNC()
		{
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"\\server\share"));
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"\\server\share\"));
			Assert.AreEqual(@"\\server\share", FileUtility.NormalizePath(@"//server/share/"));
			Assert.AreEqual(@"\\server\share\otherdir", FileUtility.NormalizePath(@"//server/share/dir/..\otherdir"));
		}
		
		[Test]
		public void NormalizePath_Web()
		{
			Assert.AreEqual(@"http://danielgrunwald.de/path/", FileUtility.NormalizePath(@"http://danielgrunwald.de/path/"));
			Assert.AreEqual(@"browser://http://danielgrunwald.de/path/", FileUtility.NormalizePath(@"browser://http://danielgrunwald.de/wrongpath/../path/"));
		}
		
		[Test]
		public void NormalizePath_Relative()
		{
			Assert.AreEqual(@"..\b", FileUtility.NormalizePath(@"..\a\..\b"));
			Assert.AreEqual(@".", FileUtility.NormalizePath(@"."));
			Assert.AreEqual(@".", FileUtility.NormalizePath(@"a\.."));
		}
		#endregion
		
		[Test]
		public void TestIsBaseDirectory()
		{
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A\b\hello"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a\"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a\"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\A", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\a\x\fWufhweoe", @"C:\a\x\fwuFHweoe\a\b\hello"));
			
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\b\..\A", @"C:\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\HELLO\..\B\..\a", @"C:\b\..\a"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\.\B\..\.\.\a", @"C:\.\.\.\.\.\.\.\a"));
			
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\b", @"C:\a\b\hello"));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\a\b\hello", @"C:\b"));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\a\x\fwufhweoe", @"C:\a\x\fwuFHweoex\a\b\hello"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\", @"C:\"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@"C:\", @"C:\a\b\hello"));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@"C:\", @"D:\a\b\hello"));
			
			Assert.IsTrue(FileUtility.IsBaseDirectory(@".", @"a\b"));
			Assert.IsTrue(FileUtility.IsBaseDirectory(@".", @"a"));
//			Assert.IsFalse(FileUtility.IsBaseDirectory(@".", @"..\a"));
//			Assert.IsFalse(FileUtility.IsBaseDirectory(@".", @".."));
			Assert.IsFalse(FileUtility.IsBaseDirectory(@".", @"c:\"));
		}
		
		[Test]
		public void TestGetAbsolutePath()
		{
			Assert.AreEqual(@"C:\a\blub", FileUtility.GetAbsolutePath(@"C:\hello\.\..\a", @".\blub"));
			Assert.AreEqual(@"C:\a\blub", FileUtility.GetAbsolutePath(@"C:\hello\", @"..\a\blub"));
		}
		
		[Test]
		public void TestGetRelativePath()
		{
			Assert.AreEqual(@"blub", FileUtility.GetRelativePath(@"C:\hello\.\..\a", @"C:\.\a\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\hello", @"C:\.\blub\.\..\.\a\.\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\hello\", @"C:\.\blub\.\..\.\a\.\blub"));
			Assert.AreEqual(@".", FileUtility.GetRelativePath(@"C:\hello", @"C:\.\hello"));
			Assert.AreEqual(@".", FileUtility.GetRelativePath(@"C:\", @"C:\"));
			Assert.AreEqual(@"D:\", FileUtility.GetRelativePath(@"C:\", @"D:\"));
			Assert.AreEqual(@"D:\def", FileUtility.GetRelativePath(@"C:\abc", @"D:\def"));
			
			// casing troubles
			Assert.AreEqual(@"blub", FileUtility.GetRelativePath(@"C:\hello\.\..\A", @"C:\.\a\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\HELlo", @"C:\.\blub\.\..\.\a\.\blub"));
			Assert.AreEqual(@"..\a\blub", FileUtility.GetRelativePath(@"C:\.\.\.\.\heLLo\A\..", @"C:\.\blub\.\..\.\a\.\blub"));
			
			// Project filename could be an URL
			Assert.AreEqual("http://example.com/vdir/", FileUtility.GetRelativePath("C:\\temp", "http://example.com/vdir/"));
		}
		
		[Test]
		public void RelativeGetRelativePath()
		{
			// Relative path
			Assert.AreEqual(@"a", FileUtility.GetRelativePath(@".", @"a"));
			Assert.AreEqual(@"..", FileUtility.GetRelativePath(@"a", @"."));
			Assert.AreEqual(@"..\b", FileUtility.GetRelativePath(@"a", @"b"));
//			Assert.AreEqual(@"..\..", FileUtility.GetRelativePath(@"a", @".."));
			
			// Getting a path from an absolute path to a relative path isn't really possible;
			// so we just keep the existing relative path (don't introduce incorrect '..\').
			Assert.AreEqual(@"def", FileUtility.GetRelativePath(@"C:\abc", @"def"));
		}
		
		[Test]
		public void TestIsEqualFile()
		{
			Assert.IsTrue(FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO WOrld.exe"));
			Assert.IsTrue(FileUtility.IsEqualFileName(@"C:\bla\..\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\a\MY.FILE.IS.THIS"));
			
			Assert.IsFalse(FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO_WOrld.exe"));
			Assert.IsFalse(FileUtility.IsEqualFileName(@"C:\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\b\MY.FILE.IS.THIS"));
			
		}
		
		[Test]
		public void TestRenameBaseDirectory()
		{
			Assert.AreEqual(@"C:\x\y\z\c\hello.txt", FileUtility.RenameBaseDirectory(@"C:\a\b\c\hello.txt", @"C:\hello\..\A\.\B\.", @"C:\.\x\y\z\."));
			Assert.AreEqual(@"C:\a\b\c\hello.txt",   FileUtility.RenameBaseDirectory(@"C:\a\b\c\hello.txt", @"C:\hello\..\A\.\B\.\FF", @"C:\.\x\y\z\."));
			Assert.AreEqual(@"C:\A\hello.txt", FileUtility.RenameBaseDirectory(@"C:\B\hello.txt", @"C:\B\", @"C:\A\"));
			Assert.AreEqual(@"C:\A\MyDir", FileUtility.RenameBaseDirectory(@"C:\B\OldDir", @"C:\B\OldDir", @"C:\A\MyDir"));
		}
	}
}
