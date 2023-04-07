using System.Runtime.InteropServices;

namespace DoW2InjecteeExample
{
    public class Inject
    {
        // C# unfortunately does not support varargs
        // but you can of course overload TimeStampedTrace
        // public extern static void TimeStampedTrace(string s, double d) <- possible
        [DllImport("Debug.dll", SetLastError = true,
        EntryPoint = "?TimeStampedTracef@dbInternal@@YAXPBDZZ",
        CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void TimeStampedTrace(string s);

        public static void Trace(string s)
        {
            TimeStampedTrace(s);
        }

        public static void ExampleMethod()
        {
            TimeStampedTrace("EXAMPLE MESSAGE SENT BY INJECTED DLL");
        }
    }
}
