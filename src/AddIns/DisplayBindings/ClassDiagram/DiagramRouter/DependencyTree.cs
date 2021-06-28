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

namespace Tools.Diagrams
{
	public class DependencyTree<T> where T : class, IEquatable<T>
	{
		DependencyTreeNode<T> root = new DependencyTreeNode<T>(default(T));
		
		public DependencyTree() { }
		
		public static DependencyTreeNode<T> FindNode (DependencyTreeNode<T> root, Predicate<DependencyTreeNode<T>> predicate)
		{
			if (predicate(root)) return root;
			DependencyTreeNode<T> ret = null;
			foreach (DependencyTreeNode<T> node in root.Dependants)
			{
				ret = FindNode(node, predicate);
				if (ret != null) break;
			}
			return ret;
		}
		
		public static DependencyTreeNode<T> FindNode (DependencyTreeNode<T> root, Predicate<T> predicate)
		{
			return FindNode(root, delegate (DependencyTreeNode<T> item) { return predicate (item.Item); } );
		}
		
		public static DependencyTreeNode<T> FindNode (DependencyTreeNode<T> root, T item)
		{
			if (item == null) return null;
			return FindNode(root, delegate (T nodeItem)
			                {
			                	bool ret = (item.Equals(nodeItem));
			                	return ret;
			                } );
		}
		
		public static void WalkTreeRootFirst (DependencyTreeNode<T> root, Action<DependencyTreeNode<T>> action)
		{
			action(root);
			foreach (DependencyTreeNode<T> node in root.Dependants)
				WalkTreeRootFirst(node, action);
		}
				
		public static void WalkTreeRootFirst (DependencyTreeNode<T> root, Action<T> action)
		{
			WalkTreeRootFirst (root, delegate (DependencyTreeNode<T> item) { action(item.Item); });
		}
		
		public static void WalkTreeChildrenFirst (DependencyTreeNode<T> root, Action<DependencyTreeNode<T>> action)
		{
			foreach (DependencyTreeNode<T> node in root.Dependants)
				WalkTreeChildrenFirst(node, action);
			action(root);
		}

		public static void WalkTreeChildrenFirst (DependencyTreeNode<T> root, Action<T> action)
		{
			WalkTreeChildrenFirst (root, delegate (DependencyTreeNode<T> item) { action(item.Item); });
		}

		public DependencyTreeNode<T> FindNode (Predicate<T> predicate)
		{
			return FindNode(root, predicate);
		}
		
		public DependencyTreeNode<T> FindNode (T item)
		{
			if (item == null) return null;
			return FindNode(root, item);
		}
		
		public void WalkTreeRootFirst (Action<T> action)
		{
			WalkTreeRootFirst (root, action);
		}
		
		public void WalkTreeRootFirst (Action<DependencyTreeNode<T>> action)
		{
			WalkTreeRootFirst (root, action);
		}
		
		public void WalkTreeChildrenFirst (Action<T> action)
		{
			WalkTreeChildrenFirst (root, action);
		}
		
		public void WalkTreeChildrenFirst (Action<DependencyTreeNode<T>> action)
		{
			WalkTreeChildrenFirst (root, action);
		}
		
		public void AddDependency (T item, T dependency)
		{
			DependencyTreeNode<T> depNode = null;
			
			if (dependency == null)
				depNode = root;
			else
				depNode = FindNode(dependency);
			
			DependencyTreeNode<T> itemNode = FindNode(item);
			
			if (depNode == null)
				depNode = root.AddDependency(dependency);
			
			if (itemNode == null)
				depNode.AddDependency(item);
			else if (dependency != null)
				itemNode.Reparent(depNode);
		}
		
		public DependencyTreeNode<T> Root
		{
			get { return root; }
		}
	}
}
