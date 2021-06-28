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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcControllerToProjectCommand : AbstractCommand
	{
		public override void Run()
		{
			IAddMvcItemToProjectView view = CreateView();
			view.DataContext = CreateDataContext();
			view.ShowDialog();
		}
		
		protected virtual IAddMvcItemToProjectView CreateView()
		{
			return new AddMvcControllerToProjectView() {
				Owner = SD.Workbench.MainWindow
			};
		}
		
		protected virtual object CreateDataContext()
		{
			SelectedMvcControllerFolder selectedFolder = GetSelectedFolder();
			return new AddMvcControllerToProjectViewModel(selectedFolder);
		}
		
		SelectedMvcControllerFolder GetSelectedFolder()
		{
			var directoryNode = Owner as DirectoryNode;
			return new SelectedMvcControllerFolder(directoryNode);
		}
	}
}
