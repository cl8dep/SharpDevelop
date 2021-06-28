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
using System.Drawing;
using System.Drawing.Printing;
namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of GlobalValues.
	/// </summary>
	public static class GlobalValues
	{
		public static string ReportExtension {get {return ".srd";}}
		
		public static string DefaultReportName {get { return "Report1";}}
		
		public static Size DefaultPageSize {get {return new Size(827,1169);}}
		
		public static string PlainFileName
		{
			get {return DefaultReportName + ReportExtension;}
		}
		
		
		public static Font DefaultFont
		{
			get {return new Font("Microsoft Sans Serif",
				               10,
				               FontStyle.Regular,
				               GraphicsUnit.Point);
			}
		}
		
		
		public static Size PreferedSize {get {return new Size(100,20);}}
		
		public static Margins DefaultPageMargin {get {return new Margins(50,50,50,50);}}
		
		public static int DefaultSectionHeight {get {return 60;}}
	}
}
