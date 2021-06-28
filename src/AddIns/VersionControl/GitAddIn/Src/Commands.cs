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
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.GitAddIn
{
	public abstract class GitCommand : AbstractMenuCommand
	{
		protected abstract void Run(string filename, Action callback);
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node != null) {
				string nodeFileName = null;
				if (node is DirectoryNode) {
					nodeFileName = ((DirectoryNode)node).Directory;
				} else if (node is FileNode) {
					nodeFileName =  ((FileNode)node).FileName;
				} else if (node is SolutionNode) {
					nodeFileName = ((SolutionNode)node).Solution.Directory;
				}
				if (nodeFileName != null) {
					List<OpenedFile> unsavedFiles = new List<OpenedFile>();
					foreach (OpenedFile file in SD.FileService.OpenedFiles) {
						if (file.IsDirty && !file.IsUntitled) {
							if (string.IsNullOrEmpty(file.FileName)) continue;
							if (FileUtility.IsUrl(file.FileName)) continue;
							if (FileUtility.IsBaseDirectory(nodeFileName, file.FileName)) {
								unsavedFiles.Add(file);
							}
						}
					}
					if (unsavedFiles.Count > 0) {
						if (MessageService.ShowCustomDialog(
							MessageService.DefaultMessageBoxTitle,
							"The version control operation would affect files with unsaved modifications.\n" +
							"You have to save those files before running the operation.",
							0, 1,
							"Save files", "Cancel")
						    == 0)
						{
							// Save
							foreach (OpenedFile file in unsavedFiles) {
								ICSharpCode.SharpDevelop.Commands.SaveFile.Save(file);
							}
						} else {
							// Cancel
							return;
						}
					}
					// now run the actual operation:
					Run(nodeFileName, AfterCommand(nodeFileName, node));
				}
			}
		}
		
		Action AfterCommand(string nodeFileName, AbstractProjectBrowserTreeNode node)
		{
			return delegate {
				SD.MainThread.VerifyAccess();
				// and then refresh the project browser:
				GitStatusCache.ClearCachedStatus(nodeFileName);
				OverlayIconManager.EnqueueRecursive(node);
				OverlayIconManager.EnqueueParents(node);
			};
		}
	}
	
	public class GitCommitCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Commit(filename, callback);
		}
	}
	
	public class GitDiffCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Diff(filename, callback);
		}
	}
	
	public class GitLogCommand : GitCommand
	{
		protected override void Run(string filename, Action callback)
		{
			GitGuiWrapper.Log(filename, callback);
		}
	}
}
