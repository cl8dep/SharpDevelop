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
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace Hornung.ResourceToolkit.Commands
{
	public static class FindMissingResourceKeysHelper
	{
		public static void Run(SearchScope scope) {
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:Hornung.ResourceToolkit.FindMissingResourceKeys}")) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToMissingKeys}"),
				                                                  ResourceRefactoringService.FindReferencesToMissingKeys(monitor, scope));
			}
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the whole solution.
	/// </summary>
	public class FindMissingResourceKeysWholeSolutionCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.WholeSolution);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the current project.
	/// </summary>
	public class FindMissingResourceKeysCurrentProjectCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.CurrentProject);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in the current file.
	/// </summary>
	public class FindMissingResourceKeysCurrentFileCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.CurrentFile);
		}
	}
	
	/// <summary>
	/// Find missing resource keys in all open files.
	/// </summary>
	public class FindMissingResourceKeysOpenFilesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FindMissingResourceKeysHelper.Run(SearchScope.OpenFiles);
		}
	}
	
	/// <summary>
	/// Find unused resource keys in the whole solution.
	/// </summary>
	public class FindUnusedResourceKeysCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ICollection<ResourceItem> unusedKeys;
			
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:Hornung.ResourceToolkit.FindUnusedResourceKeys}")) {
				unusedKeys = ResourceRefactoringService.FindUnusedKeys(monitor);
			}
			
			if (unusedKeys == null) {
				return;
			}
			
			if (unusedKeys.Count == 0) {
				MessageService.ShowMessage("${res:Hornung.ResourceToolkit.UnusedResourceKeys.NotFound}");
				return;
			}
			
			IWorkbench workbench = WorkbenchSingleton.Workbench;
			if (workbench != null) {
				UnusedResourceKeysViewContent vc = new UnusedResourceKeysViewContent(unusedKeys);
				workbench.ShowView(vc);
			}
		}
	}
}
