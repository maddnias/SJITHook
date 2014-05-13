/*The MIT License (MIT)

Copyright (c) 2014 UbbeLoL

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SJITHook.Test
{
    public class Program
    {
        private static JITHook<ClrjitAddrProvider> _jitHook;

        private static unsafe void Main22()
        {
            _jitHook = new JITHook<ClrjitAddrProvider>();
            if (_jitHook.Hook(HookedCompileMethod))
                Console.WriteLine("Successfully installed hook!\r\n");

            Console.WriteLine(Foo());

            Console.WriteLine("\r\n");
            if (_jitHook.UnHook())
                Console.WriteLine("Successfully uninstalled hook!\r\n");

            Console.WriteLine(Bar());

            Console.ReadLine();
        }

        private static int Foo()
        {
            return 1000;
        }

        private static int Bar()
        {
            return 500;
        }

        private static unsafe int HookedCompileMethod(IntPtr thisPtr, [In] IntPtr corJitInfo,
            [In] Data.CorMethodInfo* methodInfo, Data.CorJitFlag flags,
            [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode)
        {
            int token;
            Console.WriteLine("Compilation:\r\n");
            Console.WriteLine("Token: " + (token = (0x06000000 + *(ushort*) methodInfo->methodHandle)).ToString("x8"));
            Console.WriteLine("Name: " + typeof (Program).Module.ResolveMethod(token).Name);
            Console.WriteLine("Body size: " + methodInfo->ilCodeSize);

            var bodyBuffer = new byte[methodInfo->ilCodeSize];
            Marshal.Copy(methodInfo->ilCode, bodyBuffer, 0, bodyBuffer.Length);

            Console.WriteLine("Body: " + BitConverter.ToString(bodyBuffer));

            /*
              Change output of "Foo" to 1337 instead of 1000:
             
              uint old;
              VirtualProtect(methodInfo->ilCode + 2, sizeof (int), 0x40, out old);
              *(int*) (methodInfo->ilCode + 2) = 1337;
              VirtualProtect(methodInfo->ilCode + 2, sizeof (int), old, out old);

             */

            return _jitHook.OriginalCompileMethod(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
        }
    }
    public class Program64
    {
        private static JITHook64<ClrjitAddrProvider> _jitHook;

        private static unsafe void Main()
        {
            _jitHook = new JITHook64<ClrjitAddrProvider>();
            if (_jitHook.Hook(HookedCompileMethod))
                Console.WriteLine("Successfully installed hook!\r\n");

            Console.WriteLine(Foo());

            Console.WriteLine("\r\n");
            if (_jitHook.UnHook())
                Console.WriteLine("Successfully uninstalled hook!\r\n");

            Console.WriteLine(Bar());

            Console.ReadLine();
        }

        private static int Foo()
        {
            return 1000;
        }

        private static int Bar()
        {
            return 500;
        }

        private static unsafe int HookedCompileMethod(IntPtr thisPtr, [In] IntPtr corJitInfo,
            [In] Data.CorMethodInfo64* methodInfo, Data.CorJitFlag flags,
            [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode)
        {
            int token;
            Console.WriteLine("Compilation:\r\n");
            Console.WriteLine("Token: " + (token = (0x06000000 + *(ushort*)methodInfo->methodHandle)).ToString("x8"));
            Console.WriteLine("Name: " + typeof(Program).Module.ResolveMethod(token).Name);
            Console.WriteLine("Body size: " + methodInfo->ilCodeSize);

            var bodyBuffer = new byte[methodInfo->ilCodeSize];
            Marshal.Copy(methodInfo->ilCode, bodyBuffer, 0, bodyBuffer.Length);

            Console.WriteLine("Body: " + BitConverter.ToString(bodyBuffer));

            /*
              Change output of "Foo" to 1337 instead of 1000:
             
              uint old;
              VirtualProtect(methodInfo->ilCode + 2, sizeof (int), 0x40, out old);
              *(int*) (methodInfo->ilCode + 2) = 1337;
              VirtualProtect(methodInfo->ilCode + 2, sizeof (int), old, out old);

             */

            return _jitHook.OriginalCompileMethod(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
        }
    }
}
