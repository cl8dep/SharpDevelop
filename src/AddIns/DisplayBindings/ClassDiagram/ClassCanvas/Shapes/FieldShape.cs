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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class FieldShape : MethodShape
	{
		static Brush brickBrush1 = new SolidBrush(Color.FromArgb(255, 0, 192, 192));
		static Brush brickBrush2 = new SolidBrush(Color.FromArgb(255, 0, 064, 064));
		static Brush brickBrush3 = new SolidBrush(Color.FromArgb(255, 0, 128, 128));
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform(17.0f, 0.0f);
			graphics.ScaleTransform(-1.0f, 1.0f);
			MethodShape.DrawBrick(graphics, brickBrush1, brickBrush2, brickBrush3);
			graphics.Restore(state);
			graphics.FillRectangle(Brushes.Gray, 1.0f, 4.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 0.0f, 6.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 2.0f, 8.5f, 3.5f, 0.5f);
		}
	}
}
