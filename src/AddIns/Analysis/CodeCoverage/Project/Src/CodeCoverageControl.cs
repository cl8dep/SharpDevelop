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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageControl : UserControl
	{
		CodeCoverageTreeView treeView;
		ListView listView;
		SplitContainer verticalSplitContainer;
		SplitContainer horizontalSplitContainer;
		ElementHost textEditorHost;
		TextEditor textEditor;
		string textEditorFileName;
		ColumnHeader visitCountColumnHeader;
		ColumnHeader startLineColumnHeader;
		ColumnHeader endLineColumnHeader;
		ColumnHeader startColumnColumnHeader;
		ColumnHeader endColumnColumnHeader;
		ColumnHeader contentColumnHeader;
		ToolStrip toolStrip;
		bool showSourceCodePanel;
		bool showVisitCountPanel = true;
		SequencePointListViewSorter sequencePointListViewSorter;
		
		public CodeCoverageControl()
		{
			UpdateDisplay();
		}
		
		public void UpdateToolbar()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			toolStrip.Refresh();
		}
		
		public void AddModules(List<CodeCoverageModule> modules)
		{
			treeView.AddModules(modules);
		}
		
		public void Clear()
		{
			treeView.Clear();
			if (listView != null) {
				listView.Items.Clear();
			}
			if (textEditor != null) {
				textEditorFileName = null;
				textEditor.Text = String.Empty;
			}
		}
		
		public bool ShowSourceCodePanel {
			get { return showSourceCodePanel; }
			set {
				if (showSourceCodePanel != value) {
					showSourceCodePanel = value;
					OnShowSourceCodePanelChanged();
					UpdateDisplay();
					DisplaySelectedItem(treeView.SelectedNode as CodeCoverageTreeNode);
				}
			}
		}
		
		public bool ShowVisitCountPanel {
			get { return showVisitCountPanel; }
			set {
				if (showVisitCountPanel != value) {
					showVisitCountPanel = value;
					OnShowVisitCountPanelChanged();
					UpdateDisplay();
					DisplaySelectedItem(treeView.SelectedNode as CodeCoverageTreeNode);
				}
			}
		}
		
		/// <summary>
		/// Adds or removes the visit count list view or the source code 
		/// panel.
		/// </summary>
		void UpdateDisplay()
		{
			CreateTreeView();

			if (showVisitCountPanel) {
				CreateListView();
			} else {
				DisposeListView();
			}
			
			if (showSourceCodePanel) {
				CreateTextEditor();
			} else {
				DisposeTextEditor();
			}
			
			if (showVisitCountPanel || showSourceCodePanel) {
				CreateVerticalSplitContainer();
			} else {
				DisposeVerticalSplitContainer();
			}
			
			if (showSourceCodePanel && showVisitCountPanel) {
				CreateHorizontalSplitContainer();
			} else {
				DisposeHorizontalSplitContainer();
			}
			
			// Add tree view.
			if (showVisitCountPanel || showSourceCodePanel) {
				if (Controls.Contains(treeView)) {
					Controls.Remove(treeView);
				}
				if (!verticalSplitContainer.Panel1.Controls.Contains(treeView)) {
					verticalSplitContainer.Panel1.Controls.Add(treeView);
				}
			} else {
				if (!Controls.Contains(treeView)) {
					Controls.Add(treeView);
				}
			}		
			
			// Add list view.
			if (showVisitCountPanel) {
				if (showSourceCodePanel) {
					if (verticalSplitContainer.Panel2.Controls.Contains(listView)) {
						verticalSplitContainer.Panel2.Controls.Remove(listView);
					}
					if (!horizontalSplitContainer.Panel1.Controls.Contains(listView)) {
						horizontalSplitContainer.Panel1.Controls.Add(listView);
					}
				} else {
					if (!verticalSplitContainer.Panel2.Controls.Contains(listView)) {
						verticalSplitContainer.Panel2.Controls.Add(listView);
					}
				}
			}
			
			// Add text editor
			if (showSourceCodePanel) {
				if (showVisitCountPanel) {
					if (verticalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
						verticalSplitContainer.Panel2.Controls.Remove(textEditorHost);
					}
					if (!horizontalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
						horizontalSplitContainer.Panel2.Controls.Add(textEditorHost);
					}
				} else {
					if (!verticalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
						verticalSplitContainer.Panel2.Controls.Add(textEditorHost);
					}
				}
			}
			
			// Add vertical split container.
			if (showVisitCountPanel || showSourceCodePanel) {
				if (!Controls.Contains(verticalSplitContainer)) {
					Controls.Add(verticalSplitContainer);
				}
			}		
			
			// Add horizontal split container.
			if (showVisitCountPanel && showSourceCodePanel) {
				if (!verticalSplitContainer.Panel2.Controls.Contains(horizontalSplitContainer)) {
					verticalSplitContainer.Panel2.Controls.Add(horizontalSplitContainer);
				}
			}
			
			// Add toolstrip - need to re-add it last otherwise the
			// other controls will be displayed underneath it.
			if (toolStrip == null) {
				toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/CodeCoveragePad/Toolbar");
				toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			}
			if (Controls.Contains(toolStrip)) {
				Controls.Remove(toolStrip);
			}
			Controls.Add(toolStrip);
		}
		
		void CodeCoverageTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			DisplaySelectedItem((CodeCoverageTreeNode)e.Node);
		}
		
		void DisplaySelectedItem(CodeCoverageTreeNode node)
		{
			if (node == null) {
				return;
			}
			
			if (listView != null) {
				UpdateListView(node);
			} 
			if (textEditor != null) {
				UpdateTextEditor(node);
			}
		}
		
		void UpdateListView(CodeCoverageTreeNode node)
		{
			listView.BeginUpdate();
			try {
				listView.Items.Clear();
				var classNode = node as CodeCoverageClassTreeNode;
				var methodNode = node as CodeCoverageMethodTreeNode;
				var propertyNode = node as CodeCoveragePropertyTreeNode;
				if (classNode != null) {
					AddClassTreeNode(classNode);
				} else if (methodNode != null) {
					AddSequencePoints(methodNode.Method);
				} else if (propertyNode != null) {
					AddPropertyTreeNode(propertyNode);
				}
			} finally {
				listView.EndUpdate();
			}
		}
		
		void UpdateTextEditor(CodeCoverageTreeNode node)
		{
			var classNode = node as CodeCoverageClassTreeNode;
			var methodNode = node as CodeCoverageMethodTreeNode;
			var propertyNode = node as CodeCoveragePropertyTreeNode;
			if (classNode != null && classNode.Nodes.Count > 0) {
				propertyNode = classNode.Nodes[0] as CodeCoveragePropertyTreeNode;
				methodNode = classNode.Nodes[0] as CodeCoverageMethodTreeNode;
			} 
			
			if (propertyNode != null && propertyNode.Nodes.Count > 0) {
				methodNode = propertyNode.Nodes[0] as CodeCoverageMethodTreeNode;
			}
			
			if (methodNode != null && methodNode.Method.SequencePoints.Count > 0) {
				CodeCoverageSequencePoint sequencePoint = methodNode.Method.SequencePoints[0];
				if (sequencePoint.HasDocument()) {
					if (classNode == null) {
						OpenFile(sequencePoint.Document, sequencePoint.Line, sequencePoint.Column);
					} else {
						OpenFile(sequencePoint.Document, 1, 1);
					}
				}
			}
		}
		
		void AddClassTreeNode(CodeCoverageClassTreeNode node)
		{
			foreach (CodeCoverageTreeNode childNode in node.Nodes) {
				var method = childNode as CodeCoverageMethodTreeNode;
				var property = childNode as CodeCoveragePropertyTreeNode;
				if (method != null) {
					AddSequencePoints(method.Method);
				} else {
					AddPropertyTreeNode(property);
				}
			}
		}
		
		void AddPropertyTreeNode(CodeCoveragePropertyTreeNode node)
		{
			AddMethodIfNotNull(node.Property.Getter);
			AddMethodIfNotNull(node.Property.Setter);
		}
		
		void AddMethodIfNotNull(CodeCoverageMethod method)
		{
			if (method != null) {
				AddSequencePoints(method);
			}
		}
		
		void AddSequencePoints(CodeCoverageMethod method)
		{		
			foreach (CodeCoverageSequencePoint sequencePoint in method.SequencePoints) {
				if (method.FileID == sequencePoint.FileID)
					AddSequencePoint(sequencePoint);
			}
		}
		
		void AddSequencePoint(CodeCoverageSequencePoint sequencePoint)
		{
			var item = new ListViewItem(sequencePoint.VisitCount.ToString());
			item.SubItems.Add(sequencePoint.Line.ToString());
			item.SubItems.Add(sequencePoint.Column.ToString());
			item.SubItems.Add(sequencePoint.EndLine.ToString());
			item.SubItems.Add(sequencePoint.EndColumn.ToString());
			item.SubItems.Add(sequencePoint.Content.Length>80?sequencePoint.Content.Substring(0,80):sequencePoint.Content);
			item.BackColor = CodeCoverageHighlighter.GetSequencePointBackColor(sequencePoint);
			item.ForeColor = CodeCoverageHighlighter.GetSequencePointForeColor(sequencePoint);
			item.Tag = sequencePoint;
			
			listView.Items.Add(item);
		}

		void ListViewItemActivate(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count > 0) {
				var sequencePoint = (CodeCoverageSequencePoint)listView.SelectedItems[0].Tag;
				if (sequencePoint.Document.Length > 0) {
					FileService.JumpToFilePosition(sequencePoint.Document, sequencePoint.Line, sequencePoint.Column);
				}
			}
		}
		
		void OpenFile(string fileName, int line, int column)
		{
			if (fileName != textEditorFileName) {
				if (!TryLoadFileIntoTextEditor(fileName)) {
					return;
				}
				textEditor.SyntaxHighlighting = GetSyntaxHighlighting(fileName);
			}
			textEditor.ScrollToEnd();
			textEditor.TextArea.Caret.Location = new TextLocation(line, column);
			textEditor.ScrollToLine(line);
			CodeCoverageService.ShowCodeCoverage(new AvalonEditTextEditorAdapter(textEditor), fileName);
		}
		
		bool TryLoadFileIntoTextEditor(string fileName)
		{
			if (!File.Exists(fileName)) {
				textEditor.Text = String.Format("File does not exist '{0}'.", fileName);
				return false;
			}
			
			textEditor.Load(fileName);
			return true;
		}
		
		IHighlightingDefinition GetSyntaxHighlighting(string fileName)
		{
			return HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
		}
		
		void CreateTreeView()
		{
			if (treeView != null) {
				return;
			}
			
			treeView = new CodeCoverageTreeView();
			treeView.Dock = DockStyle.Fill;
			treeView.ImageList = CodeCoverageImageList.ImageList;
			treeView.AfterSelect += CodeCoverageTreeViewAfterSelect;
			
			if (CodeCoverageService.Results.Length > 0) {
				foreach (CodeCoverageResults results in CodeCoverageService.Results) {
					AddModules(results.Modules);
				}
			}
		}
		
		void DisposeTreeView()
		{
			if (treeView == null) {
				return;
			}
			
			treeView.AfterSelect -= CodeCoverageTreeViewAfterSelect;
			if (Controls.Contains(treeView)) {
				Controls.Remove(treeView);
			}
			if (verticalSplitContainer != null && verticalSplitContainer.Panel1.Controls.Contains(treeView)) {
				verticalSplitContainer.Panel1.Controls.Remove(treeView);
			}
			treeView.Dispose();
			treeView = null;
		}

		void CreateListView()
		{
			if (listView != null) {
				return;
			}
			
			listView = new ListView();
			listView.View = View.Details;
			listView.Dock = DockStyle.Fill;
			listView.FullRowSelect = true;
			listView.HideSelection = false;
			listView.ItemActivate += ListViewItemActivate;
			
			listView.Font = Core.WinForms.WinFormsResourceService.DefaultMonospacedFont;
						
			visitCountColumnHeader = new ColumnHeader();
			visitCountColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.CodeCoverage.VisitCount}");
			visitCountColumnHeader.Width = -2;
			
			startLineColumnHeader = new ColumnHeader();
			startLineColumnHeader.Text = StringParser.Parse("${res:Global.TextLine}");
			startLineColumnHeader.Width = -2;
				
			startColumnColumnHeader = new ColumnHeader();
			startColumnColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.CodeCoverage.Column}");
			startColumnColumnHeader.Width = -2;

			endLineColumnHeader = new ColumnHeader();
			endLineColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.CodeCoverage.EndLine}");
			endLineColumnHeader.Width = -2;

			endColumnColumnHeader = new ColumnHeader();
			endColumnColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.CodeCoverage.EndColumn}");
			endColumnColumnHeader.Width = -2;

			contentColumnHeader = new ColumnHeader();
			contentColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.CodeCoverage.Content}");
			contentColumnHeader.Width = 500;

			listView.Columns.AddRange(new ColumnHeader[] {visitCountColumnHeader,
			   	                      startLineColumnHeader,
			                          startColumnColumnHeader,
			                          endLineColumnHeader,
			                          endColumnColumnHeader,
			                          contentColumnHeader
			                          });
			
			// Create custom list view sorter.
			sequencePointListViewSorter = new SequencePointListViewSorter(listView);
		}
						
		void DisposeListView()
		{
			if (listView == null) {
				return;
			}
			
			if (verticalSplitContainer.Panel2.Controls.Contains(listView)) {
				verticalSplitContainer.Panel2.Controls.Remove(listView);
			}
			
			if (horizontalSplitContainer != null && horizontalSplitContainer.Panel1.Controls.Contains(listView)) {
				horizontalSplitContainer.Panel1.Controls.Remove(listView);
			}

			listView.ItemActivate -= ListViewItemActivate;
			listView.Dispose();
			listView = null;
			
			sequencePointListViewSorter.Dispose();
		}
		
		void CreateVerticalSplitContainer()
		{
			if (verticalSplitContainer != null) {
				return;
			}
			
			verticalSplitContainer = new SplitContainer();
			verticalSplitContainer.SplitterWidth = 2;
			verticalSplitContainer.Dock = DockStyle.Fill;
		}
				
		void DisposeVerticalSplitContainer()
		{
			if (verticalSplitContainer == null) {
				return;
			}
			
			if (horizontalSplitContainer != null && verticalSplitContainer.Panel2.Controls.Contains(horizontalSplitContainer)) {
				verticalSplitContainer.Panel2.Controls.Remove(horizontalSplitContainer);
			}
			
			if (listView != null && verticalSplitContainer.Panel2.Controls.Contains(listView)) {
				verticalSplitContainer.Panel2.Controls.Remove(listView);
			}
			
			if (treeView != null && verticalSplitContainer.Panel1.Controls.Contains(treeView)) {
				verticalSplitContainer.Panel1.Controls.Remove(treeView);
			}
			
			verticalSplitContainer.Dispose();
			verticalSplitContainer = null;
		}
		
		void CreateHorizontalSplitContainer()
		{
			if (horizontalSplitContainer != null) {
				return;
			}
			
			horizontalSplitContainer = new SplitContainer();
			horizontalSplitContainer.SplitterWidth = 2;
			horizontalSplitContainer.Orientation = Orientation.Horizontal;
			horizontalSplitContainer.Dock = DockStyle.Fill;
		}
				
		void DisposeHorizontalSplitContainer()
		{
			if (horizontalSplitContainer == null) {
				return;
			}
			
			if (listView != null && horizontalSplitContainer.Panel1.Controls.Contains(listView)) {
				horizontalSplitContainer.Panel1.Controls.Remove(listView);
			}
			
			if (textEditor != null && horizontalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
				horizontalSplitContainer.Panel2.Controls.Remove(textEditorHost);
			}
			
			if (verticalSplitContainer != null && verticalSplitContainer.Panel2.Controls.Contains(horizontalSplitContainer)) {
				verticalSplitContainer.Panel2.Controls.Remove(horizontalSplitContainer);
			}
			
			horizontalSplitContainer.Dispose();
			horizontalSplitContainer = null;
		}
		
		void CreateTextEditor()
		{
			if (textEditorHost != null) {
				return;
			}
			
			textEditor = AvalonEditTextEditorAdapter.CreateAvalonEditInstance();
			
			textEditor.IsReadOnly = true;
			textEditor.MouseDoubleClick += TextEditorDoubleClick;
			
			var adapter = new AvalonEditTextEditorAdapter(textEditor);
			var textMarkerService = new TextMarkerService(adapter.TextEditor.Document);
			adapter.TextEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			adapter.TextEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
			adapter.TextEditor.TextArea.TextView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			
			textEditorHost = new ElementHost();
			textEditorHost.Dock = DockStyle.Fill;
			textEditorHost.Child = textEditor;
		}
		
		void DisposeTextEditor()
		{
			if (textEditorHost == null) {
				return;
			}
			
			if (verticalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
				verticalSplitContainer.Panel2.Controls.Remove(textEditorHost);
			}
			
			if (horizontalSplitContainer != null && horizontalSplitContainer.Panel2.Controls.Contains(textEditorHost)) {
				horizontalSplitContainer.Panel2.Controls.Remove(textEditorHost);
			}
			
			textEditor.MouseDoubleClick -= TextEditorDoubleClick;
			textEditorHost.Dispose();
			textEditorHost = null;
		}
		
		void TextEditorDoubleClick(object sender, EventArgs e)
		{
			string fileName = textEditorFileName;
			if (fileName != null) {
				Caret caret = textEditor.TextArea.Caret;
				FileService.JumpToFilePosition(fileName, caret.Line + 1, caret.Column + 1);
			}
		}
		
		/// <summary>
		/// If the treeview is to be moved to a different parent then
		/// it needs to be recreated otherwise the OnBeforeExpand method
		/// is never called.
		/// </summary>
		void OnShowVisitCountPanelChanged()
		{
			if ((showVisitCountPanel && !showSourceCodePanel) ||
			    (!showVisitCountPanel && !showSourceCodePanel)) {
				// Tree view will be moved to a different parent.
				DisposeTreeView();
			}
		}
		
		void OnShowSourceCodePanelChanged()
		{
			if ((showSourceCodePanel && !showVisitCountPanel) ||
			    (!showSourceCodePanel && !showVisitCountPanel)) {
				// Tree view will be moved to a different parent.
				DisposeTreeView();
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (textEditor != null) {
				DisposeTextEditor();
			}
			base.Dispose(disposing);
		}
	}
}
