// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Dialog for adding a new configuration or platform to a solution or project.
	/// </summary>
	internal partial class AddNewConfigurationDialog
	{
		Predicate<string> checkNameValid;
		
		public AddNewConfigurationDialog(bool solution, bool editPlatforms,
		                                 IEnumerable<string> availableSourceItems,
		                                 Predicate<string> checkNameValid)
		{
			this.checkNameValid = checkNameValid;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			
			createInAllCheckBox.Visible = solution;
			nameTextBox.TextChanged += delegate {
				okButton.Enabled = nameTextBox.TextLength > 0;
			};
			copyFromComboBox.Items.Add(StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EmptyItem}"));
			copyFromComboBox.Items.AddRange(availableSourceItems.ToArray());
			copyFromComboBox.SelectedIndex = 0;
			
			if (solution) {
				if (editPlatforms)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddSolutionPlatform}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddSolutionConfiguration}");
			} else {
				if (editPlatforms)
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddProjectPlatform}");
				else
					this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.AddProjectConfiguration}");
			}
		}
		
		public bool CreateInAllProjects {
			get {
				return createInAllCheckBox.Checked;
			}
		}
		
		public string CopyFrom {
			get {
				if (copyFromComboBox.SelectedIndex <= 0)
					return null;
				else
					return copyFromComboBox.SelectedItem.ToString();
			}
		}
		
		public string NewName {
			get {
				return nameTextBox.Text;
			}
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (checkNameValid(nameTextBox.Text)) {
				this.DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
