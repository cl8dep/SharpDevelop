﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 18:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;

namespace ICSharpCode.Reporting.Addin.XML
{
	/// <summary>
	/// Description of ReportDesignerWriter.
	/// </summary>
	class ReportDesignerWriter:MycroWriter
	{
		protected override string GetTypeName(Type t)
		{
			if (t.BaseType != null && t.BaseType.Name.StartsWith("Base", StringComparison.OrdinalIgnoreCase)) {
			                                                    
			                                                    
//				return t.BaseType.Name;
			}
			return t.Name;
		}
	}
}
