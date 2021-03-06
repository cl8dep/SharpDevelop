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
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Project
{
	[Obsolete("XML Forms are obsolete")]
	public abstract class ConfigurationGuiBinding
	{
		ConfigurationGuiHelper helper;
		string property;
		bool treatPropertyValueAsLiteral = true;
		
		public MSBuildBasedProject Project {
			get {
				return helper.Project;
			}
		}
		
		public ConfigurationGuiHelper Helper {
			get {
				return helper;
			}
			internal set {
				helper = value;
			}
		}
		
		public string Property {
			get {
				return property;
			}
			internal set {
				property = value;
			}
		}
		
		/// <summary>
		/// Gets if the value should be evaluated when loading the property and escaped
		/// when saving. The default value is true.
		/// </summary>
		public bool TreatPropertyValueAsLiteral {
			get { return treatPropertyValueAsLiteral; }
			set { treatPropertyValueAsLiteral = value; }
		}
		
		PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base;
		
		public PropertyStorageLocations DefaultLocation {
			get {
				return defaultLocation;
			}
			set {
				defaultLocation = value;
			}
		}
		
		PropertyStorageLocations location = PropertyStorageLocations.Unknown;
		ChooseStorageLocationButton storageLocationButton;
		
		public PropertyStorageLocations Location {
			get {
				return location;
			}
			set {
				if (location != value) {
					location = value;
					if (storageLocationButton != null) {
						storageLocationButton.StorageLocation = value;
					}
					helper.IsDirty = true;
				}
			}
		}
		
		public ChooseStorageLocationButton CreateLocationButton()
		{
			ChooseStorageLocationButton btn = new ChooseStorageLocationButton();
			if (location == PropertyStorageLocations.Unknown) {
				btn.StorageLocation = defaultLocation;
			} else {
				btn.StorageLocation = location;
			}
			RegisterLocationButton(btn);
			return btn;
		}
		
		/// <summary>
		/// Makes this configuration binding being controlled by the specified button.
		/// Use this method if you want to use one ChooseStorageLocationButton to control
		/// multiple properties.
		/// </summary>
		public void RegisterLocationButton(ChooseStorageLocationButton btn)
		{
			this.storageLocationButton = btn;
			btn.StorageLocationChanged += delegate(object sender, EventArgs e) {
				this.Location = ((ChooseStorageLocationButton)sender).StorageLocation;
			};
		}
		
		public ChooseStorageLocationButton CreateLocationButtonInPanel(string panelName)
		{
			ChooseStorageLocationButton btn = CreateLocationButton();
			Control panel = Helper.ControlDictionary[panelName];
			foreach (Control ctl in panel.Controls) {
				if ((ctl.Anchor & AnchorStyles.Left) == AnchorStyles.Left) {
					ctl.Left += btn.Width + 8;
					if ((ctl.Anchor & AnchorStyles.Right) == AnchorStyles.Right) {
						ctl.Width -= btn.Width + 8;
					}
				}
			}
			btn.Location = new Point(4, (panel.ClientSize.Height - btn.Height) / 2);
			panel.Controls.Add(btn);
			panel.Controls.SetChildIndex(btn, 0);
			return btn;
		}
		
		/// <summary>
		/// Moves the control '<paramref name="controlName"/>' a bit to the right and inserts a
		/// <see cref="ChooseStorageLocationButton"/>.
		/// </summary>
		public ChooseStorageLocationButton CreateLocationButton(string controlName)
		{
			return CreateLocationButton(Helper.ControlDictionary[controlName]);
		}
		
		/// <summary>
		/// Moves the <paramref name="replacedControl"/> a bit to the right and inserts a
		/// <see cref="ChooseStorageLocationButton"/>.
		/// </summary>
		public ChooseStorageLocationButton CreateLocationButton(Control replacedControl)
		{
			ChooseStorageLocationButton btn = CreateLocationButton();
			btn.Location = new Point(replacedControl.Left, replacedControl.Top + (replacedControl.Height - btn.Height) / 2);
			replacedControl.Left  += btn.Width + 4;
			replacedControl.Width -= btn.Width + 4;
			replacedControl.Parent.Controls.Add(btn);
			replacedControl.Parent.Controls.SetChildIndex(btn, replacedControl.Parent.Controls.IndexOf(replacedControl));
			return btn;
		}
		
		bool isFirstGet = true;
		
		public T Get<T>(T defaultValue)
		{
			if (isFirstGet) {
				isFirstGet = false;
				return helper.GetProperty(property, defaultValue,
				                          treatPropertyValueAsLiteral, out location);
			} else {
				return helper.GetProperty(property, defaultValue, treatPropertyValueAsLiteral);
			}
		}
		
		public void Set<T>(T value)
		{
			if (location == PropertyStorageLocations.Unknown) {
				location = defaultLocation;
			}
			helper.SetProperty(property, value, treatPropertyValueAsLiteral, location);
		}
		
		public abstract void Load();
		public abstract bool Save();
	}
}
