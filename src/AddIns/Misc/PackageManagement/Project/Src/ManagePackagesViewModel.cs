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
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ManagePackagesViewModel : ViewModelBase<ManagePackagesViewModel>, IDisposable
	{
		IThreadSafePackageManagementEvents packageManagementEvents;
		ManagePackagesUserPrompts userPrompts;
		PackagesViewModels packagesViewModels;
		ManagePackagesViewTitle viewTitle;
		string message;
		bool hasError;
		
		public ManagePackagesViewModel(
			PackagesViewModels packagesViewModels,
			ManagePackagesViewTitle viewTitle,
			IThreadSafePackageManagementEvents packageManagementEvents,
			ManagePackagesUserPrompts userPrompts)
		{
			this.packagesViewModels = packagesViewModels;
			this.viewTitle = viewTitle;
			this.packageManagementEvents = packageManagementEvents;
			this.userPrompts = userPrompts;
			
			packageManagementEvents.PackageOperationError += PackageOperationError;
			packageManagementEvents.PackageOperationsStarting += PackageOperationsStarting;
			
			packagesViewModels.ReadPackages();
		}
		
		public AvailablePackagesViewModel AvailablePackagesViewModel {
			get { return packagesViewModels.AvailablePackagesViewModel; }
		}
		
		public InstalledPackagesViewModel InstalledPackagesViewModel {
			get { return packagesViewModels.InstalledPackagesViewModel; }
		}
		
		public UpdatedPackagesViewModel UpdatedPackagesViewModel {
			get { return packagesViewModels.UpdatedPackagesViewModel; }
		}
		
		public RecentPackagesViewModel RecentPackagesViewModel {
			get { return packagesViewModels.RecentPackagesViewModel; }
		}
		
		public string Title {
			get { return viewTitle.Title; }
		}
		
		public void Dispose()
		{
			packagesViewModels.Dispose();
			userPrompts.Dispose();
			
			packageManagementEvents.PackageOperationError -= PackageOperationError;
			packageManagementEvents.PackageOperationsStarting -= PackageOperationsStarting;
			packageManagementEvents.Dispose();
		}
		
		void PackageOperationError(object sender, PackageOperationExceptionEventArgs e)
		{
			ShowErrorMessage(e.Exception.Message);
		}
		
		void ShowErrorMessage(string message)
		{
			this.Message = message;
			this.HasError = true;
		}
		
		public string Message {
			get { return message; }
			set {
				message = value;
				OnPropertyChanged(model => model.Message);
			}
		}
		
		public bool HasError {
			get { return hasError; }
			set {
				hasError = value;
				OnPropertyChanged(model => model.HasError);
			}
		}
		
		void PackageOperationsStarting(object sender, EventArgs e)
		{
			ClearMessage();
		}
		
		void ClearMessage()
		{
			this.Message = null;
			this.HasError = false;
		}
	}
}
