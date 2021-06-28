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
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Utils
{
	/// <summary>
	/// Checks the change events of a model collection for consistency
	/// with the collection data.
	/// </summary>
	public class ModelCollectionEventCheck<T>
	{
		readonly IModelCollection<T> modelCollection;
		List<T> list;
		
		public ModelCollectionEventCheck(IModelCollection<T> modelCollection)
		{
			if (modelCollection == null)
				throw new ArgumentNullException("modelCollection");
			this.modelCollection = modelCollection;
			this.list = new List<T>(modelCollection);
			modelCollection.CollectionChanged += OnCollectionChanged;
		}
		
		void OnCollectionChanged(IReadOnlyCollection<T> removedItems, IReadOnlyCollection<T> addedItems)
		{
			foreach (T removed in removedItems) {
				list.Remove(removed);
			}
			foreach (T added in addedItems) {
				list.Add(added);
			}
			Verify();
		}
		
		public void Verify()
		{
			Assert.AreEqual(list, modelCollection.ToList());
		}
	}
}
