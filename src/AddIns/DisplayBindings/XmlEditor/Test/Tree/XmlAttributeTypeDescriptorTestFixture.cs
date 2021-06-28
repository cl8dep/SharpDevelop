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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Xml;

namespace XmlEditor.Tests.Tree
{
	[TestFixture]
	public class XmlAttributeTypeDescriptorTestFixture
	{
		XmlAttributeTypeDescriptor typeDescriptor;
		
		[SetUp]
		public void Init()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<root first='a'/>");
			typeDescriptor = new XmlAttributeTypeDescriptor(doc.DocumentElement.Attributes);
		}
		
		[Test]
		public void OneProperty()
		{
			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
			Assert.AreEqual(1, properties.Count);
		}
		
		[Test]
		public void PropertyName()
		{
			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
			PropertyDescriptor descriptor = properties[0];
			Assert.AreEqual("first", descriptor.Name);
		}
		
		[Test]
		public void PropertyOwner()
		{
			Assert.IsTrue(Object.ReferenceEquals(typeDescriptor, typeDescriptor.GetPropertyOwner(null)));
		}
		
		[Test]
		public void ComponentName()
		{
			Assert.IsNull(typeDescriptor.GetComponentName());
		}
		
		[Test]
		public void DefaultEvent()
		{
			Assert.IsNull(typeDescriptor.GetDefaultEvent());
		}
		
		[Test]
		public void Events()
		{
			Assert.IsNull(typeDescriptor.GetEvents());
			Assert.IsNull(typeDescriptor.GetEvents(new Attribute[0]));
		}
		
		[Test]
		public void NullAttributesCollection()
		{
			XmlAttributeTypeDescriptor typeDescriptor = new XmlAttributeTypeDescriptor(null);
			PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
			Assert.AreEqual(0, properties.Count);
		}
	}
}
