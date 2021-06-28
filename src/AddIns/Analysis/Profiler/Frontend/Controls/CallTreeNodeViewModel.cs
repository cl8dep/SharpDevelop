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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.Controls
{
	public class CallTreeNodeViewModel : IViewModel<CallTreeNodeViewModel>, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Member

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		ReadOnlyCollection<CallTreeNodeViewModel> children;
		protected readonly CallTreeNode node;
		CallTreeNodeViewModel parent;
		protected int level;
		
		public CallTreeNode Node { get { return node; } }
		
		public event EventHandler<NodeEventArgs<CallTreeNodeViewModel>> VisibleChildExpandedChanged;
		public event EventHandler<NodeEventArgs<CallTreeNodeViewModel>> RequestBringIntoView;
		
		protected virtual void OnRequestBringIntoView(NodeEventArgs<CallTreeNodeViewModel> e)
		{
			if (RequestBringIntoView != null) {
				RequestBringIntoView(this, e);
			}
		}
		
		protected virtual void OnVisibleChildExpandedChanged(NodeEventArgs<CallTreeNodeViewModel> e)
		{
			if (VisibleChildExpandedChanged != null) {
				VisibleChildExpandedChanged(this, e);
			}
		}
		
		public void BringIntoView()
		{
			BringIntoView(this);
		}
		
		void BringIntoView(CallTreeNodeViewModel item)
		{
			if (parent != null) {
				parent.BringIntoView(item);
				return;
			}
			OnRequestBringIntoView(new NodeEventArgs<CallTreeNodeViewModel>(item));
		}
		
		public int Level
		{
			get { return level; }
			set { level = value; }
		}
		
		public CallTreeNodeViewModel Parent
		{
			get { return parent; }
		}

		public virtual long CpuCyclesSpent
		{
			get {
				return node.CpuCyclesSpent;
			}
		}
		
		public virtual double TimePercentageOfParent
		{
			get {
				if (parent == null)
					return 1;
				else
					return (double)node.CpuCyclesSpent / (double)parent.node.CpuCyclesSpent;
			}
		}
		
		public Visibility HotPathIndicatorVisibility {
			get {
				if (TimePercentageOfParent >= 0.2)
					return Visibility.Visible;
				
				return Visibility.Collapsed;
			}
		}
		
		public virtual string TimePercentageOfParentAsText
		{
			get {
				return (TimePercentageOfParent * 100).ToString("0.00") + "%";
			}
		}
		
		public virtual object ToolTip
		{
			get { return CreateShortToolTip(); }
		}
		
		object CreateShortToolTip()
		{
			if (level == 0)
				return null; // no tooltip for root
			if (node.IsThread)
				return Name;

			TextBlock text = new TextBlock {
				Inlines = {
					((!string.IsNullOrEmpty(node.ReturnType)) ? node.ReturnType + " " : ""),
					new Bold { Inlines = { node.Name } },
					"(" + ((node.Parameters.Count > 0) ? string.Join(", ", node.Parameters.ToArray()) : "") + ")"
				}
			};

			return text;
		}

		public object CreateToolTip(ControlsTranslation translation)
		{
			if (node.IsThread)
				return Name; // only name for threads

			TextBlock text = new TextBlock
			{
				Inlines = {
					((!string.IsNullOrEmpty(node.ReturnType)) ? node.ReturnType + " " : ""),
					new Bold { Inlines = { node.Name } },
					"(" + ((node.Parameters.Count > 0) ? string.Join(", ", node.Parameters.ToArray()) : "") + ")\n",
					new Bold { Inlines = { translation.CpuCyclesText } },
					" " + node.CpuCyclesSpent + "\n",
					new Bold { Inlines = { translation.TimeText } },
					" " + node.TimeSpent.ToString("f6") + "ms\n",
					new Bold { Inlines = { translation.CallsText } },
					" " + node.CallCount.ToString()
				}
			};

			return text;
		}
		
		public string GetSignature()
		{
			if (node.IsThread)
				return Name;
			return node.Signature;
		}

		public ReadOnlyCollection<string> Parameters
		{
			get {
				return new ReadOnlyCollection<string>(node.Parameters);
			}
		}

		public CallTreeNodeViewModel(CallTreeNode node, CallTreeNodeViewModel parent)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			this.node = node;
			this.parent = parent;
			this.level = (this.parent == null) ? 1 : this.parent.level + 1;
		}

		public bool IsAncestorOf(CallTreeNodeViewModel descendant)
		{
			if (this != descendant) {
				if (descendant == null)
					return false;

				return IsAncestorOf(descendant.Parent);
			}
			
			return true;
		}

		public virtual ReadOnlyCollection<CallTreeNodeViewModel> Children {
			get {
				lock (this) {
					if (children == null) {
						
						children = node.Children
							.Select(c => new CallTreeNodeViewModel(c, this))
							.ToList()
							.AsReadOnly();
					}
					
					return children;
				}
			}
		}

		public virtual string Name
		{
			get {
				return node.Name ?? string.Empty;
			}
		}

		public virtual string FullyQualifiedClassName
		{
			get {
				string nodeName = node.Name;
				if (string.IsNullOrEmpty(nodeName))
					return null;
				if (!nodeName.Contains("."))
					return null;
				int index = nodeName.LastIndexOf('.');
				if (nodeName[index - 1] == '.')
					index--;
				return nodeName.Substring(0, index);
			}
		}

		public virtual string MethodName
		{
			get {
				string nodeName = node.Name;
				if (string.IsNullOrEmpty(nodeName))
					return null;
				if (!nodeName.Contains("."))
					return null;
				int index = nodeName.LastIndexOf('.');
				if (nodeName[index - 1] == '.')
					index--;
				return nodeName.Substring(index + 1);
			}
		}
		
		public virtual string ShortName
		{
			get {
				string className = FullyQualifiedClassName;
				if (className == null)
					return Name;
				int index = className.LastIndexOf('.');
				return className.Substring(index + 1) + "." + MethodName;
			}
		}
		
		bool isExpanded;
		
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded != value) {
					isExpanded = value;
					OnPropertyChanged("IsExpanded");
					IsExpandedChanged(new NodeEventArgs<CallTreeNodeViewModel>(this));
					
					if (isExpanded) {
						DeselectChildren(children);
					}
				}
			}
		}

		void DeselectChildren(IList<CallTreeNodeViewModel> children)
		{
			foreach (CallTreeNodeViewModel item in children) {
				item.IsSelected = false;
				if (item.isExpanded)
					DeselectChildren(item.Children);
			}
		}
		
		void IsExpandedChanged(NodeEventArgs<CallTreeNodeViewModel> e)
		{
			visibleElementCount = 1 + (IsExpanded ? Children.Sum(c => c.VisibleElementCount) : 0);
			OnVisibleChildExpandedChanged(e);
			
			if (Parent != null && Parent.IsExpanded) {
				Parent.IsExpandedChanged(e);
			}
		}

		/*void IsSelectedChanged(CallTreeNodeViewModel item)
		{
			if (Parent != null)
				Parent.IsSelectedChanged(item);
			else
				OnChildSelectedChanged(new CallTreeNodeViewModelEventArgs(item));
		}

		protected virtual void OnChildSelectedChanged(CallTreeNodeViewModelEventArgs eventArgs)
		{
			if (ChildSelectedChanged != null)
				ChildSelectedChanged(this, eventArgs);
		}*/
		
		bool isSelected;
		
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnPropertyChanged("IsSelected");
					//this.IsSelectedChanged(this);
					
					if (isSelected) {
						CallTreeNodeViewModel node = Parent;
						while (node != null) {
							node.IsExpanded = true;
							node = node.Parent;
						}
					}
				}
			}
		}
		
		public string TimeSpent {
			get {
				if (!node.IsThread)
					return node.TimeSpent.ToString("f6") + "ms";
				else
					return null;
			}
		}
		
		public string TimeSpentSelf {
			get {
				if (!node.IsThread)
					return node.TimeSpentSelf.ToString("f6") + "ms";
				else
					return null;
			}
		}
		
		public string TimeSpentSelfPerCall {
			get {
				if (!node.IsThread)
					return (node.TimeSpentSelf / node.CallCount).ToString("f6") + "ms";
				else
					return null;
			}
		}
		
		public string TimeSpentPerCall {
			get {
				if (!node.IsThread)
					return (node.TimeSpent / node.CallCount).ToString("f6") + "ms";
				else
					return null;
			}
		}
		
		public string CallCount {
			get {
				if (!node.IsThread)
					return node.CallCount.ToString();
				else
					return null;
			}
		}
		
		public Visibility CheckBoxVisibility
		{
			get {
				if (node.HasChildren)
					return Visibility.Visible;
				else
					return Visibility.Hidden;
			}
		}
		
		public override string ToString()
		{
			return "[" + Name + "]";
		}

		#region IViewModel<CallTreeNodeViewModel> Member
		int visibleElementCount = 1;

		public virtual int VisibleElementCount
		{
			get {
				return visibleElementCount;
			}
		}
		
		public virtual Thickness IndentationMargin {
			get {
				return new Thickness((level - 1) * 12, 0, 2, 0);
			}
		}
		#endregion
	}
}
