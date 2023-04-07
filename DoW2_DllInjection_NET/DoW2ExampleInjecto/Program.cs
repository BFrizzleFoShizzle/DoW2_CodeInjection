using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using cope.Debug;

namespace DoW2ExampleInjecto
{
    class Program
    {
        static void Main(string[] args)
        {
            // get the DoW2 process
            // we can't start DoW2.exe ourselves because launching DoW2.exe results in DoW2.exe
            // re-launching itself via Steam
            Process[] ps = Process.GetProcessesByName("DoW2");
            while (ps.Length == 0)
            {
                Thread.Sleep(1000);
                ps = Process.GetProcessesByName("DoW2");
            }

            Console.WriteLine("Found DoW2 process!");
            // next: we've found the DoW2 process but we can't just inject DLLs
            // DoW2 is protected by Xlive.dll (Game for Windows) which continously checks memory for changes
            // the following byte-array contains the signature for the memory-check:
            byte[] xlivesig = new byte[] { 0x8B, 0xFF, 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x20, 0x53, 0x56, 0x57, 0x8D, 0x45, 0xE0, 0x33, 0xF6, 0x50, 0xFF, 0x75, 0x0C };
            // we'll replace the memory-check so it just exists
            byte[] retnC = new byte[] { 0xC2, 0x0C, 0x00 };

            Process dow2 = ps[0];

            // now for the replacement:
            // Process.ReplaceSequence is an extension method found in cope.Debug which searches
            // a module ('xlive.dll') of a process (dow2) for a byte sequence ('xlivesig' in this case)
            // and replaces it with another sequence of bytes ('retnC')
            // the last argument to ReplaceSequence tells the method to only replace the first occurence
            // of the byte sequence we're searching -> while there is only one occurence of xlivesig
            // in xlive.dll, this is useful so the algorithm stops as soon as it replaced that occurence.
            // This also means, that using this method on xlive.dll twice without restarting DoW2 consumes
            // a lot of time the second time you're using it.
            // Oh, and ReplaceSequence returns the amount of occurences it replaced
            int replacementCount = dow2.ReplaceSequence(xlivesig, retnC, "xlive.dll", 1);
            Console.WriteLine("count = " + replacementCount.ToString());

            // here comes the more interesting part:
            // Process.InjectForwardOperationalBase is another extension method found in cope.Debug
            // Basically it does the following:
            // 1. Inject a native x86 DLL (cope.ManagedLoader.dll).
            // 2. cope.ManagedLoader.dll loads up the .NET-system in the target process.
            // 3. cope.FOB.dll is then loaded by cope.ManagedLoader.dll, cope.FOB.dll already is
            //    a .NET assembly.
            // 4. cope.FOB.dll sets up a local service which you can connect to using named pipes
            // 5. You don't have to manually connect, InjectForwardOperationalBase returns a
            //    a ForwardPortClient which already established a connection to the service.
            // WARNING: Don't try to connect to the service TWICE as this will currently crash the program
            //          which attempts to connect second.
            // NOTE: Both cope.FOB.dll and cope.ManagedLoader.dll need to be in the same directory as
            //       the launcher!
            ForwardPortClient client = dow2.InjectForwardOperationalBase();

            // now we're going to load a .NET DLL into DoW2:
            // The client grants you the ability to load .NET assemblies
            // LoadAssemblyAndStartMethod has the following syntax:
            // 1. argument: Full path to the assembly you want to inject.
            // 2. argument: Class which holds the method you want to start.
            // 3. argument: Name of the method you want to start.
            // 4. argument: Set to true if the method is static.
            string currentDir = Directory.GetCurrentDirectory() + '\\';
            client.LoadAssemblyAndStartMethod(currentDir + "DoW2InjecteeExample.dll", "DoW2InjecteeExample.Inject", "ExampleMethod", true);

            // Further note:
            // You can inject non-.NET DLLs using Process.Inject (also an extension method found in cope.Debug)
            Console.Read();
        }
    }
}
