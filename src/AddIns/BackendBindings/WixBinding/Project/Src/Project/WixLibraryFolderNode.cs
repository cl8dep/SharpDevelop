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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// The parent node that contains WixLibrary project items.
	/// </summary>
	public class WixLibraryFolderNode : CustomFolderNode
	{
		IProject project;
		
		public WixLibraryFolderNode(IProject project)
		{
			this.project = project;
			Text = StringParser.Parse("${res:ICSharpCode.WixBinding.WixLibraryFolderNode.Text}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixLibraryFolderNode";
			
			foreach (ProjectItem item in project.Items) {
				if (item is WixLibraryProjectItem) {
					CustomNode node = new CustomNode();
					node.AddTo(this);
					break;
				}
			}
		}
		
		public override void Refresh()
		{
			AddLibraryNodes();
			base.Refresh();
		}
		
		protected override void Initialize()
		{
			AddLibraryNodes();
			base.Initialize();
		}
		
		void AddLibraryNodes()
		{
			Nodes.Clear();

			foreach (ProjectItem item in project.Items) {
				WixLibraryProjectItem wixLibraryItem = item as WixLibraryProjectItem;
				if (wixLibraryItem != null) {
					WixLibraryNode node = new WixLibraryNode(wixLibraryItem);
					node.AddTo(this);
				}
			}
		}
	}
}
