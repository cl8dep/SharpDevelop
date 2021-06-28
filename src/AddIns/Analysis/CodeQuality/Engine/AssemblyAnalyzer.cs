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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using Mono.Cecil;

namespace ICSharpCode.CodeQuality.Engine
{
	/// <summary>
	/// Description of AssemblyAnalyzer.
	/// </summary>
	public class AssemblyAnalyzer
	{
		Dictionary<object, object> unresolvedTypeSystemToCecilDict = new Dictionary<object, object>();
		ICompilation compilation;
		internal Dictionary<IAssembly, AssemblyNode> assemblyMappings;
		internal Dictionary<string, NamespaceNode> namespaceMappings;
		internal Dictionary<ITypeDefinition, TypeNode> typeMappings;
		internal Dictionary<IMethod, MethodNode> methodMappings;
		internal Dictionary<IField, FieldNode> fieldMappings;
		internal Dictionary<IProperty, PropertyNode> propertyMappings;
		internal Dictionary<IEvent, EventNode> eventMappings;
		internal Dictionary<MemberReference, IEntity> cecilMappings;
		List<string> fileNames;
		
		internal IProgressMonitor progressMonitor;
		
		public AssemblyAnalyzer()
		{
			fileNames = new List<string>();
		}
		
		public void AddAssemblyFiles(params string[] files)
		{
			fileNames.AddRange(files);
		}
		
		HashSet<NodeBase> outgoingEdges = new HashSet<NodeBase>();
		
		public void AddEdge(NodeBase target)
		{
			// copies all ancestors of target into a hashset
			// duplicates are removed
			while (target != null) {
				if (!outgoingEdges.Add(target))
					break;
				target = target.Parent;
			}
		}
		
		void CreateEdges(NodeBase source)
		{
			// add edges to source
			while (source != null) {
				foreach (NodeBase n in outgoingEdges) {
					source.AddRelationship(n);
				}
				source = source.Parent;
			}
			outgoingEdges.Clear();
		}
		
