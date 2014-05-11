SJITHook
========

Simple JIT compiler hook in C#.

Compatibility
=============
It supports both x64 and x86 applications, and supports .NET version from 2.0 to 4.5

How to use
==========
* Add a reference to either SJITHook or SJITHook.Compact
* Determine what .NET version the target assembly is using, and pass the according VTableAddrProvider to JITHook:
   
```csharp
    // .NET 2.0->3.5
    var hook = new JITHook<MscorjitAddrProvider>();
    // .NET 4.0->4.5
    var hook = new JITHook<ClrjitAddrProvider>();
```
* Install the hook, and pass your custom CompileMethod delegate:

```csharp
    if (hook.Hook(HookedCompileMethod))
        Console.WriteLine("Successfully installed hook!");
            
    private static unsafe int HookedCompileMethod(IntPtr thisPtr, [In] IntPtr corJitInfo,
            [In] Data.CorMethodInfo* methodInfo, Data.CorJitFlag flags,
            [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode)
        {
            /*
                Do whatever you want with the parameters here
            */
            return hook.OriginalCompileMethod(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
        }
```
* Finally uninstall the hook when you're done using it:

```csharp
    if (hook.UnHook())
        Console.WriteLine("Successfully uninstalled hook!");
``` 
Remarks
=======
SJITHook is simply replacing the entry in CILJit's VTable, and not writing any instructions in the actual compileMethod function.
