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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.Core.Tests.AddInTreeTests.Tests
{
	[TestFixture]
	public class AddInTreeLoadingTests
	{
		IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
		
		#region AddIn node tests
		[Test]
		public void TestEmptyAddInTreeLoading()
		{
			string addInText = @"<AddIn/>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
		}
		
		[Test]
		public void TestAddInProperties()
		{
			string addInText = @"
<AddIn name        = 'SharpDevelop Core'
       author      = 'Mike Krueger'
       copyright   = 'GPL'
       url         = 'http://www.icsharpcode.net'
       description = 'SharpDevelop core module'
       version     = '1.0.0'/>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(addIn.Properties["name"], "SharpDevelop Core");
			Assert.AreEqual(addIn.Properties["author"], "Mike Krueger");
			Assert.AreEqual(addIn.Properties["copyright"], "GPL");
			Assert.AreEqual(addIn.Properties["url"], "http://www.icsharpcode.net");
			Assert.AreEqual(addIn.Properties["description"], "SharpDevelop core module");
			Assert.AreEqual(addIn.Properties["version"], "1.0.0");
		}
		#endregion
		
		#region Runtime section tests
		[Test]
		public void TestEmtpyRuntimeSection()
		{
			string addInText = @"<AddIn><Runtime/></AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
		}
		
		[Test]
		public void TestEmtpyRuntimeSection2()
		{
			string addInText = @"<AddIn> <!-- Comment1 --> <Runtime>  <!-- Comment2 -->    </Runtime> <!-- Comment3 --> </AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
		}
		
		[Test]
		public void TestRuntimeSectionImport()
		{
			string addInText = @"
<AddIn>
	<Runtime>
		<Import assembly = 'Test.dll'/>
	</Runtime>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Runtimes.Count);
			Assert.AreEqual(addIn.Runtimes[0].Assembly, "Test.dll");
		}
		
		[Test]
		public void TestRuntimeSectionComplexImport()
		{
			string addInText = @"
<AddIn>
	<Runtime>
		<Import assembly = '../bin/SharpDevelop.Base.dll'>
			<Doozer             name='MyDoozer'   class = 'ICSharpCode.Core.ClassDoozer'/>
			<ConditionEvaluator name='MyCompare'  class = 'ICSharpCode.Core.CompareCondition'/>
			<Doozer             name='Test'       class = 'ICSharpCode.Core.ClassDoozer2'/>
			<ConditionEvaluator name='Condition2' class = 'Condition2Class'/>
		</Import>
	</Runtime>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Runtimes.Count);
			Assert.AreEqual(addIn.Runtimes[0].Assembly, "../bin/SharpDevelop.Base.dll");
			Assert.AreEqual(addIn.Runtimes[0].DefinedDoozers.Count(), 2);
			Assert.AreEqual(addIn.Runtimes[0].DefinedDoozers.ElementAt(0).Key, "MyDoozer");
			Assert.AreEqual(addIn.Runtimes[0].DefinedDoozers.ElementAt(1).Key, "Test");
			
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditionEvaluators.Count(), 2);
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditionEvaluators.ElementAt(0).Key, "MyCompare");
			
			Assert.AreEqual(addIn.Runtimes[0].DefinedConditionEvaluators.ElementAt(1).Key, "Condition2");
		}
		#endregion
		
		#region Path section tests
		[Test]
		public void TestEmptyPathSection()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'/>
	<Path name = '/Path2'/>
	<Path name = '/Path1/SubPath'/>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(3, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			Assert.IsNotNull(addIn.Paths["/Path2"]);
			Assert.IsNotNull(addIn.Paths["/Path1/SubPath"]);
		}
		
		[Test]
		public void TestSimpleCodon()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Simple id ='Simple' attr='a' attr2='b'/>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			
			List<Codon> codons = addIn.Paths["/Path1"].Codons.ToList();
			Assert.AreEqual(1, codons.Count);
			Assert.AreEqual("Simple", codons[0].Name);
			Assert.AreEqual("Simple", codons[0].Id);
			Assert.AreEqual("a", codons[0].Properties["attr"]);
			Assert.AreEqual("b", codons[0].Properties["attr2"]);
		}
		
		[Test]
		public void TestSubCodons()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Sub id='Path2'>
			<Codon2 id='Sub2'/>
		</Sub>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(2, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			List<Codon> codons1 = addIn.Paths["/Path1"].Codons.ToList();
			Assert.AreEqual(1, codons1.Count);
			Assert.AreEqual("Sub", codons1[0].Name);
			Assert.AreEqual("Path2", codons1[0].Id);
			
			Assert.IsNotNull(addIn.Paths["/Path1/Path2"]);
			List<Codon> codons2 = addIn.Paths["/Path1/Path2"].Codons.ToList();
			Assert.AreEqual(1, codons2.Count);
			Assert.AreEqual("Codon2", codons2[0].Name);
			Assert.AreEqual("Sub2", codons2[0].Id);
		}
		
		[Test]
		public void TestSubCodonsWithCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Condition name='Equal' string='a' equal='b'>
			<Sub id='Path2'>
				<Codon2 id='Sub2'/>
			</Sub>
		</Condition>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(2, addIn.Paths.Count);
			Assert.IsNotNull(addIn.Paths["/Path1"]);
			List<Codon> codons1 = addIn.Paths["/Path1"].Codons.ToList();
			Assert.AreEqual(1, codons1.Count);
			Assert.AreEqual("Sub", codons1[0].Name);
			Assert.AreEqual("Path2", codons1[0].Id);
			Assert.AreEqual(1, codons1[0].Conditions.Count);
			
			Assert.IsNotNull(addIn.Paths["/Path1/Path2"]);
			List<Codon> codons2 = addIn.Paths["/Path1/Path2"].Codons.ToList();
			Assert.AreEqual(1, codons2.Count);
			Assert.AreEqual("Codon2", codons2[0].Name);
			Assert.AreEqual("Sub2", codons2[0].Id);
			// condition is not inherited lexically
			Assert.AreEqual(0, codons2[0].Conditions.Count);
		}
		
		[Test]
		public void TestSimpleCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Condition name='Equal' string='a' equal='b'>
			<Simple id ='Simple' attr='a' attr2='b'/>
		</Condition>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count, "Paths != 1");
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			Codon codon = path.Codons.Single();
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition.
			Assert.AreEqual(1, codon.Conditions.Count);
			Condition condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
		}
		
		[Test]
		public void TestStackedCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<Condition name='Equal' string='a' equal='b'>
			<Condition name='StackedCondition' string='1' equal='2'>
				<Simple id ='Simple' attr='a' attr2='b'/>
			</Condition>
			<Simple id ='Simple2' attr='a' attr2='b'/>
		</Condition>
			<Simple id ='Simple3' attr='a' attr2='b'/>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			
			Assert.AreEqual(3, path.Codons.Count());
			Codon codon = path.Codons.ElementAt(0);
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition
			Assert.AreEqual(2, codon.Conditions.Count);
			Condition condition = codon.Conditions[1] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
			condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("StackedCondition", condition.Name);
			Assert.AreEqual("1", condition["string"]);
			Assert.AreEqual("2", condition["equal"]);
			
			codon = path.Codons.ElementAt(1);
			Assert.AreEqual(1, codon.Conditions.Count);
			condition = codon.Conditions[0] as Condition;
			Assert.IsNotNull(condition);
			Assert.AreEqual("Equal", condition.Name);
			Assert.AreEqual("a", condition["string"]);
			Assert.AreEqual("b", condition["equal"]);
			
			codon = path.Codons.ElementAt(2);
			Assert.AreEqual(0, codon.Conditions.Count);
			
		}
		
		[Test]
		public void TestComplexCondition()
		{
			string addInText = @"
<AddIn>
	<Path name = '/Path1'>
		<ComplexCondition>
			<And>
				<Not><Condition name='Equal' string='a' equal='b'/></Not>
				<Or>
					<Condition name='Equal' string='a' equal='b'/>
					<Condition name='Equal' string='a' equal='b'/>
					<Condition name='Equal' string='a' equal='b'/>
				</Or>
			</And>
			<Simple id ='Simple' attr='a' attr2='b'/>
		</ComplexCondition>
	</Path>
</AddIn>";
			AddIn addIn = AddIn.Load(addInTree, new StringReader(addInText));
			Assert.AreEqual(1, addIn.Paths.Count);
			ExtensionPath path = addIn.Paths["/Path1"];
			Assert.IsNotNull(path);
			Codon codon = path.Codons.Single();
			Assert.AreEqual("Simple", codon.Name);
			Assert.AreEqual("Simple", codon.Id);
			Assert.AreEqual("a",      codon["attr"]);
			Assert.AreEqual("b",      codon["attr2"]);
			
			// Test for condition.
			Assert.AreEqual(1, codon.Conditions.Count);
		}
		
		#endregion
	}
}
