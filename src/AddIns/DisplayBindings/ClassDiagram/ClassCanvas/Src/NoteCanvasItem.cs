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

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using System.Globalization;
using System.Windows.Forms;

namespace ClassDiagram
{
	//TODO - complete note item
	public class NoteCanvasItem : CanvasItem
	{
		private string note = "<Text editing not implemented yet.>";
		private TextBox editBox = new TextBox();
		
		public NoteCanvasItem()
		{
			editBox.BackColor = Color.LightYellow;
			editBox.BorderStyle = BorderStyle.None;
			editBox.Multiline = true;
		}
		
		public string Note
		{
			get { return note; }
			set { note = value; }
		}
		
		public override void DrawToGraphics (Graphics graphics)
		{
			if (graphics == null) return;
			
			// Draw Shadow
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + ActualWidth, Y + 3, 4, ActualHeight);
			graphics.FillRectangle(CanvasItem.ShadowBrush, X + 4, Y + ActualHeight, ActualWidth - 4, 3);
			
			// Draw Note Area
			graphics.FillRectangle(Brushes.LightYellow, X, Y, ActualWidth, ActualHeight);
			graphics.DrawRectangle(Pens.Olive, X, Y, ActualWidth, ActualHeight);
			
			// Draw Note Text
			RectangleF rect = new RectangleF (X + 5, Y + 5, ActualWidth - 10, ActualHeight - 10);
			graphics.DrawString(note, MemberFont, Brushes.Black, rect);
			
			base.DrawToGraphics(graphics);
		}
		
		protected override bool DragAreaHitTest(PointF pos)
		{
			return (pos.X > X && pos.Y > Y && pos.X < X + ActualWidth && pos.Y < Y + ActualHeight);
		}
		
		public override Control GetEditingControl()
		{
			editBox.Width = (int) Width - 8;
			editBox.Height = (int) Height - 8;
			editBox.Left = (int) AbsoluteX + 4;
			editBox.Top = (int) AbsoluteY + 4;
			return editBox;
		}
		
		public override bool StartEditing()
		{
			editBox.Text = note;
			return true;
		}
		
		public override void StopEditing()
		{
			note = editBox.Text;
			if (editBox.Parent != null)
				editBox.Parent.Controls.Remove(editBox);
		}
		
		#region Storage
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Comment");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("CommentText", Note);
		}
		
		protected override void FillXmlPositionElement(XmlElement position, XmlDocument document)
		{
			base.FillXmlPositionElement(position, document);
			position.SetAttribute("Height", (Height/100).ToString(CultureInfo.InvariantCulture));
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			Note = navigator.GetAttribute("CommentText", "");
		}
		
		protected override void ReadXmlPositionElement(XPathNavigator navigator)
		{
			base.ReadXmlPositionElement(navigator);
			Height = 100 * float.Parse(navigator.GetAttribute("Height", ""), CultureInfo.InvariantCulture);
		}
		
		#endregion
		
		#region Geometry
		public override float Width
		{
			set { base.Width = Math.Max (value, 40.0f); }
		}
		
		public override float Height
		{
			set { base.Height = Math.Max (value, 40.0f); }
		}
		#endregion
	}
}