		public ReadOnlyCollection<AssemblyNode> Analyze()
		{
			var loadedAssemblies = LoadAssemblies();
			compilation = new SimpleCompilation(loadedAssemblies.First(), loadedAssemblies.Skip(1));
			
			assemblyMappings = new Dictionary<IAssembly, AssemblyNode>();
			namespaceMappings = new Dictionary<string, NamespaceNode>();
			typeMappings = new Dictionary<ITypeDefinition, TypeNode>();
			fieldMappings = new Dictionary<IField, FieldNode>();
			methodMappings = new Dictionary<IMethod, MethodNode>();
			propertyMappings = new Dictionary<IProperty, PropertyNode>();
			eventMappings = new Dictionary<IEvent, EventNode>();
			cecilMappings = new Dictionary<MemberReference, IEntity>();
			
			// first we have to read all types so every method, field or property has a container
			foreach (var type in compilation.GetAllTypeDefinitions()) {
				var tn = ReadType(type);
				
				foreach (var field in type.Fields) {
					var node = new FieldNode(field);
					fieldMappings.Add(field, node);
					var cecilObj = GetCecilObject((IUnresolvedField)field.UnresolvedMember);
					if (cecilObj != null)
						cecilMappings[cecilObj] = field;
					tn.AddChild(node);
				}
				
				foreach (var method in type.Methods) {
					var node = new MethodNode(method);
					methodMappings.Add(method, node);
					var cecilObj = GetCecilObject((IUnresolvedMethod)method.UnresolvedMember);
					if (cecilObj != null)
						cecilMappings[cecilObj] = method;
					tn.AddChild(node);
				}
				
				foreach (var property in type.Properties) {
					var node = new PropertyNode(property);
					propertyMappings.Add(property, node);
					var cecilPropObj = GetCecilObject((IUnresolvedProperty)property.UnresolvedMember);
					if (cecilPropObj != null)
						cecilMappings[cecilPropObj] = property;
					if (property.CanGet) {
						var cecilMethodObj = GetCecilObject((IUnresolvedMethod)property.Getter.UnresolvedMember);
						if (cecilMethodObj != null)
							cecilMappings[cecilMethodObj] = property;
					}
					if (property.CanSet) {
						var cecilMethodObj = GetCecilObject((IUnresolvedMethod)property.Setter.UnresolvedMember);
						if (cecilMethodObj != null)
							cecilMappings[cecilMethodObj] = property;
					}
					tn.AddChild(node);
				}
				
				foreach (var @event in type.Events) {
					var node = new EventNode(@event);
					eventMappings.Add(@event, node);
					var cecilObj = GetCecilObject((IUnresolvedEvent)@event.UnresolvedMember);
					if (cecilObj != null)
						cecilMappings[cecilObj] = @event;
					if (@event.CanAdd) {
						var cecilMethodObj = GetCecilObject((IUnresolvedMethod)@event.AddAccessor.UnresolvedMember);
						if (cecilMethodObj != null)
							cecilMappings[cecilMethodObj] = @event;
					}
					if (@event.CanInvoke) {
						var cecilMethodObj = GetCecilObject((IUnresolvedMethod)@event.InvokeAccessor.UnresolvedMember);
						if (cecilMethodObj != null)
							cecilMappings[cecilMethodObj] = @event;
					}
					if (@event.CanRemove) {
						var cecilMethodObj = GetCecilObject((IUnresolvedMethod)@event.RemoveAccessor.UnresolvedMember);
						if (cecilMethodObj != null)
							cecilMappings[cecilMethodObj] = @event;
					}
					tn.AddChild(node);
				}
			}
			
			ILAnalyzer analyzer = new ILAnalyzer(loadedAssemblies.Select(asm => GetCecilObject(asm)).ToArray(), this);
			int count = typeMappings.Count + methodMappings.Count + fieldMappings.Count + propertyMappings.Count;
			int i  = 0;
			
			foreach (var element in typeMappings) {
				ReportProgress(++i / (double)count);
				AddRelationshipsForTypes(element.Key.DirectBaseTypes, element.Value);
				AddRelationshipsForAttributes(element.Key.Attributes, element.Value);
				CreateEdges(element.Value);
			}
			
			foreach (var element in methodMappings) {
				ReportProgress(++i / (double)count);
				var cecilObj = GetCecilObject((IUnresolvedMethod)element.Key.UnresolvedMember);
				if (cecilObj != null)
					analyzer.Analyze(cecilObj.Body, element.Value);
				var node = element.Value;
				var method = element.Key;
				AddRelationshipsForType(node, method.ReturnType);
				AddRelationshipsForAttributes(method.Attributes, node);
				AddRelationshipsForAttributes(method.ReturnTypeAttributes, node);
				AddRelationshipsForTypeParameters(method.TypeParameters, node);
				foreach (var param in method.Parameters) {
					AddRelationshipsForType(node, param.Type);
					AddRelationshipsForAttributes(param.Attributes, node);
				}
				CreateEdges(element.Value);
			}
			
			foreach (var element in fieldMappings) {
				ReportProgress(++i / (double)count);
				var node = element.Value;
				var field = element.Key;
				AddRelationshipsForType(node, field.Type);
				AddRelationshipsForAttributes(field.Attributes, node);
				CreateEdges(element.Value);
			}
			
			foreach (var element in propertyMappings) {
				ReportProgress(++i / (double)count);
				var node = element.Value;
				var property = element.Key;
				if (property.CanGet) {
					var cecilObj = GetCecilObject((IUnresolvedMethod)element.Key.Getter.UnresolvedMember);
					if (cecilObj != null)
						analyzer.Analyze(cecilObj.Body, node);
				}
				if (property.CanSet) {
					var cecilObj = GetCecilObject((IUnresolvedMethod)element.Key.Setter.UnresolvedMember);
					if (cecilObj != null)
						analyzer.Analyze(cecilObj.Body, node);
				}
				AddRelationshipsForType(node, property.ReturnType);
				AddRelationshipsForAttributes(property.Attributes, node);
				CreateEdges(element.Value);
			}
			
			foreach (var element in eventMappings) {
				ReportProgress(++i / (double)count);
				var node = element.Value;
				var @event = element.Key;
				if (@event.CanAdd) {
					var cecilObj = GetCecilObject((IUnresolvedMethod)@event.AddAccessor.UnresolvedMember);
					if (cecilObj != null)
						analyzer.Analyze(cecilObj.Body, node);
				}
				if (@event.CanInvoke) {
					var cecilObj = GetCecilObject((IUnresolvedMethod)@event.InvokeAccessor.UnresolvedMember);
					if (cecilObj != null)
						analyzer.Analyze(cecilObj.Body, node);
				}
				if (@event.CanRemove) {
					var cecilObj = GetCecilObject((IUnresolvedMethod)@event.RemoveAccessor.UnresolvedMember);
					if (cecilObj != null)
						analyzer.Analyze(cecilObj.Body, node);
				}
				AddRelationshipsForType(node, @event.ReturnType);
				AddRelationshipsForAttributes(@event.Attributes, node);
				CreateEdges(element.Value);
			}
			
			return new ReadOnlyCollection<AssemblyNode>(assemblyMappings.Values.ToList());
		}
		
