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
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.Profiler.Controller.Data.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// A CallTreeNode based on a ProfilingDataSQLiteProvider.
	/// </summary>
	sealed class SQLiteCallTreeNode : CallTreeNode
	{
		internal int nameId;
		internal int callCount;
		internal long cpuCyclesSpent;
		internal long cpuCyclesSpentSelf;
		CallTreeNode parent;
		SQLiteQueryProvider provider;
		internal bool hasChildren;
		internal int activeCallCount;
		
		/// <summary>
		/// Creates a new CallTreeNode.
		/// </summary>
		public SQLiteCallTreeNode(int nameId, CallTreeNode parent, SQLiteQueryProvider provider)
		{
			this.nameId = nameId;
			this.parent = parent;
			this.provider = provider;
		}
		
		volatile int[] ids;
		
		/// <summary>
		/// Gets/Sets the ID list.
		/// For function nodes, the usually long ID list is delay-loaded.
		/// </summary>
		internal int[] IdList {
			get {
				int[] tmp = ids;
				if (tmp == null) {
					tmp = provider.LoadIDListForFunction(nameId);
					ids = tmp;
				}
				return tmp;
			}
			set {
				ids = value;
			}
		}

		/// <summary>
		/// Gets a reference to the name, return type and parameter list of the method.
		/// </summary>
		public override NameMapping NameMapping {
			get {
				if (nameId == 0)
					return new NameMapping(0, null, "Merged node", null);
				
				return provider.GetMapping(nameId);
			}
		}

		/// <inheritdoc/>
		public override int RawCallCount {
			get {
				return callCount;
			}
		}

		/// <summary>
		/// Gets how many CPU cycles where spent inside this method, including sub calls.
		/// </summary>
		public override long CpuCyclesSpent {
			get{
				return cpuCyclesSpent;
			}
		}
		
		public override long CpuCyclesSpentSelf {
			get { return cpuCyclesSpentSelf; }
		}

		/// <summary>
		/// Gets a reference to the parent of this CallTreeNode.
		/// </summary>
		public override CallTreeNode Parent {
			get {
				return parent;
			}
		}
		
		readonly static IQueryable<CallTreeNode> EmptyQueryable = Enumerable.Empty<CallTreeNode>().AsQueryable();
		
		/// <summary>
		/// Returns all children of the current CallTreeNode, sorted by order of first call.
		/// </summary>
		public override IQueryable<CallTreeNode> Children {
			get {
				// Approx. half of all nodes in a complex data set are leafs.
				// Not doing an SQL Query in these cases can safe time when someone recursively walks the call-tree
				// (e.g. RingDiagramControl)
				if (!hasChildren)
					return EmptyQueryable;
				
				List<int> ids = IdList.ToList();
				Expression<Func<SingleCall, bool>> filterLambda = c => ids.Contains(c.ParentID);
				return provider.CreateQuery(new MergeByName(new Filter(AllCalls.Instance, filterLambda)));
			}
		}

		/// <summary>
		/// Gets the time spent inside the method (including sub calls) in milliseconds.
		/// </summary>
		public override double TimeSpent {
			get {
				return CpuCyclesSpent / (1000.0 * provider.ProcessorFrequency);
			}
		}
		
		public override double TimeSpentSelf {
			get {
				return CpuCyclesSpentSelf / (1000.0 * provider.ProcessorFrequency);
			}
		}
		
		/// <summary>
		/// Gets whether the function call started in a previous data set that's not selected.
		/// </summary>
		public override bool IsActiveAtStart {
			get {
				return activeCallCount > 0;
			}
		}
		
		public override int CallCount {
			get { return Math.Max(1, callCount + activeCallCount); }
		}
		
		/// <summary>
		/// Merges a collection of CallTreeNodes into one CallTreeNode, all values are accumulated.
		/// </summary>
		/// <param name="nodes">The collection of nodes to process.</param>
		/// <returns>A new CallTreeNode.</returns>
		public override CallTreeNode Merge(IEnumerable<CallTreeNode> nodes)
		{
			SQLiteCallTreeNode mergedNode = new SQLiteCallTreeNode(0, null, provider);
			List<int> mergedIds = new List<int>();
			bool initialised = false;
			
			foreach (SQLiteCallTreeNode node in nodes) {
				mergedIds.AddRange(node.IdList);
				mergedNode.callCount += node.callCount;

				mergedNode.cpuCyclesSpent += node.cpuCyclesSpent;
				mergedNode.cpuCyclesSpentSelf += node.cpuCyclesSpentSelf;
				mergedNode.activeCallCount += node.activeCallCount;
				mergedNode.hasChildren |= node.hasChildren;
				if (!initialised || mergedNode.nameId == node.nameId)
					mergedNode.nameId = node.nameId;
				else
					mergedNode.nameId = 0;
				initialised = true;
			}
			mergedIds.Sort();
			mergedNode.IdList = mergedIds.ToArray();
			
			return mergedNode;
		}
		
		public override IQueryable<CallTreeNode> Callers {
			get {
				// parent is not null => this node was created by a
				// 'Children' call => all our IDs come from that parent
				if (parent != null)
					return (new CallTreeNode[] { parent }).AsQueryable();
				
				List<int> parentIDList = provider.RunSQLIDList(
					"SELECT parentid FROM Calls "
					+ "WHERE id IN(" + string.Join(",", IdList.Select(s => s.ToString()).ToArray()) + @")");
				
				Expression<Func<SingleCall, bool>> filterLambda = c => parentIDList.Contains(c.ID);
				return provider.CreateQuery(new MergeByName(new Filter(AllCalls.Instance, filterLambda)));
			}
		}
		
		public override bool Equals(CallTreeNode other)
		{
			SQLiteCallTreeNode node = other as SQLiteCallTreeNode;
			if (node != null) {
				
				int[] a = IdList;
				int[] b = node.IdList;
				if (a.Length != b.Length)
					return false;
				
				for (int i = 0; i < a.Length; i++) {
					if (a[i] != b[i])
						return false;
				}
				
				return true;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			// (a[0] * p + a[1]) * p + a[2]
			const int hashPrime = 1000000007;
			int hash = 0;
			
			unchecked {
				foreach (int i in IdList) {
					hash = hash * hashPrime + i;
				}
			}
			
			return hash;
		}
		
		public override bool HasChildren {
			get { return hasChildren; }
		}
	}
}
