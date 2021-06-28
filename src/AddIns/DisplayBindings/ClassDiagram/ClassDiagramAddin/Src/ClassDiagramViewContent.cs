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
using System.Xml;

using ClassDiagram;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ClassDiagramAddin
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class ClassDiagramViewContent : AbstractViewContent
	{
		private IProjectContent projectContent;
		private ClassCanvas canvas = new ClassCanvas();
		private ToolStrip toolstrip;
		
		public ClassDiagramViewContent (OpenedFile file) : base(file)
		{
			this.TabPageText = "Class Diagram";
			
			canvas.LayoutChanged += HandleLayoutChange;
			ParserService.ParseInformationUpdated += OnParseInformationUpdated;
			toolstrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/ViewContent/ClassDiagram/Toolbar");
			toolstrip.GripStyle = ToolStripGripStyle.Hidden;
			toolstrip.Stretch = true;
			canvas.Controls.Add(toolstrip);
			canvas.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/ViewContent/ClassDiagram/ContextMenu");
		}
		
		public override object Control {
			get { return canvas; }
		}

		public override void Load(OpenedFile file, Stream stream)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			projectContent = ParserService.GetProjectContent(ProjectService.CurrentProject);
			canvas.LoadFromXml(doc, projectContent);
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.Encoding = System.Text.Encoding.UTF8;
			
			XmlWriter xw = XmlWriter.Create(stream, settings);
			canvas.WriteToXml().WriteTo(xw);
			xw.Close();
		}

		void OnParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("ClassDiagramViewContent.OnParseInformationUpdated");

			if (e == null) return;
			if (e.NewCompilationUnit == null) return;
			if (e.NewCompilationUnit.ProjectContent == null) return;
			if (e.NewCompilationUnit.ProjectContent.Classes == null) return;
			if (e.NewCompilationUnit.ProjectContent != projectContent) return;
			//TODO - this is a wrong way to handle changed parse informtation.
			//       the correct way is to mark removed classes as missing, and to
			//       update changed classes that exist in the diagram.
			/*
			List<CanvasItem> addedItems = new List<CanvasItem>();
			foreach (IClass ct in e.CompilationUnit.ProjectContent.Classes)
			{
				if (!canvas.Contains(ct))
				{
					ClassCanvasItem item = ClassCanvas.CreateItemFromType(ct);
					canvas.AddCanvasItem(item);
					addedItems.Add(item);
				}
			}
			
			WorkbenchSingleton.SafeThreadAsyncCall<ICollection<CanvasItem>>(PlaceNewItems, addedItems);
			
			foreach (CanvasItem ci in canvas.GetCanvasItems())
			{
				ClassCanvasItem cci = ci as ClassCanvasItem;
				if (cci != null)
				{
					if (!e.CompilationUnit.ProjectContent.Classes.Contains(cci.RepresentedClassType))
						canvas.RemoveCanvasItem(cci);
				}
			}
			*/
		}

		private void PlaceNewItems (ICollection<CanvasItem> items)
		{
			float minX = float.MaxValue, minY = float.MaxValue;
			float maxX = float.MinValue, maxY = float.MinValue;
			foreach (CanvasItem ci in canvas.GetCanvasItems())
			{
				minX = Math.Min(ci.X, minX);
				minY = Math.Min(ci.Y, minY);
				maxX = Math.Max(ci.X + ci.ActualWidth, maxX);
				maxY = Math.Max(ci.Y + ci.ActualHeight, maxY);
			}
			
			float x = 20;
			float y = maxY + 20;
			float max_h = 0;
			
			foreach (CanvasItem ci in items)
			{
				ci.X = x;
				ci.Y = y;
				x += ci.Width + 20;
				if (ci.Height > max_h)
					max_h = ci.Height;
				if (x > 1000)
				{
					x = 20;
					y += max_h + 20;
					max_h = 0;
				}
			}
		}
		
		public override void Dispose()
		{
			ParserService.ParseInformationUpdated -= OnParseInformationUpdated;
			canvas.Dispose();
			base.Dispose();
		}
		
		protected void HandleLayoutChange (object sender, EventArgs args)
		{
			this.PrimaryFile.MakeDirty();
		}
	}
}
