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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ErrorProject : AbstractProject
	{
		readonly Exception exception;

		/// <summary>
		/// The exception associated that prevented loading this project.
		/// This should be a <c>ProjectLoadException</c> or <c>IOException</c>.
		/// </summary>
		public Exception Exception {
			get {
				return exception;
			}
		}

		//string warningText = "${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.NoBackendForProjectType}";
		
//		public void ShowWarningMessageBox()
//		{
//			warningDisplayedToUser = true;
//			MessageService.ShowError("Error loading " + this.FileName + ":\n" + warningText);
//		}
		
		public ErrorProject(ProjectLoadInformation information, Exception exception)
			: base(information)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			this.exception = exception;
		}
		
		public override void ProjectLoaded()
		{
			base.ProjectLoaded();
			var error = exception as ProjectLoadException;
			if (error != null) {
				SD.OutputPad.BuildCategory.Activate();
				error.WriteToOutputPad(SD.OutputPad.BuildCategory);
			}
		}
		
		protected override ProjectBehavior GetOrCreateBehavior()
		{
			// don't add behaviors from AddIn-Tree to UnknownProject
			lock (SyncRoot) {
				if (projectBehavior == null)
					projectBehavior = new DefaultProjectBehavior(this);
				return projectBehavior;
			}
		}
		
		public override bool HasProjectType(Guid projectTypeGuid)
		{
			// Don't report true for this.TypeGuid
			return false;
		}
	}
}
