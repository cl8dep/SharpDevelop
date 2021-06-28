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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	public class WixDialogDesigner : FormsDesignerViewContent, IWixDialogDesigner
	{
		string dialogId = String.Empty;
		WixProject wixProject;
		
		/// <summary>
		/// Ignore the dialog id in the text editor if the OpenDialog method is called
		/// and the designer is not being opened by the user clicking the Design tab.
		/// </summary>
		bool ignoreDialogIdSelectedInTextEditor;
		
		public WixDialogDesigner(IViewContent view)
			: this(view, new WixDialogDesignerLoaderProvider())
		{
		}
		
		public WixDialogDesigner(IViewContent view, WixDialogDesignerLoaderProvider designerLoaderProvider)
			: base(view, designerLoaderProvider)
		{
		}
		
		/// <summary>
		/// Gets the WixDialogDesigner from the primary view.
		/// </summary>
		/// <returns>The wix dialog designer view that is attached as a
		/// secondary view; <see langword="null"/> if the primary view
		/// has no such designer attached.</returns>
		public static WixDialogDesigner GetDesigner(IViewContent view)
		{
			foreach (IViewContent secondaryView in view.SecondaryViewContents) {
				WixDialogDesigner designer = secondaryView as WixDialogDesigner;
				if (designer != null) {
					return designer;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Attempts to open the Wix dialog from the document currently
		/// associated with this designer.
		/// </summary>
		/// <param name="id">The id of the dialog that will be opened.</param>
		public void OpenDialog(string id)
		{
			JumpToDialogElement(id);
			if (base.Host != null) {
				// Reload so the correct dialog is displayed.
				MergeAndUnloadDesigner();
				DialogId = id;
				ReloadDesignerFromMemory();
			} else {
				// Need to open the designer.
				DialogId = id;
				OpenDesigner();
			}
		}
		
		protected override void LoadInternal(OpenedFile file, Stream stream)
		{
			if (file == this.PrimaryFile) {
				// The FormsDesignerViewContent normally operates independently of any
				// text editor. The following statements connect the forms designer
				// directly to the underlying XML text editor so that the caret positioning
				// and text selection operations done by the WiX designer actually
				// become visible in the text editor.
				if (!this.SourceCodeStorage.ContainsFile(file)) {
					ITextEditor editor = this.PrimaryViewContent.GetService<ITextEditor>();
					this.SourceCodeStorage.AddFile(file, editor.Document, SD.FileService.DefaultFileEncoding, true);
				}
				
				try {
					if (!ignoreDialogIdSelectedInTextEditor) {
						string dialogId = GetDialogIdSelectedInTextEditor();
						if (dialogId == null) {
							dialogId = GetFirstDialogIdInTextEditor();
							JumpToDialogElement(dialogId);
						}
						DialogId = dialogId;
					}
					wixProject = GetProject();
				} catch (XmlException ex) {
					// Let the Wix designer loader try to load the XML and generate
					// an error message.
					DialogId = "InvalidXML";
					AddToErrorList(ex);
				}
			}
			base.LoadInternal(file, stream);
		}
		
		#region Switch...WithoutSaveLoad
		
		// These four methods prevent the text editor from doing a full save/load
		// cycle when switching views. This allows the text editor to keep its
		// selection and caret position.
		
		public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return (newView == this) || (newView == this.PrimaryViewContent);
		}
		
		public override void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			if (newView != this) {
				this.MergeAndUnloadDesigner();
			}
		}
		
		public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return (this.DesignerCodeFile != null) &&
				((oldView == this) || (oldView == this.PrimaryViewContent));
		}
		
		public override void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			if (oldView != this) {
				this.ReloadDesignerFromMemory();
			}
		}
		
		#endregion
		
		/// <summary>
		/// Gets the Wix document filename.
		/// </summary>
		public string DocumentFileName {
			get { return PrimaryFileName; }
		}
		
		/// <summary>
		/// Gets the wix project containing the document open in the designer.
		/// </summary>
		public WixProject Project {
			get { return wixProject; }
		}
		
		/// <summary>
		/// Gets the Wix document xml.
		/// </summary>
		public string GetDocumentXml()
		{
			return DesignerCodeFileContent;
		}
		
		public ITextEditor PrimaryViewContentTextEditor {
			get {
				return PrimaryViewContent.GetRequiredService<ITextEditor>();
			}
		}
		
		/// <summary>
		/// Gets or sets the dialog id currently being designed.
		/// </summary>
		public string DialogId {
			get { return dialogId; }
			set { dialogId = value; }
		}
		
		/// <summary>
		/// Finds the WixDialogDesigner in the current window's secondary views
		/// and switches to it.
		/// </summary>
		void OpenDesigner()
		{
			try {
				ignoreDialogIdSelectedInTextEditor = true;
				WorkbenchWindow.ActiveViewContent = this;
			} finally {
				ignoreDialogIdSelectedInTextEditor = false;
			}
		}
		
		/// <summary>
		/// From the current cursor position in the text editor determine the
		/// selected dialog id.
		/// </summary>
		string GetDialogIdSelectedInTextEditor()
		{
			ITextEditor textEditor = ActiveTextEditor;
			if (textEditor != null) {
				WixDocumentReader wixReader = new WixDocumentReader(textEditor.Document.Text);
				return wixReader.GetDialogId(textEditor.Caret.Line);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the first dialog id in the file.
		/// </summary>
		string GetFirstDialogIdInTextEditor()
		{
			ITextEditor textEditor = ActiveTextEditor;
			if (textEditor != null) {
				WixDocumentReader wixReader = new WixDocumentReader(textEditor.Document.Text);
				ReadOnlyCollection<string> ids = wixReader.GetDialogIds();
				if (ids.Count > 0) {
					return ids[0];
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the active text area control.
		/// </summary>
		ITextEditor ActiveTextEditor {
			get { return PrimaryViewContent.GetService<ITextEditor>(); }
		}
		
		void AddToErrorList(XmlException ex)
		{
			TaskService.ClearExceptCommentTasks();
			TaskService.Add(new SDTask(this.PrimaryFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error));
			SD.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
		}
		
		/// <summary>
		/// Cannot use ProjectService.CurrentProject since it is possible to switch
		/// to the designer without selecting the project.
		/// </summary>
		WixProject GetProject()
		{
			ISolution openSolution = ProjectService.OpenSolution;
			if (openSolution != null) {
				foreach (IProject project in openSolution.Projects) {
					if (project.IsFileInProject(this.PrimaryFileName)) {
						return project as WixProject;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Jumps to dialog element with corresponding dialog id. This is only used
		/// when the user opens a dialog from the Setup dialogs window or the cursor
		/// was not near a dialog when the designer was switched to. Jumping to the
		/// dialog selected ensures that when the user switches back, if they did
		/// not make any changes, then the dialog xml is displayed.
		/// </summary>
		void JumpToDialogElement(string dialogId)
		{
			try {
				if (dialogId != null) {
					ITextEditor textEditor = ActiveTextEditor;
					if (textEditor != null) {
						WixDocumentReader wixReader = new WixDocumentReader(textEditor.Document.Text);
						TextLocation location = wixReader.GetStartElementLocation("Dialog", dialogId);
						textEditor.JumpTo(location.Line, 1);
					}
				}
			} catch (XmlException) {
				// Ignore
			}
		}
		
		static SharpDevelopSideBar setupDialogControlsToolBox;
		
		public static SharpDevelopSideBar SetupDialogControlsToolBox {
			get {
				SD.MainThread.VerifyAccess();
				if (setupDialogControlsToolBox == null) {
					setupDialogControlsToolBox = new SharpDevelopSideBar();
					setupDialogControlsToolBox.Tabs.Add(SetupDialogControlsSideTab.CreateSideTab());
					setupDialogControlsToolBox.ActiveTab = setupDialogControlsToolBox.Tabs[0];
				}
				return setupDialogControlsToolBox;
			}
		}
		
		
		public override object ToolsContent {
			get { return SetupDialogControlsToolBox; }
		}
	}
}
