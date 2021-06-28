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

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Gives the Xml Attribute a type.
	/// </summary>
	public class WixXmlAttribute
	{
		string[] values;
		string name = String.Empty;
		string attributeValue = String.Empty;
		WixXmlAttributeType type = WixXmlAttributeType.Text;
		WixDocument document;
		
		public WixXmlAttribute(string name, string value, WixXmlAttributeType type, string[] values, WixDocument document)
		{
			this.name = name;
			attributeValue = value;
			this.type = type;
			this.values = values;
			this.document = document;
		}
		
		public WixXmlAttribute(string name, string value, WixXmlAttributeType type)
			: this(name, value, type, new string[0], null)
		{
		}

		public WixXmlAttribute(string name, WixXmlAttributeType type)
			: this(name, String.Empty, type, new string[0], null)
		{
		}
		
		public WixXmlAttribute(string name, WixXmlAttributeType type, string[] values, WixDocument document)
			: this(name, String.Empty, type, values, document)
		{
		}

		/// <summary>
		/// Gets the name of the attribute.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Gets or sets the value of the attribute.
		/// </summary>
		public string Value {
			get {
				return attributeValue;
			}
			set {
				if (value != null) {
					attributeValue = value;
				} else {
					attributeValue = String.Empty;
				}
			}
		}
		
		/// <summary>
		/// Gets the attribute type.
		/// </summary>
		public WixXmlAttributeType AttributeType {
			get {
				return type;
			}
		}
		
		/// <summary>
		/// Gets the set of allowed values for this attribute.
		/// </summary>
		public string[] Values {
			get {
				return values;
			}
		}
		
		/// <summary>
		/// Gets whether this attribute has any allowed values.
		/// </summary>
		public bool HasValues {
			get {
				if (values != null) {
					return values.Length > 0;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Gets the WixDocument this attribute is associated with.
		/// </summary>
		public WixDocument Document {
			get {
				return document;
			}
		}
	}
}
