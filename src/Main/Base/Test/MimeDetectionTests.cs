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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class MimeTypeDetectionTests
	{
		// Known BOMs
		static readonly byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF };
		static readonly byte[] UTF16BE = new byte[] { 0xFE, 0xFF };
		static readonly byte[] UTF16LE = new byte[] { 0xFF, 0xFE };
//		static readonly byte[] UTF32BE = new byte[] { 0x00, 0x00, 0xFE, 0xFF };
		static readonly byte[] UTF32LE = new byte[] { 0xFF, 0xFE, 0x00, 0x00 };
		
		[Test]
		public void TextPlain()
		{
			// always open empty files with text editor
			TestMime(new byte[] {}, "text/plain");
			// UTF-8
			TestMime(UTF8, "text/plain");
			// UTF-16 Big Endian
			TestMime(UTF16BE, "text/plain");
			// UTF-16 Little Endian
			TestMime(UTF16LE, "text/plain");
			// UTF-32 Big Endian
//			TestMime(UTF32BE, "text/plain");
			// UTF-32 Little Endian
			TestMime(UTF32LE, "text/plain");
		}
		
		[Test]
		public void TextXml()
		{
			string xml = "<?xml version=\"1.0\" ?><My File='Test' />";
			TestMime(Encoding.Default.GetBytes(xml), "text/xml");
			TestMime(UTF8.Concat(Encoding.Default.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(UTF16BE.Concat(Encoding.BigEndianUnicode.GetBytes(xml)).ToArray(), "text/xml");
			TestMime(UTF16LE.Concat(Encoding.Unicode.GetBytes(xml)).ToArray(), "text/xml");
//			TestMime(UTF32BE.Concat(new UTF32Encoding(true, true).GetBytes(xml)).ToArray(), "text/xml");
			TestMime(UTF32LE.Concat(Encoding.UTF32.GetBytes(xml)).ToArray(), "text/xml");
		}
		
		[Test]
		public void TestFiles()
		{
			TestMime(LoadFile("ICSharpCode.SharpDevelop.mime_utf-16_be_test.txt"), "text/plain");
			TestMime(LoadFile("ICSharpCode.SharpDevelop.mime_utf-16_le_test.txt"), "text/plain");
			TestMime(LoadFile("ICSharpCode.SharpDevelop.mime_bug1.txt"), "text/plain");
		}
		
		byte[] LoadFile(string resourceName)
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				return bytes;
			}
		}
		
		void TestMime(byte[] bytes, string expectedMime)
		{
			Assert.AreEqual(expectedMime, MimeTypeDetection.FindMimeType(bytes));
		}
	}
}
