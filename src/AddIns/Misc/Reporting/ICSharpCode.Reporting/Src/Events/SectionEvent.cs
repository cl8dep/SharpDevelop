﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.02.2014
 * Time: 20:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting
{
	/// <summary>
	/// Description of SectionEvent.
	/// </summary>
	public class SectionEventArgs : EventArgs
	{
		
		
		public SectionEventArgs(IReportContainer section,int pageCount){
			if (section == null)
				throw new ArgumentNullException("section");
			Section = section;
			PageCount = pageCount;
		}
		
		public IReportContainer Section {get;private set;}
		
		public int PageCount {get; private set;}
	}
}