		void ReportProgress(double progress)
		{
			if (progressMonitor != null) {
				progressMonitor.Progress = progress;
			}
		}
		
		void AddRelationshipsForTypeParameters(IList<ITypeParameter> typeParameters, NodeBase node)
		{
			foreach (var param in typeParameters) {
				AddRelationshipsForAttributes(param.Attributes, node);
				AddRelationshipsForType(node, param.EffectiveBaseClass);
			}
		}
		
		void AddRelationshipsForTypes(IEnumerable<IType> directBaseTypes, NodeBase node)
		{
			foreach (var baseType in directBaseTypes) {
				AddRelationshipsForType(node, baseType);
			}
		}
		
		void AddRelationshipsForAttributes(IList<IAttribute> attributes, NodeBase node)
		{
			try {
				foreach (var attr in attributes) {
					if (attr.Constructor != null)
						AddEdge(methodMappings[attr.Constructor]);
				}
			} catch (NotSupportedException nse) {
				// HACK : workaround for bug in NR5's attribute blob parser.
				LoggingService.DebugFormatted("CQA: Skipping attributes of: {0}\r\nException:\r\n{1}", node.Name, nse);
			}
		}
		
		void AddRelationshipsForType(NodeBase node, IType type)
		{
			type.AcceptVisitor(new AnalysisTypeVisitor(this, node));
		}
		
		class AnalysisTypeVisitor : TypeVisitor
		{
			NodeBase node;
			AssemblyAnalyzer context;
			
			public AnalysisTypeVisitor(AssemblyAnalyzer context, NodeBase node)
			{
				this.context = context;
				this.node = node;
			}
			
			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				TypeNode typeNode;
				if (context.typeMappings.TryGetValue(type, out typeNode))
					context.AddEdge(typeNode);
				return base.VisitTypeDefinition(type);
			}
		}
		
		IList<IUnresolvedAssembly> LoadAssemblies()
		{
			var resolver = new AssemblyResolver();
			foreach (var path in fileNames.Select(f => Path.GetDirectoryName(f)).Distinct(StringComparer.OrdinalIgnoreCase))
				resolver.AddSearchDirectory(path);
			List<AssemblyDefinition> assemblies = new List<AssemblyDefinition>();
			foreach (var file in fileNames.Distinct(StringComparer.OrdinalIgnoreCase))
				assemblies.Add(resolver.LoadAssemblyFile(file));
			foreach (var asm in assemblies.ToArray())
				assemblies.AddRange(asm.Modules.SelectMany(m => m.AssemblyReferences).Select(r => resolver.TryResolve(r)).Where(r => r != null));
			CecilLoader loader = new CecilLoader { IncludeInternalMembers = true };
			// Emulate the old CecilLoader.GetCecilObject() API: 
			loader.OnEntityLoaded = delegate(IUnresolvedEntity entity, MemberReference cecilObj) {
				unresolvedTypeSystemToCecilDict[entity] = cecilObj;
			};
			var loadedAssemblies = new List<IUnresolvedAssembly>();
			foreach (var asm in assemblies.Distinct()) {
				var loadedAssembly = loader.LoadAssembly(asm);
				loadedAssemblies.Add(loadedAssembly);
				unresolvedTypeSystemToCecilDict[loadedAssembly] = asm;
			}
			return loadedAssemblies;
		}
		
