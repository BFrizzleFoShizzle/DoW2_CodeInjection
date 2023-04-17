using System;
using System.Runtime.InteropServices;

namespace ModDebug
{
    static public class DoW2Bridge
    {
        static LuaBridge _luaBridge;
        static IntPtr _worldPtr = IntPtr.Zero; // pointer to World-object used by DoW2
        static object _worldLock = new object();
        delegate int LuaHandler(IntPtr ptr); // delegate for LUA functions
        static LuaHandler _luaHandler = null; // the current LuaHandler; gets assigned in LuaInit()

        // Test function to be called by LUA
        static int LuaTest(IntPtr state)
        {
            TimeStampedTrace("LUA CALL");
            return 1;
        }

        /// <summary>
        /// Function called by the injected DLL, all the functions you want to add to the LUA state go here.
        /// Init of LuaBridge happens in here.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        static public int LuaCallHandler(IntPtr state)
        {
            TimeStampedTrace("Lua Handler called");
            _luaBridge = new LuaBridge(state);
            _luaBridge.RegisterLuaFunction(LuaTest, "CopeLua_Test");
            return 1;
        }

        /// <summary>
        /// Initializes the bridge to DoW2, called by DebugManager.Init().
        /// </summary>
        static public void LuaInit()
        {
            TimeStampedTrace("CopeDebug - Lua Init");
            _luaHandler = LuaCallHandler; // setup a handler for LUA calls by the injected CopeLua.dll
            try
            {
                SetLuaHandler(Marshal.GetFunctionPointerForDelegate(_luaHandler));
            }
            catch (Exception ex)
            {
                TimeStampedTrace("CopeDebug - Lua Init Failed");
                TimeStampedTrace(ex.Message);
                return;
            }
            TimeStampedTrace("CopeDebug - Lua Init Finished");
        }

        /// <summary>
        /// Returns the World pointer of DoW2, may be useful in some cases.
        /// </summary>
        /// <returns></returns>
        static public IntPtr GetWorldPtr()
        {
            lock (_worldLock)
            {
                _worldPtr = DebugManager.CurrentProcess.GetProcAddress("?g_World@@3PAVWorld@@A", "SimEngine.dll");
            }
            return _worldPtr;
        }

        #region wrappers

        /// <summary>
        /// Returns the base path of the PropertyGroupManager of DoW2.
        /// </summary>
        /// <returns></returns>
        static public string PropertyGroupManager_GetBasePath()
        {
            try
            {
                IntPtr instance = PropertyGroupManager_Instance();
                IntPtr path = PGM_GetBasePath(instance);
                return Marshal.PtrToStringAnsi(path);
            }
            catch
            {
                return null;
            }
        }

        #endregion wrappers

        #region imports

        // used for printing stuff to the DoW2 console
        [DllImport("Debug.dll", SetLastError = true,
            EntryPoint = "?TimeStampedTracef@dbInternal@@YAXPBDZZ",
            CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static public extern void TimeStampedTrace(string s);

        // import from native x86 DLL used for getting access to the LUA system
        [DllImport("CopeLua.dll", EntryPoint = "SetLuaHandler", CallingConvention = CallingConvention.StdCall)]
        static public extern void SetLuaHandler(IntPtr func);

        #region SimEngine

        // various useful functions for accessing subsystems from the game
        // GetWorldPtr() -> World_GetSimManager() -> SimManager_GetScar() -> Scar_GetState = LuaState
        [DllImport("SimEngine.dll", EntryPoint = "?GetState@Scar@@QAEPAVLuaConfig@@XZ",
            CallingConvention = CallingConvention.ThisCall)]
        static public extern IntPtr Scar_GetState(IntPtr scar);

        [DllImport("SimEngine.dll", EntryPoint = "?GetScar@SimManager@@QBEPBVScar@@XZ",
            CallingConvention = CallingConvention.ThisCall)]
        static public extern IntPtr SimManager_GetScar(IntPtr simManager);

        [DllImport("SimEngine.dll", EntryPoint = "?GetSimManager@World@@QAEPAVSimManager@@XZ",
            CallingConvention = CallingConvention.ThisCall)]
        static public extern IntPtr World_GetSimManager(IntPtr world);

        // returns a pointer to the instance of PropertyGroupManager
        [DllImport("SimEngine.dll", EntryPoint = "?Instance@PropertyBagGroupManager@@SGAAV1@XZ",
            CallingConvention = CallingConvention.StdCall)]
        static public extern IntPtr PropertyGroupManager_Instance();

        // PGM == PropertyGroupManager
        [DllImport("SimEngine.dll", CharSet = CharSet.Ansi,
            EntryPoint = "?GetBasePath@PropertyBagGroupManager@@QBEPBDXZ",
            CallingConvention = CallingConvention.ThisCall)]
        static extern IntPtr PGM_GetBasePath(IntPtr pgm);

        [DllImport("SimEngine.dll", CharSet = CharSet.Ansi,
            EntryPoint = "?GetGroup@PropertyBagGroupManager@@QAEPBVPropertyBagGroup@@PBD@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static public extern IntPtr PropertyGroupManager_GetGroup(IntPtr pgm, string name);

        // reloading RBFs
        [DllImport("SimEngine.dll", CharSet = CharSet.Ansi,
            EntryPoint = "?ReloadGroup@PropertyBagGroupManager@@QAEPBVPropertyBagGroup@@PBD@Z",
            CallingConvention = CallingConvention.ThisCall)]
        static public extern IntPtr PropertyGroupManager_ReloadPropertyGroup(IntPtr pgm, string path);

        #endregion SimEngine

        #endregion imports
    }
}