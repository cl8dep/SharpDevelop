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

#include "Callback.h"

#ifndef _M_AMD64

// Disable "unreferenced formal parameter" warning
#pragma warning( disable : 4100 )

// this function is called by the CLR when a function has been entered
void _declspec(naked) FunctionEnterNaked(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func, COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{
	__asm
	{
		#define CallPopSize 0
		#define SavedRegisterOffset CallPopSize
		#define SavedRegister(X) [esp + SavedRegisterOffset + X]
		#define SavedRegisterSize 12
		#define FrameSize (SavedRegisterOffset + SavedRegisterSize)
		
		// stack layout:
		// esp +  0   -> saved edx
		//     +  4   -> saved ecx
		//     +  8   -> saved eax
		//     + 12   -> return address
		//     + 16   -> functionID      \
		//     + 20   -> clientData      |- parameters for FunctionEnterNaked
		//     + 24   -> frameInfo       |
		//     + 28   -> argumentInfo    /
		
		sub esp, FrameSize
		// eax, ecx and edx are scratch registers in fastcall, so we need to save those
		mov SavedRegister(8), eax
		mov SavedRegister(4), ecx
		mov SavedRegister(0), edx
		
		mov ecx, [esp+FrameSize+8] // get clientData = custom FunctionID
		// first argument to FunctionEnterGlobal is in ecx (__fastcall)
		
		call    FunctionEnterGlobal
		// the call causes CallPopSize bytes to be popped from the stack
		
		mov edx, SavedRegister(0 - CallPopSize)
		mov ecx, SavedRegister(4 - CallPopSize)
		mov eax, SavedRegister(8 - CallPopSize)
		
		add esp, FrameSize - CallPopSize
		ret    16
		
		#undef SavedRegisterSize
	}
}

// this function is called by the CLR when a function is exiting
void _declspec(naked) FunctionLeaveNaked(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange)
{
	__asm
	{
		#define CallPopSize 0
		#define SavedRegisterOffset CallPopSize
		#define SavedRegister(X) [esp + SavedRegisterOffset + X]
		#define SavedFPRegister(X) [esp + SavedRegisterOffset + 12 + X]
		#define SavedFloatRegisterSize 12
		#define SavedRegisterSize 12 + SavedFloatRegisterSize
		#define FrameSize (SavedRegisterOffset + SavedRegisterSize)
		
		sub esp, FrameSize
		mov SavedRegister(8), eax
		mov SavedRegister(4), ecx
		mov SavedRegister(0), edx
		
		// Check if there's anything on the FP stack.
		//
		// Again technically you need only save what you use. You might think that
		// FP regs are not commonly used in the kind of code you'd write in these,
		// but there are common cases that might interfere. For example, in the 8.0 MS CRT, 
		// memcpy clears the FP stack.
		//
		// In CLR versions 1.x and 2.0, everything from here to NoSaveFPReg
		// is only strictly necessary for FunctionLeave and FunctionLeave2.
		// Of course that may change in future releases, so use this code for all of your
		// enter/leave function hooks if you want to avoid breaking.
		fstsw   ax
		test    ax, 3800h		// Check the top-of-fp-stack bits
		jnz     SaveFPReg
		mov     dword ptr SavedFPRegister(0), 0 // otherwise, mark that there is no float value
		jmp     NoSaveFPReg
	SaveFPReg:
		fstp    qword ptr SavedFPRegister(4) // Copy the FP value to the buffer as a double
		mov     dword ptr SavedFPRegister(0), 1	// mark that a float value is present
	NoSaveFPReg:
		
		
		call    FunctionLeaveGlobal
		// the call causes CallPopSize bytes to be popped from the stack
		
		// Now see if we have to restore any floating point registers
		// In CLR versions 1.x and 2.0, everything from here to 
		// RestoreFPRegsDone is only strictly necessary for FunctionLeave and FunctionLeave2
		// Of course that may change in future releases, so use this code for all of your
		// enter/leave function hooks if you want to avoid breaking.
		cmp     dword ptr SavedFPRegister(0 - CallPopSize), 0		// Check the flag
		jz      NoRestoreFPRegs		// If zero, no FP regs
	//RestoreFPRegs:
		fld     qword ptr SavedFPRegister(4 - CallPopSize)	// Restore FP regs
	NoRestoreFPRegs:
	//RestoreFPRegsDone:
		
		mov edx, SavedRegister(0 - CallPopSize)
		mov ecx, SavedRegister(4 - CallPopSize)
		mov eax, SavedRegister(8 - CallPopSize)
		add esp, FrameSize - CallPopSize
		ret    16
		
		#undef SavedRegisterSize
	}
}

// this function is called by the CLR when a tailcall occurs.  A tailcall occurs when the 
// last action of a method is a call to another method.
void _declspec(naked) FunctionTailcallNaked(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func)
{
	__asm
	{
		#define CallPopSize 0
		#define SavedRegisterOffset CallPopSize
		#define SavedRegister(X) [esp + SavedRegisterOffset + X]
		#define SavedRegisterSize 12
		#define FrameSize (SavedRegisterOffset + SavedRegisterSize)
		
		sub esp, FrameSize
		// eax, ecx and edx are scratch registers in stdcall, so we need to save those
		mov SavedRegister(8), eax
		mov SavedRegister(4), ecx
		mov SavedRegister(0), edx
				
		call    FunctionTailcallGlobal
		// the call causes CallPopSize bytes to be popped from the stack
				
		mov edx, SavedRegister(0 - CallPopSize)
		mov ecx, SavedRegister(4 - CallPopSize)
		mov eax, SavedRegister(8 - CallPopSize)
		
		add esp, FrameSize - CallPopSize
		ret    12
	}
}
#endif
