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
using System.Xml;
using System.Xml.XPath;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Stores an XmlNode and its associated line number and position after an 
	/// XPath query has been evaluated.
	/// </summary>
	public class XPathNodeMatch : IXmlLineInfo
	{
		int? lineNumber;
		int linePosition;
		string nodeValue;
		string displayValue;
		XPathNodeType nodeType;
		
		/// <summary>
		/// Creates an XPathNodeMatch from the navigator which should be position on the
		/// node.
		/// </summary>
		/// <remarks>
		/// We deliberately use the OuterXml when we find a Namespace since the
		/// navigator location returned starts from the xmlns attribute.
		/// </remarks>
		public XPathNodeMatch(XPathNavigator currentNavigator)
		{
			SetLineNumbers(currentNavigator as IXmlLineInfo);
			nodeType = currentNavigator.NodeType;
			switch (nodeType) {
				case XPathNodeType.Text:
					SetTextValue(currentNavigator);
					break;
				case XPathNodeType.Comment:
					SetCommentValue(currentNavigator);
					break;
				case XPathNodeType.Namespace:
					SetNamespaceValue(currentNavigator);
					break;
				case XPathNodeType.Element:
					SetElementValue(currentNavigator);
					break;
				case XPathNodeType.ProcessingInstruction:
					SetProcessingInstructionValue(currentNavigator);
					break;
				case XPathNodeType.Attribute:
					SetAttributeValue(currentNavigator);
					break;
				default:
					nodeValue = currentNavigator.LocalName;
					displayValue = nodeValue;
					break;
			}
		}
		
		public XPathNodeMatch(string nodeValue, string displayValue, int? lineNumber, int linePosition, XPathNodeType nodeType)
		{
			this.nodeValue = nodeValue;
			this.displayValue = displayValue;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.nodeType = nodeType;
		}
		
		/// <summary>
		/// Line numbers are zero based.
		/// </summary>
		public int LineNumber {
			get { return lineNumber.GetValueOrDefault(0); }
		}
		
		/// <summary>
		/// Line positions are zero based.
		/// </summary>
		public int LinePosition {
			get { return linePosition; }
		}
		
		public bool HasLineInfo()
		{
			return lineNumber.HasValue;
		}
		
		/// <summary>
		/// Gets the text value of the node.
		/// </summary>
		public string Value {
			get { return nodeValue; }
		}
		
		/// <summary>
		/// Gets the node display value. This includes the angle brackets if it is
		/// an element, for example.
		/// </summary>
		public string DisplayValue {
			get { return displayValue; }
		}
		
		public XPathNodeType NodeType {
			get { return nodeType; }
		}
		
		void SetElementValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			if (navigator.IsEmptyElement) {
				displayValue = String.Concat("<", nodeValue, "/>");
			} else {
				displayValue = String.Concat("<", nodeValue, ">");
			}
		}
		
		void SetTextValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Value;
			displayValue = nodeValue;
		}
		
		void SetCommentValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Value;
			displayValue = navigator.OuterXml;
		}
		
		void SetNamespaceValue(XPathNavigator navigator)
		{
			nodeValue = navigator.OuterXml;
			displayValue = nodeValue;
		}
		
		void SetProcessingInstructionValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			displayValue = navigator.OuterXml;
		}
		
		void SetAttributeValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			displayValue = String.Concat("@", nodeValue);
		}
		
		/// <summary>
		/// Takes one of the xml line number so the numbers are now zero
		/// based instead of one based.
		/// </summary>
		/// <remarks>A namespace query (e.g. //namespace::*) will return
		/// a line info of -1, -1 for the xml namespace. Which looks like
		/// a bug in the XPathDocument class.</remarks>
		void SetLineNumbers(IXmlLineInfo lineInfo)
		{
			if (lineInfo.HasLineInfo() && lineInfo.LineNumber > 0) {
				lineNumber = lineInfo.LineNumber - 1;
				linePosition = lineInfo.LinePosition - 1;
			}
		}
	}
}
