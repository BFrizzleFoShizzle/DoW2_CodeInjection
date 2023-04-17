Adding functions to SCAR using DLL injection
--------------------------------------------

The example Dll for this code can be found in the ModDebug subfolder.
The injection example (previously released) are included as well (see InjectionExample).


1. Inject "LuaLibLoad.dll" using any method you like; I suggest using the example I provided for injecting .NET assemblies and x86 Dlls. (use <Process-object>.InjectDll(...) to inject x86 Dlls and <ForwardPortClient-object>.LoadAssemblyAndStartMethod(...) for .NET assemblies) "LuaLibLoad.dll" is native-x86 so inject it as an x86 Dll (ergo non-.NET).

2. "LuaLibLoad.dll" extends the loadfile-function of LUA to enable you to load DLLs and any function they contain at runtime. Add the following code to your main SCAR-script:
	
	loadfile("CopeLua.dll", "CopeLua_Init")()
	
Of course you need to ensure that both the "LuaLibLoad.dll" and the "CopeLua.dll" are in your DoW2 directory. Both files are included in the download which this ReadMe accompanies.

3. "CopeLua.dll" is a x86 DLL as well and doesn't actually do anything but register one single function which can be used in SCAR:
	
	copeLib.CopeLua_Call()

This function will call a handler which can do whatever you want. The "CopeLua.dll" exposes a function called SetLuaHandler(void* handler) which we will use to set a custom handler.

4. Setup a new .NET DLL or use the sources coming with this download. This DLL will need to be injected as well; it's generally preferable to inject both "LuaLibLoad.dll" and this DLL at the start of DoW2. I recommend injecting "LuaLibLoad.dll" first but in practice it shouldn't actually matter. Your way of injecting that new DLL should allow you to call a function within that DLL so your DLL can setup the LUA-handler using the SetLuaHandler-function (if you're using my example code, your injector should make sure to call "DebugManager.Init()").

5. This new DLL will need to import the aforementioned SetLuaHandler(void*) function from the "CopeLua.dll" to setup a custom LUA-Handler. This handler receives one single parameter, which is the LUA state used by SCAR. If you don't feel like setting up this system yourself, you can simply use the sources I provide within this package.

6. Your handler can do all kinds of funky stuff like registering new LUA functions. Take a look at the "LuaCallHandler(IntPtr state)" function from my example code ("DoW2Bride.cs") to see a simply straight forward example of adding a function to LUA. 

-cope.