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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.IO;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using ICSharpCode.Data.EDMDesigner.Core.Windows.EDMWizard;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.DisplayBinding
{
	public class EDMDesignerViewContent : AbstractViewContent, IHasPropertyContainer, IToolsHost, IEDMDesignerChangeWatcherObserver
	{
		private ScrollViewer _scrollViewer = new ScrollViewer() {
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto
		};
		private DesignerCanvas _designerCanvas = null;
		private PropertyContainer _propertyContainer = new PropertyContainer();
		private EDMView _edmView = null;
		private object _selection = null;

		public object Selection {
			get { return _selection; }
			set {
				if (_selection == null)
					_propertyContainer.Clear();
				else
					_propertyContainer.SelectedObject = value;

				_selection = value;
			}
		}

		public Window Window {
			get { return Application.Current.MainWindow; }
		}

		public EDMView EDMView {
			get { return _edmView; }
		}
		
		public override object Control {
			get { return _scrollViewer; }
		}
		
		public DesignerCanvas DesignerCanvas {
			get { return _designerCanvas; }
		}
		
		public EDMDesignerViewContent(OpenedFile primaryFile)
			: base(primaryFile)
		{
			if (primaryFile == null)
				throw new ArgumentNullException("primaryFile");
			
			primaryFile.ForceInitializeView(this); // call Load()

			EDMDesignerChangeWatcher.AddEDMDesignerViewContent(this);
		}

		public override void Load(OpenedFile file, Stream stream)
		{
			Debug.Assert(file == this.PrimaryFile);

			EDMDesignerChangeWatcher.Init = true;

			// Load EDMX from stream
			XElement edmxElement = null;
			Action<XElement> readMoreAction = edmxElt => edmxElement = edmxElt;
			_edmView = new EDMView(stream, readMoreAction);
			
			// If EDMX is empty run EDM Wizard
			if (_edmView.EDM.IsEmpty) {
				edmxElement = null;
				
				string ns = String.Empty;
				if (ProjectService.CurrentProject == null) {
					ns = ICSharpCode.Core.MessageService.ShowInputBox("EDMDesigner","Enter NameSpace","DefaultNamespace");
				} else {
					ns = ProjectService.CurrentProject.RootNamespace;
				}
				
				EDMWizardWindow wizard = RunWizard(file, ns);

				if (wizard.DialogResult == true)
					_edmView = new EDMView(wizard.EDMXDocument, readMoreAction);
				else
					throw new WizardCancelledException();
			}

			// Load or generate DesignerView and EntityTypeDesigners
			EntityTypeDesigner.Init = true;

			XElement designerViewsElement = null;

			if (edmxElement == null || (designerViewsElement = EDMXIO.ReadSection(edmxElement, EDMXIO.EDMXSection.DesignerViews)) == null) {
				designerViewsElement = DesignerIO.GenerateNewDesignerViewsFromCSDLView(_edmView);
			}

			if (edmxElement != null && designerViewsElement != null)
				DesignerIO.Read(_edmView, designerViewsElement, entityType => new EntityTypeDesigner(entityType), complexType => new ComplexTypeDesigner(complexType));

			EntityTypeDesigner.Init = false;

			// Gets the designer canvas
			_designerCanvas = DesignerCanvas.GetDesignerCanvas(this, _edmView, _edmView.DesignerViews.FirstOrDefault());
			_scrollViewer.Content = _designerCanvas;
			
			// Register CSDL of EDMX in CSDL DatabaseTreeView
			CSDLViews.Add(_edmView.CSDL);
			
			EDMDesignerChangeWatcher.Init = false;
		}
		
		ObservableCollection<CSDLView> CSDLViews {
			get { return CSDLDatabaseTreeViewAdditionalNode.Instance.CSDLViews; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			EDMXIO.WriteXDocument(_edmView).Save(stream);
		}
		
		private EDMWizardWindow RunWizard(OpenedFile file, string projectStandardNamespace)
		{
			var wizard = new EDMWizardWindow(file, projectStandardNamespace);
			wizard.Owner = Application.Current.MainWindow;
			wizard.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			wizard.ShowDialog();
			
			return wizard;
		}

		public override void Dispose()
		{
			if (_edmView != null) {
				CSDLViews.Remove(_edmView.CSDL);
			}

			EDMDesignerChangeWatcher.RemoveEDMDesignerViewContent(this);
		}

		public void ShowMappingTab(IUIType uiType)
		{
		}

		#region IHasPropertyContainer

		public PropertyContainer PropertyContainer {
			get { return _propertyContainer; }
		}

		#endregion

		#region IToolsHost

		object IToolsHost.ToolsContent {
			get { return null; }
		}

		#endregion

		#region IEDMDesignerChangeWatcherObserver Member

		public bool ObjectChanged(object changedObject)
		{
			foreach (DesignerView designerView in _edmView.DesignerViews) {
				foreach (ITypeDesigner uiType in designerView) {
					if (uiType == changedObject || uiType.UIType.BusinessInstance == changedObject) {
						PrimaryFile.IsDirty = true;
						return true;
					}
				}
			}
			return false;
		}

		#endregion
	}
}
