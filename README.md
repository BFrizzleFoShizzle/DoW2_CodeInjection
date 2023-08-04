# DoW2 Code Injection
This repository is a historical archive of publicly posted projects for extending Dawn of War 2 via code injection.

The files are uploaded exactly as they were sent to me with zero modifications (excluding `cope.Debug.fixed`).  
Unless otherwise specified, all files have been tested as working in Dawn of War 2 Retribution v3.19.1.10320

Most of the files come from [this post](https://web.archive.org/web/20140914215052/http://forums.relicnews.com/showthread.php?242502-TOOL-DLL-Loader-NET-injection-Adding-SCAR-functions-%2810-12-10%29%29), Copernicus and Corsix are the original authors.

## DoW2_DllInjection_NET
This project has two sub-projects, both written in C#
### DoW2ExampleInjecto
This is a DLL injector for DoW2 which also includes an `Xlive` bypass.  
This project has two issues:  
1) A case-sensitivity bug in `cope.Debug.ProcExt.GetModuleByName` stops the inejctor from detecting the DoW2 process
	- I created a fixed version of `cope.Debug.dll` which can be found in `./cope.Debug.fixed/` - overwrite the old DLL with this to fix the issue
2) DoW2 no longer uses `Xlive`
	- This can be fixed by commenting out the `ReplaceSequence` call in `Program.cs`
### DoW2InjecteeExample
This contains example code for an injected C# DLL, loadable via `DoW2ExampleInjecto`

## DoW2_ScarInjection
This project contains source code for an example DLL `ModDebug` that can be used to extend the SCAR scripting system.  
This project also appears to contain all the files from `DoW2_DllInjection_NET`.  
You can find more information on both C# and C++ DLL injection in the readme for this project.  
### ModDebug
Besides showing how to inject new functions into SCAR, this project also contains  example code  for interacting with Lua/registering Lua functions.  
After the DLL created by this project is injected, the function `DebugManager.Init()` must be called by external code. Adding a static initializer to `DebugManager`  that calls `Init()` removes this requirement.  
This project also contains symbol names for several useful DoW2 engine functions, which can be found in `DoW2Bridge.cs`.

## DoW2_FixedWorldBuilder
Enables Action Marker usage in the worldbuilder. This appears to work for all current DoW2 versions.

## DoW2_LuaLibLoad
This appears to be the source for `LuaLibLoad.dll` - I haven't tested this code.  
This project modifies the Lua funciton `loadfile` to allow loading of DLLs  
It looks like `GetLuaLoadfileAddress` might not be implemented?

## DoW2_DllLoad
This project is a DLL injector, specifically written for DoW2. It has multiple bugs that cause it to no longer work on the current version(s) of DoW2.  
It appears to  have the same case sensitivity  issue as the injector in `DoW2_DllInjection_NET` - I made a fix for this, but the injector appears to have other issues as well.  
The injector also contains the same `Xlive` bypass as in `DoW2_DllInjection_NET`.  
The DLL files in this repo can be injected via any DLL injector, so I have no plans to fix `DoW2_DllLoad` - there's plenty of other working DLL injectors out there that you can use in its place.

# Disclaimer
THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
