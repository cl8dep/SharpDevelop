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
using System.Windows.Media;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ColorableItemsTests
	{
		ColorableItems items;
		CustomizedHighlightingColor highlightingColor;
		FakeCustomizedHighlightingRules fakeHighlightingRules;
		FontsAndColorsItems fontsAndColorsItems;
		
		void CreateColorableItems()
		{
			highlightingColor = new CustomizedHighlightingColor();
			
			fakeHighlightingRules = new FakeCustomizedHighlightingRules();
			fakeHighlightingRules.Colors.Add(highlightingColor);
			
			fontsAndColorsItems = new FontsAndColorsItems(fakeHighlightingRules);
			
			items = new ColorableItems("Name", highlightingColor, fontsAndColorsItems);
		}
		
		[Test]
		public void Foreground_ForegroundHighlightingColorIsRed_ReturnsRed()
		{
			CreateColorableItems();
			highlightingColor.Foreground = Colors.Red;
			
			UInt32 foregroundOleColor = items.Foreground;
			Color foregroundColor = ColorHelper.ConvertToColor(foregroundOleColor);
			
			Assert.AreEqual(Colors.Red, foregroundColor);
		}
		
		[Test]
		public void Background_BackgroundHighlightingColorIsRed_ReturnsRed()
		{
			CreateColorableItems();
			highlightingColor.Background = Colors.Red;
			
			UInt32 backgroundOleColor = items.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(Colors.Red, backgroundColor);
		}
		
		[Test]
		public void Bold_HighlightingColorIsBold_ReturnsTrue()
		{
			CreateColorableItems();
			highlightingColor.Bold = true;
			
			bool bold = items.Bold;
			
			Assert.IsTrue(bold);
		}
		
		[Test]
		public void Bold_HighlightingColorIsNotBold_ReturnsFalse()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			bool bold = items.Bold;
			
			Assert.IsFalse(bold);
		}
		
		[Test]
		public void Bold_SetBoldToTrue_HighlightingColorIsChangedToBold()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			items.Bold = true;
			
			Assert.IsTrue(highlightingColor.Bold);
		}
		
		[Test]
		public void Foreground_ForegroundHighlightingColorIsNull_ReturnsWindowsTextSystemColor()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 foregroundOleColor = items.Foreground;
			Color foregroundColor = ColorHelper.ConvertToColor(foregroundOleColor);
			
			Assert.AreEqual(SystemColors.WindowTextColor, foregroundColor);
		}
		
		[Test]
		public void Background_BackgroundHighlightingColorIsNull_ReturnsWindowSystemColor()
		{
			CreateColorableItems();
			highlightingColor.Background = null;
			
			UInt32 backgroundOleColor = items.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(SystemColors.WindowColor, backgroundColor);
		}
				
		[Test]
		public void Foreground_SetForegroundToRed_HighlightingColorIsChangedToRed()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 oleColor = ColorHelper.ConvertToOleColor(System.Drawing.Color.Red);
			items.Foreground = oleColor;
			Color color = highlightingColor.Foreground.Value;
			
			Assert.AreEqual(Colors.Red, color);
		}
		
		[Test]
		public void Background_SetForegroundToGreen_HighlightingColorIsChangedToGreen()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 oleColor = ColorHelper.ConvertToOleColor(System.Drawing.Color.Green);
			items.Background = oleColor;
			Color color = highlightingColor.Background.Value;
			
			Assert.AreEqual(Colors.Green, color);
		}
		
		[Test]
		public void Foreground_SetForegroundToBlue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			
			items.Foreground = ColorHelper.ConvertToOleColor(System.Drawing.Color.Blue);
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Background_SetForegroundToBlue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			
			items.Background = ColorHelper.ConvertToOleColor(System.Drawing.Color.Blue);
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Background_BoldIsSetToTrue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			items.Bold = true;
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
	}
}
