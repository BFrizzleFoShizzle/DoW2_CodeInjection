DoW2-Injector example
---------------------

1. Open up DoW2ExampleInjecto\DoW2ExampleInjecto.sln.
2. Fix the references to cope.Debug (found in DoW2ExampleInjecto\DLLs).
3. Compile.
4. Copy cope.Debug.dll, cope.dll and cope.FOB.dll and cope.ManagedLoader into the directory of the loader.

DoW2ExampleInjecto\Program.cs contains an example for an injector.
DoW2InjecteeExample\Inject.cs contains an example DLL which can be injected.

-cope.