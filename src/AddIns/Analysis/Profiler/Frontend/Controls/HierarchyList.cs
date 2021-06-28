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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// A list implementation that flattens a tree.
	/// Because WPF collection change notifications work only one-by-one, and the collection
	/// implementation must represent all intermediate states, we're using a normal
	/// ObservableCollection.
	/// However, this makes expanding/collapsing O(N*M). N=total list size, M=change size
	/// We need to optimize this later.
	/// It might be possible to go back to the old implementation and use a hack to "blend out"/"add in"
	/// some items to represent the intermediate states; or we could use a tree-based
	/// representation to get O(M*lg N).
	/// Note: If we could make proper use of the ListView's virtualization and send range changes,
	/// we could be much faster. Maybe we should consider not using detailed change notifications
	/// (just "NotifyCollectionChangedAction.Reset") and work around the problems caused in the list view.
	/// </summary>
	public class HierarchyList<T> : ObservableCollection<T> where T : class, IViewModel<T>
	{
		ReadOnlyCollection<T> roots;
		
		public HierarchyList(IList<T> roots)
			: base(roots)
		{
			this.roots = new ReadOnlyCollection<T>(roots);
			
			foreach (T item in roots) {
				item.VisibleChildExpandedChanged += (sender, e) => this.NotifyNodeExpandedChanged(e.Node);
			}
		}
		
		public void NotifyNodeExpandedChanged(T changedNode)
		{
			Debug.WriteLine("HierarchyList: Start NotifyNodeExpandedChanged " + changedNode);
			// find index and add 1 to it because we add/remove after the expanded/collapsed node
			int index = IndexOf(changedNode) + 1;
			if (index <= 0)
				throw new ArgumentException("Could not find changed node");
			// The list of added/removed nodes are all children that are visible
			// from changedNode.
			// WPF does not support adding/removing multiple collection elements at once.
			// We'll have to go through the list one-by-one.
			if (changedNode.IsExpanded) {
				foreach (T node in GetVisibleChildren(changedNode.Children)) {
					base.Insert(index++, node);
				}
			} else {
				int visibleChildren = changedNode.Children.Sum(c => c.VisibleElementCount);
				for (int i = visibleChildren - 1; i >= 0; i--) {
					base.RemoveAt(index + i);
				}
			}
			Debug.WriteLine("HierarchyList: End NotifyNodeExpandedChanged");
		}
		
		public static IEnumerable<T> GetVisibleChildren(IEnumerable<T> roots)
		{
			var stack = new Stack<IEnumerator<T>>();
			try {
				stack.Push(roots.GetEnumerator());
				while (stack.Count > 0) {
					IEnumerator<T> e = stack.Peek();
					if (e.MoveNext()) {
						yield return e.Current;
						if (e.Current.IsExpanded)
							stack.Push(e.Current.Children.GetEnumerator());
					} else {
						e.Dispose();
						stack.Pop();
					}
				}
			} finally {
				while (stack.Count > 0)
					stack.Pop().Dispose();
			}
		}
		
		public ReadOnlyCollection<T> Roots
		{
			get { return roots; }
		}
	}
}
