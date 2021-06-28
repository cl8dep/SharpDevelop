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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// GridView where the first column can be scrolled individually.
	/// </summary>
	public class CustomGridView : GridView
	{
		public static readonly DependencyProperty CurrentScrollPositionProperty =
			DependencyProperty.Register("CurrentScrollPosition", typeof(double), typeof(CustomGridView));
		
		public static readonly DependencyProperty MaxDesiredWidthProperty =
			DependencyProperty.Register("MaxDesiredWidth", typeof(double), typeof(CustomGridView),
			                            new FrameworkPropertyMetadata(OnMaxDesiredWidthPropertyChanged));
		
		static readonly DependencyPropertyKey MaxOverflowWidthPropertyKey =
			DependencyProperty.RegisterReadOnly("MaxOverflowWidth", typeof(double), typeof(CustomGridView),
			                                    new FrameworkPropertyMetadata(0.0));
		
		public static readonly DependencyProperty MaxOverflowWidthProperty = MaxOverflowWidthPropertyKey.DependencyProperty;
		
		static CustomGridView()
		{
			AllowsColumnReorderProperty.AddOwner(typeof(CustomGridView), new FrameworkPropertyMetadata(false));
		}
		
		static void OnMaxDesiredWidthPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			((CustomGridView)o).CalculateMaxOverflowWidth();
		}
		
		GridViewColumn primaryColumn;
		
		GridViewColumn PrimaryColumn {
			get { return primaryColumn; }
			set {
				if (primaryColumn != value) {
					INotifyPropertyChanged oldColumn = primaryColumn;
					if (oldColumn != null)
						oldColumn.PropertyChanged -= PrimaryColumnPropertyChanged;
					
					primaryColumn = value;
					
					INotifyPropertyChanged newColumn = primaryColumn;
					if (newColumn != null)
						newColumn.PropertyChanged += PrimaryColumnPropertyChanged;
				}
			}
		}
		
		void PrimaryColumnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ActualWidth") {
				CalculateMaxOverflowWidth();
			}
		}
		
		public CustomGridView()
		{
			this.Columns.CollectionChanged += delegate {
				this.PrimaryColumn = this.Columns.Count > 0 ? this.Columns[0] : null;
			};
		}
		
		/// <summary>
		/// The number of pixels that the first column is scrolled to the right.
		/// </summary>
		public double CurrentScrollPosition {
			get { return (double)GetValue(CurrentScrollPositionProperty); }
			set { SetValue(CurrentScrollPositionProperty, value); }
		}
		
		/// <summary>
		/// The maximum desired width of the first column.
		/// </summary>
		public double MaxDesiredWidth {
			get { return (double)GetValue(MaxDesiredWidthProperty); }
			set { SetValue(MaxDesiredWidthProperty, value); }
		}
		
		/// <summary>
		/// Gets the maximum overflow width (=MaxDesiredWidth - Columns[0].Width)
		/// </summary>
		public double MaxOverflowWidth {
			get { return (double)GetValue(MaxDesiredWidthProperty); }
			private set { SetValue(MaxOverflowWidthPropertyKey, value); }
		}
		
		void CalculateMaxOverflowWidth()
		{
			MaxOverflowWidth = Math.Max(0, MaxDesiredWidth - Columns[0].ActualWidth);
		}
		
		protected override object DefaultStyleKey {
			get {
				return CustomGridViewStyle;
			}
		}
		
		public static ResourceKey CustomGridViewStyle {
			get {
				return new ComponentResourceKey(typeof(CustomGridView), "Style");
			}
		}
	}
	
	[ContentProperty("Child")]
	public class CustomGridViewScrollableCell : FrameworkElement
	{
		public static readonly DependencyProperty CurrentScrollPositionProperty =
			DependencyProperty.Register("CurrentScrollPosition", typeof(double), typeof(CustomGridViewScrollableCell),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
		
		UIElementCollection uiElements;
		UIElement child;
		
		public CustomGridViewScrollableCell()
		{
			this.uiElements = new UIElementCollection(this, this);
		}
		
		public double CurrentScrollPosition {
			get { return (double)GetValue(CurrentScrollPositionProperty); }
			set { SetValue(CurrentScrollPositionProperty, value); }
		}
		
		public UIElement Child {
			get { return child; }
			set {
				if (child != null) {
					uiElements.Remove(child);
				}
				child = value;
				if (value != null) {
					uiElements.Add(value);
				}
			}
		}
		
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return uiElements.GetEnumerator();
			}
		}
		
		protected override Visual GetVisualChild(int index)
		{
			return uiElements[index];
		}
		
		protected override int VisualChildrenCount {
			get { return uiElements.Count; }
		}
		
		protected override Size MeasureOverride(Size availableSize)
		{
			child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
			if (child.DesiredSize.Width > 0) {
				ListView lv = FindListView();
				if (lv != null) {
					CustomGridView cgv = lv.View as CustomGridView;
					if (cgv != null) {
						cgv.MaxDesiredWidth = Math.Max(cgv.MaxDesiredWidth, child.DesiredSize.Width + 16);
					}
				}
			}
			return child.DesiredSize;
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			double pos = CurrentScrollPosition;
			child.Arrange(new Rect(-pos, 0, finalSize.Width + pos, finalSize.Height));
			return child.RenderSize;
		}
		
		ListView FindListView()
		{
			DependencyObject d;
			for (d = this; d != null && !(d is ListView); d = VisualTreeHelper.GetParent(d));
			return d as ListView;
		}
	}
}
