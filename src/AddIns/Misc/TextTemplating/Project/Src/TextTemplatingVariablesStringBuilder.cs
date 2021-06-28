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
using System.Text;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingVariablesStringBuilder
	{
		StringBuilder variablesBuilder = new StringBuilder();
		string unexpandedVariablesString;
		ITextTemplatingVariables templatingVariables;
		int currentIndex;
		
		public TextTemplatingVariablesStringBuilder(
			string unexpandedVariablesString,
			ITextTemplatingVariables templatingVariables)
		{
			this.unexpandedVariablesString = unexpandedVariablesString;
			this.templatingVariables = templatingVariables;
		}
		
		public void Append(string text)
		{
			variablesBuilder.Append(text);
		}
		
		public override string ToString()
		{
			return variablesBuilder.ToString();
		}
		
		public void AppendVariable(TextTemplatingVariableLocation variableLocation)
		{
			AppendVariableText(variableLocation);
			UpdateCurrentIndex(variableLocation);
		}
		
		public void AppendTextBeforeVariable(TextTemplatingVariableLocation variableLocation)
		{
			string textBeforeVariable = unexpandedVariablesString.Substring(currentIndex, variableLocation.Index);
			variablesBuilder.Append(textBeforeVariable);
		}
		
		void AppendVariableText(TextTemplatingVariableLocation variableLocation)
		{
			string variableValue = GetVariableValue(variableLocation);
			variablesBuilder.Append(variableValue);
		}
		
		void UpdateCurrentIndex(TextTemplatingVariableLocation variableLocation)
		{
			currentIndex = variableLocation.Index + variableLocation.Length;
		}
		
		string GetVariableValue(TextTemplatingVariableLocation variableLocation)
		{
			return templatingVariables.GetValue(variableLocation.VariableName);
		}
		
		public void AppendRemaining()
		{
			string textNotAppended = GetTextNotAppended();
			variablesBuilder.Append(textNotAppended);
		}
		
		string GetTextNotAppended()
		{
			return unexpandedVariablesString.Substring(currentIndex);
		}
	}
}
