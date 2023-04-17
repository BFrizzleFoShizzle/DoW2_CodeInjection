using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ModDebug
{
    public static class DebugManager
    {
        static Process _currentProcess = null;

        public static void Init()
        {
            DoW2Bridge.TimeStampedTrace("CopeDebug - Establishing Cope's Forward Operations Base!");
            _currentProcess = Process.GetCurrentProcess();
            DoW2Bridge.LuaInit();
            DoW2Bridge.TimeStampedTrace("CopeDebug - Setup finished!");
        }

        public static Process CurrentProcess
        {
            get
            {
                return _currentProcess;
            }
        }
    }
}