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

namespace Tools.Diagrams
{
	public interface IRectangle
	{
		/// <summary>
		/// The X position of the rectangular item, relative to its container.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
		float X { get; set; }
		
		/// <summary>
		/// The Y position of the rectangular item, relative to its container.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
		float Y { get; set; }
		
			
		/// <summary>
		/// The X position of the rectangular item, relative to the root container.
		/// </summary>
		float AbsoluteX { get; }
		
		/// <summary>
		/// The Y position of the rectangular item, relative to the root container.
		/// </summary>
		float AbsoluteY { get; }
		
		/// <summary>
		/// The visible width of the item.
		/// Layout managers such as ItemsStack change this value to define the width of
		/// the item.
		/// The value returned from this property must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float ActualWidth { get; set; }

		/// <summary>
		/// The visible height of the item.
		/// Layout managers such as ItemsStack change this value to define the height of
		/// the item.
		/// The value returned from this property must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float ActualHeight { get; set; }
		
		/// <summary>
		/// The defined width of the item.
		/// </summary>
		/// <remarks>
		/// A negative value means the the width is undefined, and is due to change by
		/// layout managers, such as ItemsStack. In that case, ActualWidth is set to the
		/// wanted value minus twice the border size.
		/// </remarks>
		float Width { get; set; }
		
		/// <summary>
		/// The defined height of the item.
		/// </summary>
		/// <remarks>
		/// A negative value means the the height is undefined, and is due to change by
		/// layout managers, such as ItemsStack. In that case, ActualHeight is set to the
		/// wanted value minus twice the border size.
		/// </remarks>
		float Height { get; set; }
		
		/// <summary>
		/// The distance between the item borders to its container's content borders.
		/// </summary>
		float Border { get; set; }

		/// <summary>
		/// The distance between the item borders to its content.
		/// </summary>
		float Padding { get; set; }
		
		/// <summary>
		/// The width of the item content disregarding defined of visible modifying values,
		/// such as border or width.
		/// The value returned must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>
		float GetAbsoluteContentWidth ();
		
		/// <summary>
		/// The height of the item content disregarding defined of visible modifying values,
		/// such as border or height.
		/// The value returned must a positive decimal or zero, and must never
		/// be NaN or infinite.
		/// </summary>		
		float GetAbsoluteContentHeight ();

		bool KeepAspectRatio { get; set; }
		
		/// <summary>
		/// If the implementing object is contained within another rectangle,
		/// this property points to the container.
		/// </summary>
		IRectangle Container { get; set; }
		
		bool IsHResizable { get; }
		bool IsVResizable { get; }
		
		event EventHandler AbsolutePositionChanged;
		event EventHandler WidthChanged;
		event EventHandler HeightChanged;
		event EventHandler ActualWidthChanged;
		event EventHandler ActualHeightChanged;
	}
}
