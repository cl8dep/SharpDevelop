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
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using MSHelpSystem.Core;
using MSHelpSystem.Helper;

namespace MSHelpSystem
{
	public partial class Help3OptionsPanel : OptionPanel
	{
		public Help3OptionsPanel()
		{
			InitializeComponent();
			Load();
		}

		void Load()
		{
			HelpLibraryAgent.Start();
			DataContext = Help3Service.Items;
			if (Help3Service.Items.Count > 0)
				groupBox1.Header = string.Format("{0} ({1})", StringParser.Parse("${res:AddIns.HelpViewer.InstalledHelpCatalogsLabel}"), Help3Service.Items.Count);
			if (Help3Service.ActiveCatalog != null)
				help3Catalogs.SelectedValue = Help3Service.ActiveCatalog.ShortName;
			help3Catalogs.IsEnabled = (Help3Service.Items.Count > 1 && Help3Service.Config.OfflineMode);
			onlineMode.IsChecked = !Help3Service.Config.OfflineMode;
			externalHelp.IsChecked = Help3Service.Config.ExternalHelp;
			onlineMode.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
			offlineMode.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
			externalHelp.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
		}

		void Help3CatalogsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string item = (string)help3Catalogs.SelectedValue;
			if (!string.IsNullOrEmpty(item)) {
				Help3Service.ActiveCatalogId = item;
			}
		}

		void Help3OfflineModeClicked(object sender, RoutedEventArgs e)
		{
			Help3Service.Config.OfflineMode = true;
			help3Catalogs.IsEnabled = (help3Catalogs.Items.Count > 1 && Help3Service.Config.OfflineMode);
			LoggingService.Info("HelpViewer: Help mode set to \"offline\"");
		}

		void Help3OnlineModeClicked(object sender, RoutedEventArgs e)
		{
			Help3Service.Config.OfflineMode = false;
			help3Catalogs.IsEnabled = false;
			LoggingService.Info("HelpViewer: Help mode set to \"online\"");
		}

		void Help3UseExternalHelpClicked(object sender, RoutedEventArgs e)
		{
			Help3Service.Config.ExternalHelp = (bool)externalHelp.IsChecked;
			LoggingService.Info(string.Format("HelpViewer: External help viewer {0}", (Help3Service.Config.ExternalHelp)?"enabled":"disabled"));
		}

		public override bool SaveOptions()
		{
			Help3Service.SaveHelpConfiguration();
			return true;
		}
	}
}
