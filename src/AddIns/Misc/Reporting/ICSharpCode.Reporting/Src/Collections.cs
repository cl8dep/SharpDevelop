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
using System.Globalization;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting
{
	
	public class SortColumnCollection: Collection<AbstractColumn>
	{
		
		public AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,StringComparison.OrdinalIgnoreCase));
		}
	
		
		public void AddRange (IEnumerable<SortColumn> items)
		{
			foreach (SortColumn item in items){
				this.Add(item);
			}
		}
	}
	
	
	public class GroupColumnCollection: SortColumnCollection
	{
		
		public new AbstractColumn Find (string columnName)
		{
			if (String.IsNullOrEmpty(columnName)) {
				throw new ArgumentNullException("columnName");
			}
			
			return this.FirstOrDefault(x => 0 == String.Compare(x.ColumnName,columnName,StringComparison.OrdinalIgnoreCase));
		}
	}
	
	
	public class ParameterCollection: Collection<BasicParameter>{		
		
		public BasicParameter Find (string parameterName)
		{
			if (String.IsNullOrEmpty(parameterName)) {
				throw new ArgumentNullException("parameterName");
			}
			return this.FirstOrDefault(x => 0 == String.Compare(x.ParameterName,parameterName,StringComparison.OrdinalIgnoreCase));
		}
		
		
		public static CultureInfo Culture
		{
			get { return CultureInfo.CurrentCulture; }
		}
		
		
		public void AddRange (IEnumerable<BasicParameter> items)
		{
			foreach (BasicParameter item in items){
				this.Add(item);
			}
		}
	}
}
