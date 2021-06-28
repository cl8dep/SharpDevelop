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
using System.Linq;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	public sealed class DebugInfoTokenWriterDecorator : DecoratingTokenWriter
	{
		readonly Stack<MethodDebugSymbols> symbolsStack = new Stack<MethodDebugSymbols>();
		
		public readonly Dictionary<string, MethodDebugSymbols> DebugSymbols = new Dictionary<string, MethodDebugSymbols>();
		public readonly Dictionary<string, ICSharpCode.NRefactory.TextLocation> MemberLocations = new Dictionary<string, ICSharpCode.NRefactory.TextLocation>();
		
		public DebugInfoTokenWriterDecorator(TokenWriter writer)
			: base(writer)
		{
		}
		
		public override void StartNode(AstNode node)
		{
			base.StartNode(node);
			if (node.Annotation<MethodDebugSymbols>() != null) {
				symbolsStack.Push(node.Annotation<MethodDebugSymbols>());
			}
		}
		
		public override void EndNode(AstNode node)
		{
			base.EndNode(node);
			if (node is EntityDeclaration && node.Annotation<MemberReference>() != null) {
				MemberLocations[XmlDocKeyProvider.GetKey(node.Annotation<MemberReference>())] = node.StartLocation;
			}
			
			// code mappings
			var ranges = node.Annotation<List<ILRange>>();
			if (symbolsStack.Count > 0 && ranges != null && ranges.Count > 0) {
				symbolsStack.Peek().SequencePoints.Add(
					new SequencePoint() {
						ILRanges = ILRange.OrderAndJoin(ranges).ToArray(),
						StartLocation = node.StartLocation,
						EndLocation = node.EndLocation
					});
			}
			
			if (node.Annotation<MethodDebugSymbols>() != null) {
				var symbols = symbolsStack.Pop();
				symbols.SequencePoints = symbols.SequencePoints.OrderBy(s => s.ILOffset).ToList();
				symbols.StartLocation = node.StartLocation;
				symbols.EndLocation = node.EndLocation;
				DebugSymbols[XmlDocKeyProvider.GetKey(symbols.CecilMethod)] = symbols;
			}
		}
	}
}
