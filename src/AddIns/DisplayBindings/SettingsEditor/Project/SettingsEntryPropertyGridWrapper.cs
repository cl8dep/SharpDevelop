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
using System.Xml;
using System.Configuration;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SettingsEditor
{
	/// <summary>
	/// Wraps around one SettingsEntry. Used to bind to the property grid, supporting
	/// type editor based on the setting's type.
	/// </summary>
	public sealed class SettingsEntryPropertyGridWrapper : LocalizedObject, INotifyPropertyChanged
	{
		readonly SettingsEntry entry;
		
		public SettingsEntryPropertyGridWrapper(SettingsEntry entry)
		{
			if (entry == null)
				throw new ArgumentNullException("entry");
			this.entry = entry;
		}
		
		[LocalizedProperty("Description",
		                   Description="Description of the setting.")]
		public string Description {
			get { return entry.Description; }
			set { entry.Description = value; }
		}
		
		[LocalizedProperty("Generate default value in code",
		                   Description="Specifies whether the value should be saved as attribute in the generated code.")]
		[DefaultValue(SettingsEntry.GenerateDefaultValueInCodeDefault)]
		public bool GenerateDefaultValueInCode {
			get { return entry.GenerateDefaultValueInCode; }
			set { entry.GenerateDefaultValueInCode = value; }
		}
		
		[LocalizedProperty("Name",
		                   Description="Name of the setting.")]
		public string Name {
			get { return entry.Name; }
			set { entry.Name = value; }
		}
		
		[LocalizedProperty("Provider",
		                   Description="The provider used to manage the setting.")]
		public string Provider {
			get { return entry.Provider; }
			set { entry.Provider = value; }
		}
		
		[LocalizedProperty("Roaming",
		                   Description="Specifies whether changes to the setting are stored in 'Application Data' (Roaming=true) or 'Local Application Data' (Roaming=false)")]
		public bool Roaming {
			get { return entry.Roaming; }
			set { entry.Roaming = value; }
		}
		
		[LocalizedProperty("Scope",
		                   Description="Specifies whether the setting is per-application (read-only) or per-user (read/write).")]
		public SettingScope Scope {
			get { return entry.Scope; }
			set { entry.Scope = value; }
		}
		
		[LocalizedProperty("SerializedSettingType",
		                   Description="The type used for the setting in the strongly-typed settings class.")]
		public string SerializedSettingType {
			get { return entry.SerializedSettingType; }
		}
		
		[LocalizedProperty("Value",
		                   Description="The default value of the setting.")]
		public object Value {
			get { return entry.Value; }
			set { entry.Value = value; }
		}
		
		#region Custom property descriptors
		protected override void FilterProperties(PropertyDescriptorCollection col)
		{
			base.FilterProperties(col);
			PropertyDescriptor oldValue = col["Value"];
			col.Remove(oldValue);
			col.Add(new CustomValuePropertyDescriptor(oldValue, entry.Type ?? typeof(string)));
			PropertyDescriptor oldRoaming = col["Roaming"];
			col.Remove(oldRoaming);
			col.Add(new RoamingPropertyDescriptor(oldRoaming, entry));
		}
		
		class RoamingPropertyDescriptor : ProxyPropertyDescriptor
		{
			SettingsEntry entry;
			
			public RoamingPropertyDescriptor(PropertyDescriptor baseDescriptor, SettingsEntry entry)
				: base(baseDescriptor)
			{
				if (entry == null)
					throw new ArgumentNullException("entry");
				this.entry = entry;
			}
			
			public override bool IsReadOnly {
				get {
					return entry.Scope == SettingScope.Application;
				}
			}
		}
		
		class CustomValuePropertyDescriptor : ProxyPropertyDescriptor
		{
			Type newPropertyType;
			
			public CustomValuePropertyDescriptor(PropertyDescriptor baseDescriptor, Type newPropertyType)
				: base(baseDescriptor)
			{
				if (newPropertyType == null)
					throw new ArgumentNullException("newPropertyType");
				this.newPropertyType = newPropertyType;
			}
			
			public override Type PropertyType {
				get {
					return newPropertyType;
				}
			}
			
			public override object GetEditor(Type editorBaseType)
			{
				return TypeDescriptor.GetEditor(newPropertyType, editorBaseType);
			}
		}
		#endregion
		
		public event PropertyChangedEventHandler PropertyChanged {
			add    { entry.PropertyChanged += value; }
			remove { entry.PropertyChanged -= value; }
		}
	}
}