		AssemblyDefinition GetCecilObject(IUnresolvedAssembly assembly)
		{
			object cecilObj;
			if (unresolvedTypeSystemToCecilDict.TryGetValue(assembly, out cecilObj)) {
				return cecilObj as AssemblyDefinition;
			} else {
				return null;
			}
		}
		
		MemberReference GetCecilObject(IUnresolvedEntity entity)
		{
			object cecilObj;
			if (unresolvedTypeSystemToCecilDict.TryGetValue(entity, out cecilObj)) {
				return cecilObj as MemberReference;
			} else {
				return null;
			}
		}
		
		MethodDefinition GetCecilObject(IUnresolvedMethod method)
		{
			object cecilObj;
			if (unresolvedTypeSystemToCecilDict.TryGetValue(method, out cecilObj)) {
				return cecilObj as MethodDefinition;
			} else {
				return null;
			}
		}
		
		NamespaceNode GetOrCreateNamespace(AssemblyNode assembly, string namespaceName)
		{
			NamespaceNode result;
			var asmDef = GetCecilObject(assembly.AssemblyInfo.UnresolvedAssembly);
			if (!namespaceMappings.TryGetValue(namespaceName + "," + asmDef.FullName, out result)) {
				result = new NamespaceNode(namespaceName);
				assembly.AddChild(result);
				namespaceMappings.Add(namespaceName + "," + asmDef.FullName, result);
			}
			return result;
		}
		
		AssemblyNode GetOrCreateAssembly(IAssembly asm)
		{
			AssemblyNode result;
			if (!assemblyMappings.TryGetValue(asm, out result)) {
				result = new AssemblyNode(asm);
				assemblyMappings.Add(asm, result);
			}
			return result;
		}
		
		TypeNode ReadType(ITypeDefinition type)
		{
			var asm = GetOrCreateAssembly(type.ParentAssembly);
			var ns = GetOrCreateNamespace(asm, type.Namespace);
			TypeNode parent;
			var node = new TypeNode(type);
			if (type.DeclaringTypeDefinition != null) {
				if (typeMappings.TryGetValue(type.DeclaringTypeDefinition, out parent))
					parent.AddChild(node);
				else
					throw new Exception("TypeNode not found: " + type.DeclaringTypeDefinition.FullName);
			} else
				ns.AddChild(node);
			cecilMappings[GetCecilObject(type.Parts.First())] = type;
			typeMappings.Add(type, node);
			return node;
		}
		
		class AssemblyResolver : DefaultAssemblyResolver
		{
			public AssemblyDefinition LoadAssemblyFile(string fileName)
			{
				var assembly = AssemblyDefinition.ReadAssembly(fileName, new ReaderParameters { AssemblyResolver = this });
				RegisterAssembly(assembly);
				return assembly;
			}
			
			public AssemblyDefinition TryResolve(AssemblyNameReference reference)
			{
				try {
					return Resolve(reference);
				} catch (AssemblyResolutionException are) {
					LoggingService.DebugFormatted("CQA: Skipping assembly reference: {0}\r\nException:\r\n{1}", reference, are);
					TaskService.Add(new SDTask(null, are.Message, 0, 0, SharpDevelop.TaskType.Warning));
					return null;
				}
			}
		}
	}
}
