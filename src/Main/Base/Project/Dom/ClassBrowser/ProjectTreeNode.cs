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
using System.Collections.Generic;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ProjectTreeNode : ModelCollectionTreeNode
	{
		class ProjectChildComparer : IComparer<SharpTreeNode>
		{
			IComparer<string> stringComparer = StringComparer.OrdinalIgnoreCase;
			
			public int Compare(SharpTreeNode x, SharpTreeNode y)
			{
				// "References" node has precedence over other nodes
				if ((x is AssemblyReferencesTreeNode) && !(y is AssemblyReferencesTreeNode))
					return -1;
				if (!(x is AssemblyReferencesTreeNode) && (y is AssemblyReferencesTreeNode))
					return 1;
				
				// All other nodes are compared by their Text property
				return stringComparer.Compare(x.Text.ToString(), y.Text.ToString());
			}
		}
		
		protected static readonly IComparer<SharpTreeNode> ChildNodeComparer = new ProjectChildComparer();
		
		IProject project;
		IAssemblyReferencesModel assemblyReferencesModel;
		
		public ProjectTreeNode(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			assemblyReferencesModel = project.AssemblyModel.References;
		}
		
		protected override object GetModel()
		{
			return project;
		}
		
		public override object Text {
			get {
				return project.Name;
			}
		}
		
		public override object Icon {
			get {
				return IconService.GetImageSource(IconService.GetImageForProjectType(project.Language));
			}
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get {
				return ChildNodeComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return project.AssemblyModel.Namespaces;
			}
		}
		
		protected override void InsertSpecialNodes()
		{
			var treeNode = SD.TreeNodeFactory.CreateTreeNode(assemblyReferencesModel);
			if (treeNode != null)
				Children.OrderedInsert(treeNode, ChildNodeComparer);
		}
	}
}
