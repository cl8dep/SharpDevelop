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

namespace Debugger.Tests
{
	public class DebugType_CompilerGeneratedClasses
	{
		delegate void IntDelegate(int i);
		
		public static void Main()
		{
			new List<object>(new DebugType_CompilerGeneratedClasses().MyEnum());
		}
		
		string instanceField = "instance field value";
		static string staticField = "static field value";
			
		IEnumerable<object> MyEnum()
		{
			int stateFullVar = 101;
			int stateFullVar_DelegRef = 102;
			int stateFullVar_NestedDelegRef = 103;
			
			yield return stateFullVar + stateFullVar_DelegRef + stateFullVar_NestedDelegRef;
			
			{
				int stateLessVar = 201;
				int stateLessVar_DelegRef = 202;
				int stateLessVar_NestedDelegRef = 203;
				
				System.Diagnostics.Debugger.Break();
				
				IntDelegate deleg = delegate (int delegArg_NestedDelegRef) {
					int delegVar = 301;
					int delegVar_NestedDelegRef = 302;
					Console.WriteLine(stateFullVar_DelegRef);
					Console.WriteLine(stateLessVar_DelegRef);
					
					IntDelegate nestedDeleg = delegate (int nestedDelegArg) {
						int nestedDelegVar = 303;
						Console.WriteLine(delegArg_NestedDelegRef);
						Console.WriteLine(delegVar_NestedDelegRef);
						Console.WriteLine(stateFullVar_NestedDelegRef);
						Console.WriteLine(stateLessVar_NestedDelegRef);
						Console.WriteLine(this);
						
						System.Diagnostics.Debugger.Break();
					};
					
					System.Diagnostics.Debugger.Break();
					nestedDeleg(402);
				};
				deleg(401);
			}
		}
	}
}

#if TEST_CODE
namespace Debugger.Tests {
	using System.Linq;
	
	public partial class DebuggerTests
	{
		[NUnit.Framework.Test]
		public void DebugType_CompilerGeneratedClasses()
		{
			StartTest();
			DumpLocalVariables("YieldLocalVariables");
			process.Continue();
			DumpLocalVariables("OutterDelegateLocalVariables");
			process.Continue();
			DumpLocalVariables("InnterDelegateLocalVariables");
			
			evalContext = GetResource("DebugType_CompilerGeneratedClasses.cs");
			
			AssertEval("nestedDelegArg", "402");
			AssertEval("instanceField", "\"instance field value\"");
			AssertEval("staticField", "\"static field value\"");
			
			EndTest();
		}
	}
}
#endif

#if EXPECTED_OUTPUT
<?xml version="1.0" encoding="utf-8"?>
<DebuggerTests>
  <Test
    name="DebugType_CompilerGeneratedClasses.cs">
    <Started />
    <ModuleLoaded>mscorlib.dll (No symbols)</ModuleLoaded>
    <ModuleLoaded>DebugType_CompilerGeneratedClasses.exe (Has symbols)</ModuleLoaded>
    <Paused>DebugType_CompilerGeneratedClasses.cs:49,5-49,41</Paused>
    <YieldLocalVariables>
      <Item>
        <LocalVariable
          Name="stateLessVar"
          Type="System.Int32"
          Value="201" />
      </Item>
      <Item>
        <LocalVariable
          Name="deleg"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses+IntDelegate"
          Value="null" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_DelegRef"
          Type="System.Int32"
          Value="202" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_NestedDelegRef"
          Type="System.Int32"
          Value="203" />
      </Item>
      <Item>
        <LocalVariable
          Name="this"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses"
          Value="{Debugger.Tests.DebugType_CompilerGeneratedClasses}" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar"
          Type="System.Int32"
          Value="101" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_DelegRef"
          Type="System.Int32"
          Value="102" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_NestedDelegRef"
          Type="System.Int32"
          Value="103" />
      </Item>
      <Item>
        <LocalVariable
          Name="this"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses"
          Value="{Debugger.Tests.DebugType_CompilerGeneratedClasses}" />
      </Item>
    </YieldLocalVariables>
    <Paused>DebugType_CompilerGeneratedClasses.cs:68,6-68,42</Paused>
    <OutterDelegateLocalVariables>
      <Item>
        <LocalVariable
          Name="delegVar"
          Type="System.Int32"
          Value="301" />
      </Item>
      <Item>
        <LocalVariable
          Name="nestedDeleg"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses+IntDelegate"
          Value="{Debugger.Tests.DebugType_CompilerGeneratedClasses.IntDelegate}" />
      </Item>
      <Item>
        <LocalVariable
          Name="delegVar_NestedDelegRef"
          Type="System.Int32"
          Value="302" />
      </Item>
      <Item>
        <LocalVariable
          Name="delegArg_NestedDelegRef"
          Type="System.Int32"
          Value="401" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_DelegRef"
          Type="System.Int32"
          Value="202" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_NestedDelegRef"
          Type="System.Int32"
          Value="203" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_DelegRef"
          Type="System.Int32"
          Value="102" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_NestedDelegRef"
          Type="System.Int32"
          Value="103" />
      </Item>
      <Item>
        <LocalVariable
          Name="this"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses"
          Value="{Debugger.Tests.DebugType_CompilerGeneratedClasses}" />
      </Item>
    </OutterDelegateLocalVariables>
    <Paused>DebugType_CompilerGeneratedClasses.cs:65,7-65,43</Paused>
    <InnterDelegateLocalVariables>
      <Item>
        <LocalVariable
          Name="nestedDelegVar"
          Type="System.Int32"
          Value="303" />
      </Item>
      <Item>
        <LocalVariable
          Name="delegVar_NestedDelegRef"
          Type="System.Int32"
          Value="302" />
      </Item>
      <Item>
        <LocalVariable
          Name="delegArg_NestedDelegRef"
          Type="System.Int32"
          Value="401" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_DelegRef"
          Type="System.Int32"
          Value="202" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateLessVar_NestedDelegRef"
          Type="System.Int32"
          Value="203" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_DelegRef"
          Type="System.Int32"
          Value="102" />
      </Item>
      <Item>
        <LocalVariable
          Name="stateFullVar_NestedDelegRef"
          Type="System.Int32"
          Value="103" />
      </Item>
      <Item>
        <LocalVariable
          Name="this"
          Type="Debugger.Tests.DebugType_CompilerGeneratedClasses"
          Value="{Debugger.Tests.DebugType_CompilerGeneratedClasses}" />
      </Item>
    </InnterDelegateLocalVariables>
    <Exited />
  </Test>
</DebuggerTests>
#endif // EXPECTED_OUTPUT
