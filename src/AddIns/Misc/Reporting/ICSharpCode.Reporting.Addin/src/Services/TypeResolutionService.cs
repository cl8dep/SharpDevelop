﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.02.2014
 * Time: 19:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.Reporting.Addin.Services
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	
	class TypeResolutionService : ITypeResolutionService
	{
		readonly static List<Assembly> designerAssemblies = new List<Assembly>();
		
		/// <summary>
		/// List of assemblies used by the form designer. This static list is not an optimal solution,
		/// but better than using AppDomain.CurrentDomain.GetAssemblies(). See SD2-630.
		/// </summary>
		public static List<Assembly> DesignerAssemblies {
			get {
				return designerAssemblies;
			}
		}
		
		static TypeResolutionService()
		{
			DesignerAssemblies.Add(typeof(object).Assembly);
			DesignerAssemblies.Add(typeof(Uri).Assembly);
			DesignerAssemblies.Add(typeof(System.Drawing.Point).Assembly);
			DesignerAssemblies.Add(typeof(System.Windows.Forms.Design.AnchorEditor).Assembly);
			DesignerAssemblies.Add(typeof(TypeResolutionService).Assembly);
		}
		
		public Assembly GetAssembly(AssemblyName name)
		{
			return LoadAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			return LoadAssembly(name, throwOnError);
		}
		
		static Assembly LoadAssembly(AssemblyName name, bool throwOnError)
		{
			try {
				return Assembly.Load(name);
			} catch (System.IO.FileLoadException) {
				if (throwOnError)
					throw;
				return null;
			}
		}
		
		public string GetPathOfAssembly(AssemblyName name)
		{
			Assembly assembly = GetAssembly(name);
			if (assembly != null) {
				return assembly.Location;
			}
			return null;
		}
		
		public Type GetType(string name)
		{
			return GetType(name, false, false);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError, false);
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null || name.Length == 0) {
				return null;
			}
			#if DEBUG
			if (!name.StartsWith("System.",StringComparison.InvariantCultureIgnoreCase)) {
				LoggingService.Debug("TypeResolutionService: Looking for " + name);
			}
			#endif
			try {
				
				Type type = Type.GetType(name, false, ignoreCase);
				
				if (type == null) {
					lock (designerAssemblies) {
						foreach (Assembly asm in DesignerAssemblies) {
							Type t = asm.GetType(name, false);
							if (t != null) {
								return t;
							}
						}
					}
				}
				
				if (throwOnError && type == null)
					throw new TypeLoadException(name + " not found by TypeResolutionService");
				
				return type;
			} catch (Exception e) {
				LoggingService.Error(e);
			}
			return null;
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			LoggingService.Warn("TODO: Add Assembly reference : " + name);
		}
	}
	
}
