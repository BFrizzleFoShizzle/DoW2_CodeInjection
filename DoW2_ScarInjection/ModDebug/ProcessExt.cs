using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ModDebug
{
    /// <summary>
    /// Extensions for the Process class.
    /// </summary>
    static class ProcessExt
    {
        static public IntPtr GetProcAddress(this Process p, string name, string moduleName)
        {
            return GetProcAddress(p, name, p.GetModuleByName(moduleName));
        }

        static public IntPtr GetProcAddress(this Process p, string name, ProcessModule m = null)
        {
            if (m == null)
                m = p.MainModule;
            return krnl32GetProcAddress(m.BaseAddress, name);
        }

        static public ProcessModule GetModuleByName(this Process p, string name)
        {
            foreach (ProcessModule m in p.Modules)
            {
                if (m.ModuleName == name)
                    return m;
            }
            return null;
        }

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        static extern IntPtr krnl32GetProcAddress(IntPtr module, string name);
    }
}
