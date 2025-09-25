using System.Runtime.InteropServices;

namespace TestRevify.Api.Facade;

public static partial class RevifyFacade
{
    public const string AssemblyName = "liblogger.so";

    [DllImport(AssemblyName, EntryPoint = "Logger_testSuma", CallingConvention = CallingConvention.Cdecl)]
    public static extern int LoggerTestSuma(int arg1, int arg2);

    [DllImport(AssemblyName, EntryPoint = "logger_WaitSeconds", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoggerWaitSeconds(int arg1);

    public async static ValueTask<int> ExternalInvoke(int n1, int n2)
    {
        //call to C++ function
        try
        {
            return await Task.Run(() => LoggerTestSuma(n1, n2));
        }
        catch
        {
            throw;
        }
    }

    public async static ValueTask ExternalSleepInvoke(int n1)
    {
        //call to C++ function
        try
        {
            await Task.Run(() => LoggerWaitSeconds(n1));
        }
        catch
        {
            throw;
        }
    }
}
