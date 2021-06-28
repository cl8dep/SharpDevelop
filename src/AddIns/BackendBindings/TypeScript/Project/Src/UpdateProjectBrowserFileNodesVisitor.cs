﻿// 
// UpdateProjectBrowserFileNodesVisitor.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TypeScriptBinding
{
	public class UpdateProjectBrowserFileNodesVisitor : ProjectBrowserTreeNodeVisitor
	{
		ProjectItemEventArgs projectItemEventArgs;
		FileProjectItem newFileAddedToProject;
		string directoryForNewFileAddedToProject;
		
		public UpdateProjectBrowserFileNodesVisitor(ProjectItemEventArgs projectItemEventArgs)
		{
			this.projectItemEventArgs = projectItemEventArgs;
			this.newFileAddedToProject = projectItemEventArgs.ProjectItem as FileProjectItem;
		}
		
		string DirectoryForNewFileAddedToProject {
			get {
				if (directoryForNewFileAddedToProject == null) {
					directoryForNewFileAddedToProject = Path.GetDirectoryName(newFileAddedToProject.FileName);
				}
				return directoryForNewFileAddedToProject;
			}
		}
		
		public override object Visit(ProjectNode projectNode, object data)
		{
			if (IsFileAddedInProject(projectNode)) {
				return Visit((DirectoryNode)projectNode, data);
			}
			return null;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			if (!ShouldVisitDirectoryNode(directoryNode))
				return null;
			
			if (IsImmediateParentForNewFile(directoryNode)) {
				if (IsNewFileIsDependentUponAnotherFile()) {
					base.Visit(directoryNode, data);
				} else if (IsChildFileNodeMissingForNewFile(directoryNode)) {
					AddFileOrDirectoryNodeTo(directoryNode);
				}
			} else if (IsChildDirectoryNodeMissingForNewFile(directoryNode)) {
				AddChildDirectoryNodeForNewFileTo(directoryNode);
			} else {
				return base.Visit(directoryNode, data);
			}
			return null;
		}
		
		bool ShouldVisitDirectoryNode(DirectoryNode directoryNode)
		{
			return directoryNode.IsInitialized && IsNewFileInsideDirectory(directoryNode);
		}
		
		bool IsNewFileInsideDirectory(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(directoryNode.Directory, DirectoryForNewFileAddedToProject);
		}
		
		bool IsFileAddedInProject(ProjectNode projectNode)
		{
			return projectNode.Project == newFileAddedToProject.Project;
		}
		
		bool IsImmediateParentForNewFile(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(DirectoryForNewFileAddedToProject, directoryNode.Directory);
		}
		
		bool IsNewFileIsDependentUponAnotherFile()
		{
			return !String.IsNullOrEmpty(newFileAddedToProject.DependentUpon);
		}
		
		string GetDirectoryForFileAddedToProject()
		{
			return Path.GetDirectoryName(newFileAddedToProject.FileName);
		}
		
		void AddChildDirectoryNodeForNewFileTo(DirectoryNode parentNode)
		{
			string childDirectory = GetMissingChildDirectory(parentNode.Directory);
			AddDirectoryNodeTo(parentNode, childDirectory);
		}
		
		string GetMissingChildDirectory(string parentDirectory)
		{
			string relativeDirectoryForNewFile = GetRelativeDirectoryForNewFile(parentDirectory);
			string childDirectoryName = GetFirstChildDirectoryName(relativeDirectoryForNewFile);
			return Path.Combine(parentDirectory, childDirectoryName);
		}
		
		string GetRelativeDirectoryForNewFile(string baseDirectory)
		{
			return FileUtility.GetRelativePath(baseDirectory, DirectoryForNewFileAddedToProject);
		}
		
		string GetFirstChildDirectoryName(string fullSubFolderPath)
		{
			return fullSubFolderPath.Split('\\').First();
		}

		void AddDirectoryNodeTo(TreeNode parentNode, string directory)
		{
			var directoryNode = new DirectoryNode(directory, FileNodeStatus.InProject);
			directoryNode.InsertSorted(parentNode);
		}

		void AddFileOrDirectoryNodeTo(DirectoryNode directoryNode)
		{
			if (newFileAddedToProject.ItemType == ItemType.Folder) {
				AddDirectoryNodeTo(directoryNode, newFileAddedToProject.FileName);
			} else {
				AddFileNodeTo(directoryNode);
			}
		}
		
		void AddFileNodeTo(TreeNode node, FileNodeStatus status = FileNodeStatus.InProject)
		{
			var fileNode = new FileNode(newFileAddedToProject.FileName, status);
			fileNode.InsertSorted(node);
		}
		
		bool IsChildFileNodeMissingForNewFile(DirectoryNode parentDirectoryNode)
		{
			return !IsChildFileNodeAlreadyAddedForNewFile(parentDirectoryNode);
		}
		
		bool IsChildFileNodeAlreadyAddedForNewFile(DirectoryNode parentDirectoryNode)
		{
			return GetChildFileNodes(parentDirectoryNode)
				.Any(childFileNode => FileNodeMatchesNewFileAdded(childFileNode));
		}
		
		bool FileNodeMatchesNewFileAdded(FileNode fileNode)
		{
			return FileUtility.IsEqualFileName(fileNode.FileName, newFileAddedToProject.FileName);
		}
		
		bool IsChildDirectoryNodeMissingForNewFile(DirectoryNode parentDirectoryNode)
		{
			return !IsChildDirectoryNodeAlreadyAddedForNewFile(parentDirectoryNode);
		}
		
		bool IsChildDirectoryNodeAlreadyAddedForNewFile(DirectoryNode parentDirectoryNode)
		{
			return GetChildDirectoryNodes(parentDirectoryNode)
				.Any(childDirectoryNode => DirectoryOfNewFileStartsWith(childDirectoryNode));
		}
		
		bool DirectoryOfNewFileStartsWith(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(directoryNode.Directory, DirectoryForNewFileAddedToProject);
		}
		
		IEnumerable<FileNode> GetChildFileNodes(ExtTreeNode parentNode)
		{
			return parentNode.AllNodes.OfType<FileNode>();
		}
		
		IEnumerable<DirectoryNode> GetChildDirectoryNodes(ExtTreeNode parentNode)
		{
			return parentNode.AllNodes.OfType<DirectoryNode>();
		}
		
		public override object Visit(FileNode fileNode, object data)
		{
			if (IsNewFileIsDependentUponAnotherFile()) {
				if (IsImmediateParentForNewFile(fileNode)) {
					AddFileNodeTo(fileNode, FileNodeStatus.BehindFile);
					return null;
				}
			}
			return base.Visit(fileNode, data);
		}
		
		bool IsImmediateParentForNewFile(FileNode fileNode)
		{
			return FileProjectItemExtensions.IsDependentUponFileName(newFileAddedToProject, fileNode.FileName);
		}
	}
}
