using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sonar.AutoSwitch.Services.Win32;

public static class MemoryUtils
{
    [DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    public static void MinimizeFootprint()
    {
        using var currentProcess = Process.GetCurrentProcess();
        EmptyWorkingSet(currentProcess.Handle);
    }
}