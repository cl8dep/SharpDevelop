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
using System.Collections.ObjectModel;
using System.Windows;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn.Options
{
	/// <summary>
	/// Interaction logic for ChooseExceptionsDialog.xaml
	/// </summary>
	public partial class ChooseExceptionsDialog : Window
	{
		public ChooseExceptionsDialog(IEnumerable<ExceptionFilterEntry> entries)
		{
			InitializeComponent();
			
			FormLocationHelper.ApplyWindow(this, "Debugger.ChooseExceptionsDialog", true);
			
			ExceptionFilterList = new ObservableCollection<ExceptionFilterEntry>(entries);
			dataGrid.ItemsSource = ExceptionFilterList;
		}

		public IList<ExceptionFilterEntry> ExceptionFilterList { get; set; }
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}