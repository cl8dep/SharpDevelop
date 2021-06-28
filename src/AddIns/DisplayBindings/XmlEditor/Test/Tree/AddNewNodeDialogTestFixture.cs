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
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Tree
{
	/// <summary>
	/// Tests the AddXmlNodeDialog which is the base class for the
	/// AddElementDialog and the AddAttributeDialog classes. All the
	/// core logic for the two add dialogs is located in the AddXmlNodeDialog
	/// since their behaviour is the same.
	/// </summary>
	[TestFixture]
	public class AddNewNodeDialogTestFixture : SDTestFixtureBase
	{
		DerivedAddXmlNodeDialog dialog;
		ListBox namesListBox;
		string[] names;
		Button okButton;
		Button cancelButton;
		TextBox customNameTextBox;
		Label customNameTextBoxLabel;
		Panel bottomPanel;
		
		[SetUp]
		public void Init()
		{
			names = new string[] {"Chimpanzee", "Monkey"};
			dialog = new DerivedAddXmlNodeDialog(names);
			
			// Get the various dialog controls that are to be used in this
			// test fixture.
			bottomPanel = (Panel)dialog.Controls["bottomPanel"];
			namesListBox = (ListBox)dialog.Controls["namesListBox"];
			okButton = (Button)bottomPanel.Controls["okButton"];
			cancelButton = (Button)bottomPanel.Controls["cancelButton"];
			customNameTextBox = (TextBox)bottomPanel.Controls["customNameTextBox"];
			customNameTextBoxLabel = (Label)bottomPanel.Controls["customNameTextBoxLabel"];
		}
		
		[TearDown]
		public void TearDown()
		{
			dialog.Dispose();
		}
		
		[Test]
		public void TwoNamesAddedToListBox()
		{
			Assert.AreEqual(2, namesListBox.Items.Count);
		}
		
		[Test]
		public void NamesAddedToListBox()
		{
			Assert.Contains("Chimpanzee", namesListBox.Items);
			Assert.Contains("Monkey", namesListBox.Items);
		}
		
		[Test]
		public void OkButtonInitiallyDisabled()
		{
			Assert.IsFalse(okButton.Enabled);
		}
		
		[Test]
		public void OkButtonIsDialogAcceptButton()
		{
			Assert.AreSame(okButton, dialog.AcceptButton);
		}
		
		[Test]
		public void CancelButtonIsDialogCancelButton()
		{
			Assert.AreSame(cancelButton, dialog.CancelButton);
		}
		
		/// <summary>
		/// The dialog's Names property should not return any items
		/// when nothing is selected in the list box or added to the
		/// custom name text box.
		/// </summary>
		[Test]
		public void NoNamesInitiallySelected()
		{
			Assert.AreEqual(0, dialog.GetNames().Length);
		}
		
		[Test]
		public void NamesSelectedAfterSelectingOneListBoxItem()
		{
			namesListBox.SelectedIndex = 0;
			string[] expectedNames = new string[] {(string)namesListBox.Items[0]};
			string[] names = dialog.GetNames();
			
			Assert.AreEqual(expectedNames, names);
		}
		
		[Test]
		public void NamesSelectedAfterSelectingTwoListBoxItems()
		{
			namesListBox.SelectedIndices.Add(0);
			namesListBox.SelectedIndices.Add(1);
			string[] expectedNames = new string[] {(string)namesListBox.Items[0], (string)namesListBox.Items[1]};
			string[] names = dialog.GetNames();
			
			Assert.AreEqual(expectedNames, names);
		}
		
		/// <summary>
		/// Tests that the returned names from the dialog includes any
		/// text in the custom name text box. Also check that the text box's
		/// text is trimmed.
		/// </summary>
		[Test]
		public void NamesSelectedAfterEnteringCustomName()
		{
			string customName = " customname ";
			customNameTextBox.Text = customName;
			string[] expectedNames = new string[] {customName.Trim()};
			string[] names = dialog.GetNames();
			
			Assert.AreEqual(expectedNames, names);
		}
		
		[Test]
		public void OkButtonEnabledWhenListItemSelected()
		{
			namesListBox.SelectedIndex = 0;
			dialog.CallNamesListBoxSelectedIndexChanged();
			
			Assert.IsTrue(okButton.Enabled);
		}
		
		[Test]
		public void OkButtonEnabledWhenCustomNameEntered()
		{
			customNameTextBox.Text = "Custom";
			dialog.CallCustomNameTextBoxTextChanged();
			
			Assert.IsTrue(okButton.Enabled);
		}
		
		/// <summary>
		/// Tests that a custom name string that contains spaces does not
		/// cause the OK button to be enabled.
		/// </summary>
		[Test]
		public void OkButtonDisabledWhenEmptyCustomNameStringEntered()
		{
			customNameTextBox.Text = "  ";
			dialog.CallCustomNameTextBoxTextChanged();
			
			Assert.IsFalse(okButton.Enabled);
		}
		
		[Test]
		public void InvalidCustomNameEntered()
		{
			customNameTextBox.Text = "<element>";
			dialog.CallCustomNameTextBoxTextChanged();
			
			string error = dialog.ErrorText;
			string expectedError = null;
			
			try {
				XmlConvert.VerifyName(customNameTextBox.Text);
				Assert.Fail("XmlConvert.VerifyName should have failed.");
			} catch (Exception ex) {
				expectedError = ex.Message;
			}
			Assert.IsFalse(okButton.Enabled);
			Assert.AreEqual(expectedError, error);
		}
		
		[Test]
		public void CustomNameWithTwoColonCharsEntered()
		{
			customNameTextBox.Text = "xsl:test:this";
			dialog.CallCustomNameTextBoxTextChanged();
			
			string error = dialog.ErrorText;
			Assert.IsFalse(okButton.Enabled);
			Assert.IsTrue(error.Length > 0);
		}
		
		[Test]
		public void CustomNameWithOneColonCharEntered()
		{
			customNameTextBox.Text = "xsl:test";
			dialog.CallCustomNameTextBoxTextChanged();
			
			string error = dialog.ErrorText;
			Assert.IsTrue(okButton.Enabled);
			Assert.AreEqual(0, error.Length);
		}
		
		[Test]
		public void CustomNameWithOneColonCharAtStart()
		{
			customNameTextBox.Text = ":test";
			dialog.CallCustomNameTextBoxTextChanged();
			
			string error = dialog.ErrorText;
			Assert.IsFalse(okButton.Enabled);
			Assert.IsTrue(error.Length > 0);
		}
		
		[Test]
		public void ErrorClearedAfterTextChanged()
		{
			InvalidCustomNameEntered();
			customNameTextBox.Text = "element";
			dialog.CallCustomNameTextBoxTextChanged();
			
			Assert.IsTrue(okButton.Enabled);
			Assert.AreEqual(0, dialog.ErrorText.Length);
		}
		
		[Test]
		public void StartPositionIsCenterParent()
		{
			Assert.AreEqual(FormStartPosition.CenterParent, dialog.StartPosition);
		}
		
		[Test]
		public void OkButtonDialogResult()
		{
			Assert.AreEqual(DialogResult.OK, okButton.DialogResult);
		}
		
		[Test]
		public void CancelButtonDialogResult()
		{
			Assert.AreEqual(DialogResult.Cancel, cancelButton.DialogResult);
		}
		
		[Test]
		public void ShowInTaskBar()
		{
			Assert.IsFalse(dialog.ShowInTaskbar);
		}
		
		[Test]
		public void MinimizeBox()
		{
			Assert.IsFalse(dialog.MinimizeBox);
		}
		
		[Test]
		public void MaximizeBox()
		{
			Assert.IsFalse(dialog.MaximizeBox);
		}
		
		[Test]
		public void ShowIcon()
		{
			Assert.IsFalse(dialog.ShowIcon);
		}
		
		[Test]
		public void SetCustomNameLabel()
		{
			dialog.CustomNameLabelText = "test";
			Assert.AreEqual("test", dialog.CustomNameLabelText);
			Assert.AreEqual("test", customNameTextBoxLabel.Text);
		}
		
		[Test]
		public void RightToLeftConversion()
		{
			try {
				RightToLeftConverter.IsRightToLeft = true;
				using (AddXmlNodeDialog dialog = new AddXmlNodeDialog()) {
					Assert.AreEqual(RightToLeft.Yes, dialog.RightToLeft);
				}
			} finally {
				RightToLeftConverter.IsRightToLeft = false;
			}
		}
		
		/// <summary>
		/// Check that the list box is not on the form when there are no
		/// names passed to the constructor of AddXmlNodeDialog.
		/// </summary>
		[Test]
		public void NoListBoxShownWhenNoNames()
		{
			using (AddXmlNodeDialog dialog = new AddXmlNodeDialog(new string[0])) {
				Size expectedClientSize = this.bottomPanel.Size;
				Size expectedMinSize = dialog.Size;
				
				Panel bottomPanel = (Panel)dialog.Controls["bottomPanel"];
			
				Assert.IsFalse(dialog.Controls.ContainsKey("namesListBox"));
				Assert.AreEqual(DockStyle.Fill, bottomPanel.Dock);
				Assert.AreEqual(expectedClientSize, dialog.ClientSize);
				Assert.AreEqual(expectedMinSize, dialog.MinimumSize);
			}
		}
	}
}
