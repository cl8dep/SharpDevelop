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
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcClass : IMvcClass
	{
		bool isModelClass = true;
		
		public FakeMvcClass(string fullName)
		{
			this.FullName = fullName;
		}
		
		public FakeMvcClass(string @namespace, string name)
		{
			this.Namespace = @namespace;
			this.Name = name;
		}
		
		public string FullName { get; set; }
		public string Name { get; set; }
		public string Namespace { get; set; }
		public string BaseClassFullName { get; set; }
		public string AssemblyLocation { get; set; }
		
		public void SetIsNotModelClass()
		{
			isModelClass = false;
		}
		
		public bool IsModelClass()
		{
			return isModelClass;
		}
	}
}
