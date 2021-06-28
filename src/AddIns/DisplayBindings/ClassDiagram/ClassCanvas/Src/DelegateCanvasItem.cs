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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using System.Globalization;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	// TODO - perhaps abandon this base class and implement styles mechanism instead?
	public class EnumDelegateCanvasItem : ClassCanvasItem
	{
		public EnumDelegateCanvasItem (IClass ct) : base (ct) {}

		private InteractiveItemsStack items = new InteractiveItemsStack();
		
		public InteractiveItemsStack Items {
			get { return items; }
		}

		protected override DrawableRectangle InitContentBackground()
		{
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				return new DrawableRectangle(ContentBG, null, 0, 0, radius, 0);
			}
			else
				return new DrawableRectangle(ContentBG, null, 0, 0, 0, 0);
		}
		
		protected override DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack decorator = new DrawableItemsStack();
			decorator.OrientationAxis = Axis.X;
			DrawableRectangle rect;
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				rect = new DrawableRectangle(TitleBG, null, 0, 0, 0, radius);
			}
			else
			{
				rect = new DrawableRectangle(TitleBG, null, 0, 0, 0, 0);
			}
			
			rect.Width = 20;
			
			decorator.Add(rect);
			decorator.Add(base.InitContentContainer(items));
			return decorator;
		}
		
		protected override IDrawableRectangle InitContent()
		{
			items.Border = 5;
			items.OrientationAxis = Axis.Y;
			return items;
		}
	}
	
	public class DelegateCanvasItem : EnumDelegateCanvasItem
	{
		public DelegateCanvasItem (IClass ct) : base (ct) {}
		
		static Color titlesBG = Color.FromArgb(255, 237, 219, 221);
		protected override Color TitleBackground
		{
			get { return titlesBG; }
		}
		
		static Brush contentBG = new SolidBrush(Color.FromArgb(255, 247, 240, 240));
		protected override Brush ContentBG
		{
			get { return contentBG; }
		}

		protected override void PrepareMembersContent()
		{
			Items.Clear();
			IMethod invokeMethod = RepresentedClassType.SearchMember("Invoke", RepresentedClassType.ProjectContent.Language) as IMethod;
			IAmbience ambience = GetAmbience();
			foreach (IParameter par in invokeMethod.Parameters)
			{
				TextSegment ts = new TextSegment(Graphics, par.Name  + " : " + ambience.Convert(par.ReturnType), MemberFont, true);
				Items.Add(ts);
			}
		}
		
		// TODO - remove - for debug only.
		public override void DrawToGraphics(Graphics graphics)
		{
			base.DrawToGraphics(graphics);
		}
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Delegate");
		}
	}
	
	public delegate TestEnum TestDelegate (int num, string str);
}
