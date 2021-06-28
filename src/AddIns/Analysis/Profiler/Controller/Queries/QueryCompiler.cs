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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.Controller.Queries
{
	/// <summary>
	/// Used to report an error during compilation to a higher level.
	/// </summary>
	public delegate void ErrorReporter(IEnumerable<CompilerError> error);
	
	/// <summary>
	/// Analyzes, compiles and executes Profiler-specific LINQ queries.
	/// Any public static members of this type are thread safe. Any instance
	/// members are not guaranteed to be thread safe.
	/// </summary>
	public class QueryCompiler
	{
		static readonly CSharpCodeProvider csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

		static readonly string text = "using System;" + Environment.NewLine +
			"using System.Collections.Generic;" + Environment.NewLine +
			"using System.Linq;" + Environment.NewLine +
			"using ICSharpCode.Profiler.Controller;" + Environment.NewLine +
			"using ICSharpCode.Profiler.Controller.Queries;" + Environment.NewLine +
			"using ICSharpCode.Profiler.Controller.Data;" + Environment.NewLine +
			"class Query : QueryBase {" + Environment.NewLine +
			"public override object Execute()" + Environment.NewLine +
			"{" + Environment.NewLine +
			"return ";
		static readonly string textEnd = ";" + Environment.NewLine +
			"}" + Environment.NewLine +
			"}";

		string currentQuery;
		
		static Dictionary<string, Assembly> queryCache = new Dictionary<string, Assembly>();
		
		ErrorReporter report;

		/// <summary>
		/// Creates a new instance of the QueryCompiler.
		/// </summary>
		/// <param name="reporter">A delegate to report any errors to an upper layer.</param>
		/// <param name="query">The query to compile.</param>
		public QueryCompiler(ErrorReporter reporter, string query)
		{
			if (reporter == null)
				throw new ArgumentNullException("reporter");
			if (query == null)
				throw new ArgumentNullException("query");
			
			this.report = reporter;
			this.currentQuery = query;
		}

		/// <summary>
		/// Compiles the query.
		/// </summary>
		/// <returns>true, if successful, otherwise false.</returns>
		public bool Compile()
		{
			if (string.IsNullOrEmpty(currentQuery))
				return false;
			
			lock (queryCache) {
				if (!queryCache.ContainsKey(currentQuery)) {
					string code = text + PreprocessString(currentQuery) + textEnd;
					CompilerResults results = csc.CompileAssemblyFromSource(GetParameters(), code);
					report(results.Errors.Cast<CompilerError>());
					
					if (results.Errors.Count > 0)
						return false;
					
					queryCache.Add(currentQuery, results.CompiledAssembly);
				} else {
					report(new List<CompilerError>().AsEnumerable()); // clear errors list
				}
			}

			return true;
		}

		/// <summary>
		/// Executes the query.
		/// </summary>
		/// <returns>The result of the query.</returns>
		public IEnumerable<CallTreeNode> ExecuteQuery(ProfilingDataProvider provider, int startIndex, int endIndex)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			
			Assembly assembly;
			lock (queryCache)
				assembly = queryCache[currentQuery];
			QueryBase queryContainer = assembly.CreateInstance("Query") as QueryBase;
			
			queryContainer.Provider = provider;
			queryContainer.StartDataSetIndex = startIndex;
			queryContainer.EndDataSetIndex = endIndex;
			queryContainer.Root = provider.GetRoot(startIndex, endIndex);
			
			object data = queryContainer.Execute();
			
			if (data == null)
				return new List<CallTreeNode>().AsEnumerable();
			
			if (data is CallTreeNode) {
				return (new CallTreeNode[] { data as CallTreeNode }).AsEnumerable();
			} else if (data is IEnumerable<CallTreeNode>) {
				return (data as IEnumerable<CallTreeNode>);
			} else {
				throw new ProfilerException("Wrong query return type: " + data.GetType());
			}
		}

		CompilerParameters GetParameters()
		{
			var cp = new CompilerParameters();

			cp.OutputAssembly = Path.GetTempFileName();
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.Core.dll");
			cp.ReferencedAssemblies.Add(GetType().Assembly.Location);

			cp.WarningLevel = 4;
			cp.TreatWarningsAsErrors = true;

			return cp;
		}

		static string PreprocessString(string query)
		{
			return query;
		}
	}
}
