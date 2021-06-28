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
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageClassTreeNode : CodeCoverageMethodsTreeNode
	{
		public CodeCoverageClassTreeNode(string name, List<CodeCoverageMethod> methods)
			: base(name, methods, CodeCoverageImageListIndex.Class)
		{
		}

		public override void ActivateItem()
		{
			foreach (CodeCoverageTreeNode node in Nodes) {
				CodeCoverageMethodTreeNode methodNode = node as CodeCoverageMethodTreeNode;
				CodeCoverageMethodsTreeNode methodsNode = node as CodeCoverageMethodsTreeNode;
				
				bool openedFile = false;
				if (methodNode != null) {
					openedFile = OpenFile(methodNode.Method.SequencePoints);
				} else if ((methodsNode != null) && (methodsNode.Methods.Count > 0)) {
					openedFile = OpenFile(methodsNode.Methods[0].SequencePoints);
				}
				
				if (openedFile) {
					break;
				}
			}
		}
		
		bool OpenFile(List<CodeCoverageSequencePoint> sequencePoints)
		{
			foreach (CodeCoverageSequencePoint point in sequencePoints) {
				if (point.HasDocument()) {
					OpenFile(point.Document);
					return true;
				}
			}
			return false;
		}
		
		protected override void Initialize()
		{
			Nodes.Clear();

			// Add methods.
			CodeCoveragePropertyCollection properties = new CodeCoveragePropertyCollection();
			foreach (CodeCoverageMethod method in Methods) {
				if (method.IsProperty) {
					properties.Add(method);
				} else {
					CodeCoverageMethodTreeNode node = new CodeCoverageMethodTreeNode(method);
					node.AddTo(this);
				}
			}
			
			// Add properties.s
			foreach (CodeCoverageProperty property in properties) {
				CodeCoveragePropertyTreeNode node = new CodeCoveragePropertyTreeNode(property);
				node.AddTo(this);
			}
			
			// Sort nodes.
			SortChildNodes();
		}
	}
}
