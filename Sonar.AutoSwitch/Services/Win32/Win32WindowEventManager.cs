using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Sonar.AutoSwitch.Services.Win32;

public interface IWindowEventManager
{
    void SubscribeToWindowEvents();
    void UnsubscribeToWindowsEvents();
    event EventHandler<WindowInfo> ForegroundWindowChanged;
}

public class Win32WindowEventManager : IWindowEventManager
{
    private readonly WinEventProc _lpfnWinEventProc;
    private IntPtr _windowEventHook;

    public Win32WindowEventManager()
    {
        _lpfnWinEventProc = WindowEventCallback;
    }

    public void SubscribeToWindowEvents()
    {
        if (_windowEventHook == IntPtr.Zero)
        {
            _windowEventHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, // eventMin
                EVENT_SYSTEM_FOREGROUND, // eventMax
                IntPtr.Zero, // hmodWinEventProc
                _lpfnWinEventProc, // lpfnWinEventProc
                0, // idProcess
                0, // idThread
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

            if (_windowEventHook == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }

    public void UnsubscribeToWindowsEvents()
    {
        if (_windowEventHook != IntPtr.Zero)
            UnhookWinEvent(_windowEventHook);
        _windowEventHook = IntPtr.Zero;
    }

    public event EventHandler<WindowInfo>? ForegroundWindowChanged;

    protected virtual void OnForegroundWindowChanged(WindowInfo e)
    {
        ForegroundWindowChanged?.Invoke(this, e);
    }

    private void WindowEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject,
        int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (GetWindowThreadProcessId(hwnd, out var pid) == 0)
            return;
        try
        {
            using var processById = Process.GetProcessById((int) pid);
            string? fileName = processById.MainModule?.FileName;
            if (string.IsNullOrEmpty(fileName))
                return;
            string name = Path.GetFileNameWithoutExtension(fileName);
            OnForegroundWindowChanged(new WindowInfo(name));
        }
        catch (Exception)
        {
            // ignored
        }
    }


    #region Win32

    private delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc,
        WinEventProc lpfnWinEventProc, int idProcess, int idThread, int dwflags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int UnhookWinEvent(IntPtr hWinEventHook);


    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    private const int WINEVENT_INCONTEXT = 4;
    private const int WINEVENT_OUTOFCONTEXT = 0;
    private const int WINEVENT_SKIPOWNPROCESS = 2;
    private const int WINEVENT_SKIPOWNTHREAD = 1;
    private const int EVENT_SYSTEM_FOREGROUND = 3;

    #endregion
}