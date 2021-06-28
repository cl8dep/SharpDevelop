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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Profiler.Controller.Queries;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Provides a skeleton for implementing data-structure specific versions of CallTreeNodes.
	/// </summary>
	/// <remarks>
	/// All members of this class are thread-safe. However, calling members of this class
	/// might be unsafe when the data source (<see cref="ProfilingDataProvider"/> or
	/// <see cref="UnmanagedProfilingDataSet"/>) is concurrently disposed. See the documentation
	/// of the data source for details.
	/// </remarks>
	public abstract class CallTreeNode : IEquatable<CallTreeNode>
	{
		/// <summary>
		/// Creates the signature representation for the CallTreeNode.
		/// thread nodes will be represented by "Thread#" + their unmanaged thread id.
		/// All other nodes (except root nodes) have the representation:
		/// returntype MethodName(parameter1, parameter2, ...)
		/// </summary>
		public string Signature
		{
			get {
				if (Name.StartsWith("Thread#", StringComparison.Ordinal))
					return Name;
				return ((!string.IsNullOrEmpty(ReturnType)) ? ReturnType + " " : "") + Name +
					"(" + ((Parameters.Count > 0) ? string.Join(", ", Parameters.ToArray()) : "") + ")";
			}
		}

		/// <summary>
		/// Gets a reference to the name, return type and parameter list of the method.
		/// </summary>
		public abstract NameMapping NameMapping { get; }
		
		/// <summary>
		/// Gets the number of calls to the method represented by the CallTreeNode.
		/// </summary>
		public abstract int RawCallCount { get; }
		
		/// <summary>
		/// Gets the number of calls to the method represented by the CallTreeNode.
		/// </summary>
		public virtual int CallCount {
			get {
				return RawCallCount + (IsActiveAtStart ? 1 : 0);
			}
		}
		
		/// <summary>
		/// Gets whether this call is user code.
		/// </summary>
		public virtual bool IsUserCode {
			get {
				return NameMapping.Id > 0;
			}
		}
		
		/// <summary>
		/// Gets whether the function call started in a previous data set that's not selected.
		/// </summary>
		public abstract bool IsActiveAtStart { get; }
		
		/// <summary>
		/// Gets how many CPU cycles were spent inside this method, including sub calls.
		/// </summary>
		public abstract long CpuCyclesSpent { get; }
		
		/// <summary>
		/// Gets how many CPU cycles were spent inside this method, excluding sub calls.
		/// </summary>
		public virtual long CpuCyclesSpentSelf {
			get {
				return CpuCyclesSpent - Children.Sum(item => item.CpuCyclesSpent);
			}
		}
		
		/// <summary>
		/// Gets the name of the method including namespace and class name.
		/// </summary>
		public string Name {
			get {
				NameMapping name = NameMapping;
				return name != null ? name.Name : null;
			}
		}
		
		/// <summary>
		/// Gets the return type of the method as string.
		/// </summary>
		public string ReturnType {
			get {
				NameMapping name = NameMapping;
				return name != null ? name.ReturnType : null;
			}
		}
		
		/// <summary>
		/// Determines whether this node is a thread node.
		/// </summary>
		public virtual bool IsThread {
			get { return Name.StartsWith("Thread#", StringComparison.Ordinal); }
		}
		
		/// <summary>
		/// Determines whether this node has chil
		/// </summary>
		public virtual bool HasChildren {
			get {
				return Children.Any();
			}
		}
		
		/// <summary>
		/// Gets a readonly list of the string representation of the parameters.
		/// </summary>
		public IList<string> Parameters {
			get {
				NameMapping name = NameMapping;
				return name != null ? name.Parameters : NameMapping.EmptyParameterList;
			}
		}
		
		/// <summary>
		/// Gets the time spent inside the method (including sub calls) in milliseconds.
		/// </summary>
		public abstract double TimeSpent { get; }
		
		/// <summary>
		/// Gets the time spent inside the method (excluding sub calls) in milliseconds.
		/// </summary>
		public abstract double TimeSpentSelf { get; }
		
		/// <summary>
		/// Gets a reference to the parent of this CallTreeNode.
		/// </summary>
		public abstract CallTreeNode Parent { get; }
		
		/// <summary>
		/// Returns all children of the current CallTreeNode, sorted by order of first call.
		/// </summary>
		public abstract IQueryable<CallTreeNode> Children {
			get;
		}
		
		/// <summary>
		/// Merges a collection of CallTreeNodes into one CallTreeNode, all valuess are accumulated.
		/// </summary>
		/// <param name="nodes">The collection of nodes to process.</param>
		/// <returns>A new CallTreeNode.</returns>
		public abstract CallTreeNode Merge(IEnumerable<CallTreeNode> nodes);
		
		/// <summary>
		/// Returns all descendants of this CallTreeNode.
		/// </summary>
		public virtual IQueryable<CallTreeNode> Descendants {
			get {
				return GetDescendants(false).AsQueryable();
			}
		}
		
		/// <summary>
		/// Returns all descendants of this CallTreeNode including itself.
		/// </summary>
		public virtual IQueryable<CallTreeNode> DescendantsAndSelf {
			get {
				return GetDescendants(true).AsQueryable();
			}
		}
		
		IEnumerable<CallTreeNode> GetDescendants(bool includeSelf)
		{
			Stack<IEnumerator<CallTreeNode>> stack = new Stack<IEnumerator<CallTreeNode>>();
			try {
				if (includeSelf)
					yield return this; // Descendants is reflexive
				stack.Push(Children.GetEnumerator());
				while (stack.Count > 0) {
					IEnumerator<CallTreeNode> e = stack.Peek();
					if (e.MoveNext()) {
						yield return e.Current;
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
		
		/// <summary>
		/// Returns all ancestors of this CallTreeNode.
		/// </summary>
		public virtual IQueryable<CallTreeNode> Ancestors {
			get {
				return GetAncestors(false).AsQueryable();
			}
		}
		
		/// <summary>
		/// Returns all ancestors of this CallTreeNode including itself.
		/// </summary>
		public virtual IQueryable<CallTreeNode> AncestorsAndSelf {
			get {
				return GetAncestors(true).AsQueryable();
			}
		}
		
		IEnumerable<CallTreeNode> GetAncestors(bool includeSelf)
		{
			CallTreeNode n = includeSelf ? this : Parent;
			while (n != null) {
				yield return n;
				n = n.Parent;
			}
		}
		
		/// <summary>
		/// Returns a list of all CallTreeNodes that are callers of this node.
		/// </summary>
		public abstract IQueryable<CallTreeNode> Callers {
			get;
		}
		
		/// <summary>
		/// Returns a descendant of this node described by its absolute NodePath.
		/// </summary>
		public virtual CallTreeNode GetDescendantByPath(NodePath path)
		{
			CallTreeNode node = this;
			
			foreach (int nameId in path) {
				node = node.Children.FirstOrDefault(n => n.NameMapping.Id == nameId);
				if (node == null)
					return null;
			}
			
			return node;
		}
		
		/// <summary>
		/// Returns the NodePath of this CallTreeNode.
		/// </summary>
		public virtual IEnumerable<NodePath> GetPath()
		{
			bool hasItems = false;
			foreach (CallTreeNode caller in Callers) {
				Debug.Print("caller: " + caller);
				foreach (NodePath p in caller.GetPath()) {
					hasItems = true;
					yield return p.Append(NameMapping.Id);
				}
			}
			
			if (!hasItems)
				yield return NodePath.Empty;
		}
		
		/// <summary>
		/// Returns the NodePath of this CallTreeNode relative to the specified node.
		/// </summary>
		/// <remarks>This might not work properly for merged nodes.</remarks>
		public virtual IEnumerable<NodePath> GetPathRelativeTo(CallTreeNode relativeTo)
		{
			if (relativeTo.Equals(this))
				yield return NodePath.Empty;
			else {
				foreach (CallTreeNode caller in Callers) {
					foreach (NodePath p in caller.GetPathRelativeTo(relativeTo))
						yield return p.Append(NameMapping.Id);
				}
			}
		}
		
		/// <inheritdoc/>
		public sealed override bool Equals(object obj)
		{
			return Equals(obj as CallTreeNode);
		}
		
		/// <summary>
		/// Returns whether this and the other CallTreeNode are equal or not.
		/// </summary>
		public abstract bool Equals(CallTreeNode other);
		
		/// <inheritdoc/>
		public abstract override int GetHashCode();
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " " + NameMapping.Id + " " + NameMapping.Name + "]";
		}
	}
}
