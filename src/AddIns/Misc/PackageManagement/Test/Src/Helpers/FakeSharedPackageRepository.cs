// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSharedPackageRepository : FakePackageRepository, ISharedPackageRepository
	{
		public string PathPassedToRegisterRepository;
		
		public List<string> PackageIdsReferences = new List<string>();
		public string PackageIdPassedToIsReferenced;
		public SemanticVersion VersionPassedToIsReferenced;
		
		public bool IsReferenced(string packageId, SemanticVersion version)
		{
			PackageIdPassedToIsReferenced = packageId;
			VersionPassedToIsReferenced = version;
			return PackageIdsReferences.Contains(packageId);
		}
		
		public void RegisterRepository(string path)
		{
			PathPassedToRegisterRepository = path;
		}
		
		public void UnregisterRepository(string path)
		{
			throw new NotImplementedException();
		}
		
		public void AddPackageReferenceEntry(string packageId, SemanticVersion version)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSolutionReferenced(string packageId, SemanticVersion version)
		{
			throw new NotImplementedException();
		}
	}
}
