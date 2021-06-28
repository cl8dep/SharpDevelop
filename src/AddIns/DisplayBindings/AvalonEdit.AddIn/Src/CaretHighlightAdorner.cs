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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Animated rectangle around the caret.
	/// </summary>
	sealed class CaretHighlightAdorner : Adorner
	{
		Pen pen;
		RectangleGeometry geometry;
		
		public CaretHighlightAdorner(TextArea textArea)
			: base(textArea.TextView)
		{
			Rect min = textArea.Caret.CalculateCaretRectangle();
			min.Offset(-textArea.TextView.ScrollOffset);
			
			Rect max = min;
			double size = Math.Max(min.Width, min.Height) * 0.25;
			max.Inflate(size, size);
			
			pen = new Pen(TextBlock.GetForeground(textArea.TextView).Clone(), 1);
			
			geometry = new RectangleGeometry(min, 2, 2);
			geometry.BeginAnimation(RectangleGeometry.RectProperty, new RectAnimation(min, max, new Duration(TimeSpan.FromMilliseconds(300))) { AutoReverse = true });
			pen.Brush.BeginAnimation(Brush.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200))) { BeginTime = TimeSpan.FromMilliseconds(450) });
		}
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawGeometry(null, pen, geometry);
		}
	}
}
