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
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Description of AnalysisProjectOptionsTreeNodes.
	/// </summary>
	public class BaseTree:SharpTreeNode
	{
		private int index;
		
		public BaseTree()
		{
			RuleState = new ObservableCollection<Tuple<ImageSource,string>>();
			
			Icon icon = SystemIcons.Warning;
			ImageSource imageSource = ToImageSource(icon);
			this.RuleState.Add(Tuple.Create<ImageSource,string>(imageSource,
			                                                    ResourceService.GetString("Global.WarningText")));
			
			icon = SystemIcons.Error;
			imageSource = ToImageSource(icon);
			this.RuleState.Add(Tuple.Create<ImageSource,string>(imageSource,
			                                                    ResourceService.GetString("Global.ErrorText")));
			
		}
		
		
		private static ImageSource ToImageSource( Icon icon)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hBitmap = bitmap.GetHbitmap();
			
			ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
			
			return wpfBitmap;
		}

		
		public ObservableCollection<Tuple<ImageSource,string>> RuleState {get;set;}
		
		private Tuple<Icon,string> selectedItem;
		
		public Tuple<Icon, string> SelectedNode {
			get { return selectedItem; }
			set { selectedItem = value; }
		}
		
		
		public virtual int Index {
			get { return index; }
			set {
				index = value;
			}
		}
	}

	
	
	public class CategoryTreeNode : BaseTree
	{
		internal FxCopCategory category;
		private Tuple<ImageSource,string>  mixedModeTuple;
		
		public CategoryTreeNode(FxCopCategory category):base()
		{
			
			this.category = category;
			foreach (FxCopRule rule in category.Rules) {
				this.Children.Add(new RuleTreeNode(rule));
			}
		}
		

		public override bool IsCheckable {
			get { return true; }
		}
		
		
		public override object Text {
			get { return category.DisplayName; }
		}
		

		public override int Index {
			get { return base.Index; }
			set {
				if (value != base.Index) {
					base.Index = value;
					RaisePropertyChanged("Index");
				}
			}
		}
		
		
		private void AddMixedMode()
		{
			if (!RuleState.Contains(mixedModeTuple)) {
				var image = PresentationResourceService.GetBitmapSource("Icons.16x16.ClosedFolderBitmap");
				mixedModeTuple = Tuple.Create<ImageSource,string>(image,
				                                                  StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.WarningErrorMixed}"));
				RuleState.Add(mixedModeTuple);
				Index = RuleState.Count -1;
				base.RaisePropertyChanged("Index");
			}
		}
		
		
		private void RemoveMixedMode()
		{
			if (mixedModeTuple != null) {
				if (RuleState.Contains(mixedModeTuple)) {
					RuleState.Remove(mixedModeTuple);
					mixedModeTuple = null;
					base.RaisePropertyChanged("Index");
				}
			}
		}
		
		
		public void CheckMode ()
		{
			var state = ErrorState;
			
			if (state == 0) {
				RemoveMixedMode();
				Index = 0;
			} else if (state == 1) {
				RemoveMixedMode();
				Index = 1;
			} else {
				AddMixedMode();	
			}
		}
		
		
		internal int ErrorState {
			get {
				bool allWarn = true;
				bool allErr = true;
				foreach (RuleTreeNode tn in Children) {
					if (tn.isError)
						allWarn = false;
					else
						allErr = false;
				}
				if (allErr)
					return 1;
				if (allWarn) {
					return 0;
				}
				return -1;
			}
		}
	}
	
	
	public class RuleTreeNode :BaseTree
	{
		internal FxCopRule rule;
		bool error;
		
		internal bool isError {
			get { return error; }
			set {error = value;}
		}
		
		
		public RuleTreeNode(FxCopRule rule):base()
		{
			this.rule = rule;
		}
		
		
		public override bool IsCheckable {
			get { return true; }
		}
		
		
		public override object Text {
			get { return rule.CheckId + " : " + rule.DisplayName; }
		}
		
		
		public string Identifier {
			get {
				return rule.CategoryName + "#" + rule.CheckId;
			}
		}
		
		
		public override int Index {
			get { return base.Index; }
			set {
				if (base.Index != value) {
					isError = value == 1;
					base.Index = value;
					RaisePropertyChanged("Index");
				}
			}
		}
	}
	
	
	public class MessageNode : SharpTreeNode
	{
		private string message;
		
		public MessageNode (string message)
		{
			this.message = message;
		}
		
		public override object Text {
			get { return message; }
		}
	}
}
