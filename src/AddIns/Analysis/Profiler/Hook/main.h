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

#pragma once

#ifndef STRICT
#define STRICT
#endif

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#ifndef WINVER				// Allow use of features specific to Windows XP or later.
#define WINVER 0x0501		// Change this to the appropriate value to target other versions of Windows.
#endif

#ifndef _WIN32_WINNT		// Allow use of features specific to Windows XP or later.                   
#define _WIN32_WINNT 0x0501	// Change this to the appropriate value to target other versions of Windows.
#endif						

#ifndef _WIN32_WINDOWS		// Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0410 // Change this to the appropriate value to target Windows Me or later.
#endif

#ifndef _WIN32_IE			// Allow use of features specific to IE 6.0 or later.
#define _WIN32_IE 0x0600	// Change this to the appropriate value to target other versions of IE.
#endif

#include <windows.h>
#include <stdio.h>
#include <iostream>
#include <time.h>
#include <wchar.h>
#include <string.h>
#include "tchar.h"
#include "stdlib.h"
#include <stdarg.h>
#include <intrin.h>

#define nullptr 0

#ifdef DEBUG
void DebugWriteLine(WCHAR* pszFmtString, ...);
#else
// Take a look at the assembly if you don't believe it: the compiler
// is too dumb to inline an empty method if it has varargs.
// So we'll use a macro instead.
#define DebugWriteLine(...) ((void)0)
#endif

// use intrinsics for InterlockedCompareExchange:

#ifndef _M_AMD64
inline void* _InterlockedCompareExchangePointer(void* volatile *Destination, void* ExChange, void* Comparand)
{
	return ((PVOID)(LONG_PTR)_InterlockedCompareExchange((LONG volatile *)Destination, (LONG)(LONG_PTR)ExChange, (LONG)(LONG_PTR)Comparand));
}

#define HOOK_MODULE_NAME L"Hook32.dll"
#else
#define HOOK_MODULE_NAME L"Hook64.dll"
#endif

#undef InterlockedCompareExchangePointer
#define InterlockedCompareExchangePointer _InterlockedCompareExchangePointer
#undef InterlockedCompareExchangePointerRelease
#define InterlockedCompareExchangePointerRelease _InterlockedCompareExchangePointer
#undef InterlockedCompareExchange
#define InterlockedCompareExchange _InterlockedCompareExchange
#undef InterlockedCompareExchangeRelease
#define InterlockedCompareExchangeRelease _InterlockedCompareExchange
