﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Interaction logic for OpenFromGacDialog.xaml
	/// </summary>
	internal partial class OpenFromGacDialog : Window
	{
		ObservableCollection<GacEntry> gacEntries = new ObservableCollection<GacEntry>();
		ObservableCollection<GacEntry> filteredEntries = new ObservableCollection<GacEntry>();
		Predicate<GacEntry> filterMethod = _ => true;
		volatile bool cancelFetchThread;

		public OpenFromGacDialog()
		{
			InitializeComponent();
			FormLocationHelper.ApplyWindow(this, "ICSharpCode.SharpDevelop.Dom.OpenFromGacDialog.Bounds", true);
			listView.ItemsSource = filteredEntries;
			SortableGridViewColumn.SetCurrentSortColumn(listView, nameColumn);
			SortableGridViewColumn.SetSortDirection(listView, ColumnSortDirection.Ascending);

			new Thread(new ThreadStart(FetchGacContents)).Start();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			cancelFetchThread = true;
		}

		#region Fetch Gac Contents
		sealed class GacEntry
		{
			readonly DomAssemblyName r;
			readonly string fileName;
			string formattedVersion;

			public GacEntry(DomAssemblyName r, string fileName)
			{
				this.r = r;
				this.fileName = fileName;
			}

			public string FullName {
				get { return r.FullName; }
			}

			public string ShortName {
				get { return r.ShortName; }
			}

			public string FileName {
				get { return fileName; }
			}

			public Version Version {
				get { return r.Version; }
			}

			public string FormattedVersion {
				get {
					if (formattedVersion == null)
						formattedVersion = Version.ToString();
					return formattedVersion;
				}
			}

			public string Culture {
				get { return r.Culture; }
			}

			public string PublicKeyToken {
				get { return r.PublicKeyToken; }
			}

			public override string ToString()
			{
				return r.FullName;
			}
		}

		void FetchGacContents()
		{
			IGlobalAssemblyCacheService gacService = SD.GetService<IGlobalAssemblyCacheService>();
			HashSet<string> fullNames = new HashSet<string>();
			UpdateProgressBar(pg => { pg.Visibility = System.Windows.Visibility.Visible; pg.IsIndeterminate = true; });
			var list = gacService.Assemblies.TakeWhile(_ => !cancelFetchThread).ToList();
			UpdateProgressBar(pg => { pg.IsIndeterminate = false; pg.Maximum = list.Count; });
			foreach (var r in list) {
				if (cancelFetchThread)
					break;
				if (fullNames.Add(r.FullName)) { // filter duplicates
					var file = gacService.FindAssemblyInNetGac(r);
					if (file != null) {
						var entry = new GacEntry(r, file);
						UpdateProgressBar(pg => { pg.Value = pg.Value + 1; AddNewEntry(entry); });
					}
				}
			}
			UpdateProgressBar(pg => { pg.Visibility = System.Windows.Visibility.Hidden; });
		}

		void UpdateProgressBar(Action<ProgressBar> updateAction)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => updateAction(gacReadingProgressBar)));
		}

		void AddNewEntry(GacEntry entry)
		{
			gacEntries.Add(entry);
			if (filterMethod(entry))
				filteredEntries.Add(entry);
		}
		#endregion

		void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string filterString = filterTextBox.Text.Trim();
			if (filterString.Length == 0)
				filterMethod = _ => true;
			else {
				var elements = filterString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				filterMethod = entry => elements.All(el => Contains(entry.FullName, el) || Contains(entry.FormattedVersion, el));
			}

			filteredEntries.Clear();
			filteredEntries.AddRange(gacEntries.Where(entry => filterMethod(entry)));
		}

		static bool Contains(string s, string subString)
		{
			return s.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			okButton.IsEnabled = listView.SelectedItems.Count > 0;
		}

		void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// Double click in list = click on OK button
			ConfirmCurrentSelection();
		}

		void OKButton_Click(object sender, RoutedEventArgs e)
		{
			ConfirmCurrentSelection();
		}
		
		void ConfirmCurrentSelection()
		{
			this.DialogResult = true;
			Close();
		}
		
		public string[] SelectedFileNames {
			get {
				return listView.SelectedItems.OfType<GacEntry>().Select(e => e.FileName).ToArray();
			}
		}
	}
}