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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElementsList<T> : MarshalByRefObject, global::EnvDTE.CodeElements
		where T : global::EnvDTE.CodeElement
	{
		readonly List<T> elements = new List<T>();
		
		public int Count {
			get { return elements.Count; }
		}
		
		public IEnumerator GetEnumerator()
		{
			return elements.GetEnumerator();
		}
		
		global::EnvDTE.CodeElement global::EnvDTE.CodeElements.Item(object index)
		{
			if (index is int) {
				return GetItem((int)index);
			}
			return GetItem((string)index);
		}
		
		global::EnvDTE.CodeElement GetItem(int index)
		{
			return elements[index - 1];
		}
		
		global::EnvDTE.CodeElement GetItem(string name)
		{
			return elements.Single(item => item.Name == name);
		}
		
		internal void Add(T element)
		{
			elements.Add(element);
		}
		
		internal void AddRange(IEnumerable<T> items)
		{
			foreach (T element in items) {
				Add(element);
			}
		}
	}
}
