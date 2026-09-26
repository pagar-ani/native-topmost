using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

internal static class Program {
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void TimerDelegate(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool ConsoleCtrlDelegate(uint ctrlType);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

    [DllImport("user32.dll")]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex) {
        if (IntPtr.Size == 8)
            return GetWindowLongPtr64(hWnd, nIndex);
        else
            return new IntPtr(GetWindowLong32(hWnd, nIndex));
    }

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
        if (IntPtr.Size == 8)
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        else
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessageW(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerDelegate lpTimerFunc);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);

    [DllImport("user32.dll")]
    private static extern bool MessageBeep(uint uType);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref bool pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint SetThreadExecutionState(uint esFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmRegisterThumbnail(IntPtr hwndDest, IntPtr hwndSrc, out IntPtr phThumbnailId);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmUnregisterThumbnail(IntPtr hThumbnailId);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnailId, ref DWM_THUMBNAIL_PROPERTIES props);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, uint dwFlags);

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct WNDCLASSEX {
        public uint cbSize;
        public uint style;
        public WndProcDelegate lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DWM_THUMBNAIL_PROPERTIES {
        public uint dwFlags;
        public RECT rcDestination;
        public RECT rcSource;
        public byte opacity;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fVisible;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fSourceClientAreaOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GUITHREADINFO {
        public int cbSize;
        public uint flags;
        public IntPtr hwndActive;
        public IntPtr hwndFocus;
        public IntPtr hwndCapture;
        public IntPtr hwndMenuOwner;
        public IntPtr hwndMoveSize;
        public IntPtr hwndCaret;
        public RECT rcCaret;
    }

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOSENDCHANGING = 0x0400;

    private const uint SWP_STEADY_FLAGS = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOSENDCHANGING;

    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_WIN = 0x0008;
    private const uint VK_T = 0x54;
    private const int HOTKEY_ID = 9001;

    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    private const uint WINEVENT_SKIPOWNPROCESS = 0x0002;

    private const uint GA_ROOT = 2;
    private const uint GW_HWNDNEXT = 2;
    private const uint GW_HWNDPREV = 3;
    private const uint GW_CHILD = 5;
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOPMOST = 0x00000008L;
    private const long WS_EX_LAYERED = 0x00080000L;
    private const long WS_EX_NOACTIVATE = 0x08000000L;

    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWCP_DEFAULT = 0;
    private const int DWMWCP_DONOTROUND = 1;
    private const int DWMWCP_ROUNDSMALL = 3;

    private const uint DWM_TNP_VISIBLE = 0x00000008;
    private const uint DWM_TNP_OPACITY = 0x00000004;

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;

    private const uint NOTIFY_FOR_THIS_SESSION = 0;
    private const uint WM_WTSSESSION_CHANGE = 0x02B1;
    private const uint WTS_SESSION_UNLOCK = 0x8;

    private const uint WM_POWERBROADCAST = 0x0218;
    private const uint PBT_APMRESUMEAUTOMATIC = 0x0012;
    private const uint PBT_APMRESUMESUSPEND = 0x0007;

    private const uint WM_DWMCOMPOSITIONCHANGED = 0x031E;
    private const uint WM_DISPLAYCHANGE = 0x007E;
    private const uint WM_QUERYENDSESSION = 0x0011;
    private const uint WM_ENDSESSION = 0x0016;
    private const uint WM_CLOSE = 0x0010;
    private const uint WM_DESTROY = 0x0002;
    private const uint WM_HOTKEY = 0x0312;

    private const int SM_REMOTESESSION = 0x1000;
    private const uint MONITOR_DEFAULTTONEAREST = 2;

    private const int MAX_PINNED = 16;

    private static readonly IntPtr[] Pinned = new IntPtr[MAX_PINNED];
    private static readonly int[] OrigCorners = new int[MAX_PINNED];
    private static readonly long[] OrigExStyles = new long[MAX_PINNED];
    private static readonly IntPtr[] Thumbnails = new IntPtr[MAX_PINNED];
    private static readonly bool[] CornerApplied = new bool[MAX_PINNED];
    private static readonly bool[] ThumbnailApplied = new bool[MAX_PINNED];
    private static readonly bool[] LayeredApplied = new bool[MAX_PINNED];
    private static readonly bool[] NoActivateApplied = new bool[MAX_PINNED];
    private static int PinnedCount = 0;

    private static IntPtr _hHostWnd = IntPtr.Zero;
    private static UIntPtr _timerId = UIntPtr.Zero;
    private static IntPtr _hHook = IntPtr.Zero;
    private static IntPtr _lastForeground = IntPtr.Zero;
    private static int _pulseRef = 0;
    private static bool _isCleaningUp = false;

    private static WndProcDelegate _wndProcDelegate;
    private static WinEventDelegate _winEventDelegate;
    private static TimerDelegate _timerDelegate;
    private static ConsoleCtrlDelegate _consoleCtrlDelegate;

    [STAThread]
    private static void Main() {
        bool createdNew;
        using (Mutex singleInstanceMutex = new Mutex(true, @"Local\NativeTopmostDaemon_SingleInstanceMutex", out createdNew)) {
            if (!createdNew) {
                MessageBeep(0x00000010);
                return;
            }

            try {
                SetProcessDPIAware();
            } catch { }

            _consoleCtrlDelegate = OnConsoleCtrl;
            SetConsoleCtrlHandler(_consoleCtrlDelegate, true);

            IntPtr hInstance = GetModuleHandle(null);
            _wndProcDelegate = HostWndProc;
            WNDCLASSEX wc = new WNDCLASSEX();
            wc.cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
            wc.lpfnWndProc = _wndProcDelegate;
            wc.hInstance = hInstance;
            wc.lpszClassName = "NativeTopmostHostClass";

            ushort regResult = RegisterClassEx(ref wc);
            if (regResult == 0 && Marshal.GetLastWin32Error() != 1410) {
                return;
            }

            _hHostWnd = CreateWindowEx(
                0x00080000,
                "NativeTopmostHostClass",
                "NativeTopmostHost",
                0x80000000,
                0, 0, 1, 1,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero
            );

            if (_hHostWnd == IntPtr.Zero) {
                return;
            }

            if (!RegisterHotKey(_hHostWnd, HOTKEY_ID, MOD_CONTROL | MOD_WIN, VK_T)) {
                MessageBeep(0x00000010);
                DestroyWindow(_hHostWnd);
                return;
            }

            try {
                WTSRegisterSessionNotification(_hHostWnd, NOTIFY_FOR_THIS_SESSION);
            } catch { }

            _winEventDelegate = OnForegroundChanged;
            _hHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventDelegate,
                0, 0,
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
            );

            _timerDelegate = OnHeartbeat;

            try {
                SetProcessWorkingSetSize(GetCurrentProcess(), new IntPtr(-1), new IntPtr(-1));
            } catch { }

            try {
                MSG msg;
                while (GetMessageW(out msg, IntPtr.Zero, 0, 0) > 0) {
                    TranslateMessage(ref msg);
                    DispatchMessageW(ref msg);
                }
            } finally {
                Cleanup();
            }
        }
    }

    private static IntPtr HostWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) {
        switch (msg) {
            case WM_HOTKEY:
                if (wParam.ToInt64() == HOTKEY_ID) {
                    ToggleActiveWindow();
                }
                return IntPtr.Zero;

            case WM_DWMCOMPOSITIONCHANGED:
            case WM_DISPLAYCHANGE:
                ReArmAll();
                return IntPtr.Zero;

            case WM_POWERBROADCAST:
                long pbt = wParam.ToInt64();
                if (pbt == PBT_APMRESUMEAUTOMATIC || pbt == PBT_APMRESUMESUSPEND) {
                    ReArmAll();
                }
                return IntPtr.Zero;

            case WM_WTSSESSION_CHANGE:
                if (wParam.ToInt64() == WTS_SESSION_UNLOCK) {
                    ReArmAll();
                }
                return IntPtr.Zero;

            case WM_QUERYENDSESSION:
                return new IntPtr(1);

            case WM_ENDSESSION:
                if (wParam != IntPtr.Zero) {
                    Cleanup();
                }
                return IntPtr.Zero;

            case WM_CLOSE:
            case WM_DESTROY:
                Cleanup();
                PostQuitMessage(0);
                return IntPtr.Zero;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static bool OnConsoleCtrl(uint ctrlType) {
        Cleanup();
        return false;
    }

    private static void Cleanup() {
        if (_isCleaningUp) return;
        _isCleaningUp = true;

        for (int i = 0; i < PinnedCount; i++) {
            if (IsWindow(Pinned[i])) {
                RestoreMpoDefense(i);
                SetWindowPos(Pinned[i], HWND_NOTOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
            }
        }
        PinnedCount = 0;

        if (_timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            KillTimer(_hHostWnd, _timerId);
            _timerId = UIntPtr.Zero;
        }

        if (_hHook != IntPtr.Zero) {
            UnhookWinEvent(_hHook);
            _hHook = IntPtr.Zero;
        }

        if (_hHostWnd != IntPtr.Zero) {
            UnregisterHotKey(_hHostWnd, HOTKEY_ID);
            try { WTSUnRegisterSessionNotification(_hHostWnd); } catch { }
            DestroyWindow(_hHostWnd);
            _hHostWnd = IntPtr.Zero;
        }
    }

    private static IntPtr GetTrueRoot(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return IntPtr.Zero;
        IntPtr root = GetAncestor(hwnd, GA_ROOT);
        return (root != IntPtr.Zero) ? root : hwnd;
    }

    private static IntPtr FindFullscreenWindow(IntPtr hMonitor, IntPtr ignoreWnd) {
        if (hMonitor == IntPtr.Zero) return IntPtr.Zero;
        MONITORINFO mi = new MONITORINFO();
        mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        if (!GetMonitorInfo(hMonitor, ref mi)) return IntPtr.Zero;

        IntPtr hwnd = GetWindow(GetDesktopWindow(), GW_CHILD);
        while (hwnd != IntPtr.Zero) {
            if (hwnd != ignoreWnd && IsWindowVisible(hwnd) && !IsIconic(hwnd)) {
                if (!IsSystemShellWindow(hwnd)) {
                    RECT rc;
                    if (GetWindowRect(hwnd, out rc)) {
                        if (rc.left <= mi.rcMonitor.left && rc.top <= mi.rcMonitor.top &&
                            rc.right >= mi.rcMonitor.right && rc.bottom >= mi.rcMonitor.bottom) {
                            return hwnd;
                        }
                    }
                }
            }
            hwnd = GetWindow(hwnd, GW_HWNDNEXT);
        }
        return IntPtr.Zero;
    }

    private static void SuppressTaskbarIfFullscreen(IntPtr targetWnd) {
        IntPtr hMon = MonitorFromWindow(targetWnd, MONITOR_DEFAULTTONEAREST);
        if (FindFullscreenWindow(hMon, targetWnd) != IntPtr.Zero) {
            IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
            if (hTaskbar != IntPtr.Zero) {
                SetWindowPos(hTaskbar, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
            IntPtr hSecTaskbar = FindWindow("Shell_SecondaryTrayWnd", null);
            if (hSecTaskbar != IntPtr.Zero) {
                SetWindowPos(hSecTaskbar, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }
    }

    private static void ToggleActiveWindow() {
        IntPtr target = IntPtr.Zero;
        POINT pt;
        if (GetCursorPos(out pt)) {
            IntPtr wndUnder = WindowFromPoint(pt);
            if (wndUnder != IntPtr.Zero) {
                IntPtr rootUnder = GetTrueRoot(wndUnder);
                for (int i = 0; i < PinnedCount; i++) {
                    if (Pinned[i] == rootUnder) {
                        target = rootUnder;
                        break;
                    }
                }
            }
        }

        if (target == IntPtr.Zero) {
            IntPtr fg = GetForegroundWindow();
            if (fg != IntPtr.Zero) {
                target = GetTrueRoot(fg);
            }
        }

        if (target == IntPtr.Zero) return;

        if (IsSystemShellWindow(target)) {
            MessageBeep(0x00000010);
            return;
        }

        for (int i = 0; i < PinnedCount; i++) {
            if (Pinned[i] == target) {
                UnpinIndex(i);
                MessageBeep(0x00000000);
                return;
            }
        }

        if (PinnedCount >= MAX_PINNED) {
            MessageBeep(0x00000010);
            return;
        }

        int idx = PinnedCount;

        ApplyMpoDefense(target, idx);

        Pinned[PinnedCount++] = target;

        if (PinnedCount == 1 && _timerId == UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            _timerId = SetTimer(_hHostWnd, new UIntPtr(1), 350, _timerDelegate);
        }

        SetWindowPos(target, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
        SuppressTaskbarIfFullscreen(target);
        MessageBeep(0x00000040);
    }

    private static void UnpinIndex(int i) {
        IntPtr hwnd = Pinned[i];

        RestoreMpoDefense(i);

        long currentEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        if ((currentEx & WS_EX_TOPMOST) != 0) {
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(currentEx & ~WS_EX_TOPMOST));
        }
        SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);

        IntPtr hMon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        IntPtr hFs = FindFullscreenWindow(hMon, hwnd);
        if (hFs != IntPtr.Zero) {
            SetWindowPos(hwnd, hFs, 0, 0, 0, 0, SWP_STEADY_FLAGS);
        }

        int tail = --PinnedCount;
        Pinned[i] = Pinned[tail];
        OrigCorners[i] = OrigCorners[tail];
        OrigExStyles[i] = OrigExStyles[tail];
        Thumbnails[i] = Thumbnails[tail];
        CornerApplied[i] = CornerApplied[tail];
        ThumbnailApplied[i] = ThumbnailApplied[tail];
        LayeredApplied[i] = LayeredApplied[tail];
        NoActivateApplied[i] = NoActivateApplied[tail];

        Pinned[tail] = IntPtr.Zero;
        OrigCorners[tail] = 0;
        OrigExStyles[tail] = 0;
        Thumbnails[tail] = IntPtr.Zero;
        CornerApplied[tail] = false;
        ThumbnailApplied[tail] = false;
        LayeredApplied[tail] = false;
        NoActivateApplied[tail] = false;

        if (PinnedCount == 0 && _timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            KillTimer(_hHostWnd, _timerId);
            _timerId = UIntPtr.Zero;
            try {
                SetProcessWorkingSetSize(GetCurrentProcess(), new IntPtr(-1), new IntPtr(-1));
            } catch { }
        }
    }

    private static void ApplyMpoDefense(IntPtr hwnd, int idx) {
        CornerApplied[idx] = false;
        ThumbnailApplied[idx] = false;
        LayeredApplied[idx] = false;
        NoActivateApplied[idx] = false;
        Thumbnails[idx] = IntPtr.Zero;

        long origEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        OrigExStyles[idx] = origEx;

        int origCorner = DWMWCP_DEFAULT;
        try {
            if (DwmGetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, out origCorner, sizeof(int)) != 0) {
                origCorner = DWMWCP_DEFAULT;
            }
        } catch {
            origCorner = DWMWCP_DEFAULT;
        }
        OrigCorners[idx] = origCorner;

        if (GetSystemMetrics(SM_REMOTESESSION) != 0) {
            PulseDisplayExecutionLock();
            return;
        }

        uint dpi = 96;
        try { dpi = GetDpiForWindow(hwnd); } catch { }

        bool isGameOrProtected = IsGameOrProtectedClass(hwnd);

        if (dpi < 168 && !isGameOrProtected) {
            int pref = DWMWCP_ROUNDSMALL;
            int hr = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));
            if (hr == 0) {
                CornerApplied[idx] = true;
            }
        }

        if (!CornerApplied[idx] && _hHostWnd != IntPtr.Zero) {
            try {
                IntPtr hMon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                MONITORINFO mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                if (GetMonitorInfo(hMon, ref mi)) {
                    SetWindowPos(_hHostWnd, IntPtr.Zero, mi.rcMonitor.left, mi.rcMonitor.top, 1, 1, SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOSENDCHANGING);
                }

                IntPtr thumbId;
                int hrThumb = DwmRegisterThumbnail(_hHostWnd, hwnd, out thumbId);
                if (hrThumb == 0 && thumbId != IntPtr.Zero) {
                    DWM_THUMBNAIL_PROPERTIES props = new DWM_THUMBNAIL_PROPERTIES();
                    props.dwFlags = DWM_TNP_VISIBLE | DWM_TNP_OPACITY;
                    props.fVisible = false;
                    props.opacity = 0;
                    DwmUpdateThumbnailProperties(thumbId, ref props);
                    Thumbnails[idx] = thumbId;
                    ThumbnailApplied[idx] = true;
                }
            } catch { }
        }

        if (!CornerApplied[idx] && !ThumbnailApplied[idx] && !isGameOrProtected) {
            if ((origEx & WS_EX_LAYERED) == 0) {
                IntPtr res = SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(origEx | WS_EX_LAYERED));
                if (res != IntPtr.Zero || Marshal.GetLastWin32Error() == 0) {
                    LayeredApplied[idx] = true;
                    SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                }
            }
        }

        RECT rc = new RECT();
        if (GetWindowRect(hwnd, out rc)) {
            int w = rc.right - rc.left;
            int h = rc.bottom - rc.top;
            if (!IsTextInputClass(hwnd) && (w <= 1280 && h <= 720)) {
                long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                if ((cur & WS_EX_NOACTIVATE) == 0) {
                    SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(cur | WS_EX_NOACTIVATE));
                    SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                    NoActivateApplied[idx] = true;
                }
            }
        }

        PulseDisplayExecutionLock();
    }

    private static void RestoreMpoDefense(int idx) {
        IntPtr hwnd = Pinned[idx];

        if (NoActivateApplied[idx]) {
            try {
                long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(cur & ~WS_EX_NOACTIVATE));
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
            } catch { }
            NoActivateApplied[idx] = false;
        }

        if (ThumbnailApplied[idx] && Thumbnails[idx] != IntPtr.Zero) {
            try {
                DwmUnregisterThumbnail(Thumbnails[idx]);
            } catch { }
            Thumbnails[idx] = IntPtr.Zero;
            ThumbnailApplied[idx] = false;
        }

        if (CornerApplied[idx]) {
            try {
                int corner = OrigCorners[idx];
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref corner, sizeof(int));
            } catch { }
            CornerApplied[idx] = false;
        }

        if (LayeredApplied[idx]) {
            try {
                long currentEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(currentEx & ~WS_EX_LAYERED));
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
            } catch { }
            LayeredApplied[idx] = false;
        }
    }

    private static void ReArmAll() {
        for (int i = 0; i < PinnedCount; i++) {
            IntPtr hwnd = Pinned[i];
            if (!IsWindow(hwnd)) continue;

            if (CornerApplied[i]) {
                int pref = DWMWCP_ROUNDSMALL;
                try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int)); } catch { }
            }

            if (ThumbnailApplied[i] && _hHostWnd != IntPtr.Zero) {
                try {
                    if (Thumbnails[i] != IntPtr.Zero) {
                        DwmUnregisterThumbnail(Thumbnails[i]);
                    }
                    IntPtr thumbId;
                    if (DwmRegisterThumbnail(_hHostWnd, hwnd, out thumbId) == 0) {
                        DWM_THUMBNAIL_PROPERTIES props = new DWM_THUMBNAIL_PROPERTIES();
                        props.dwFlags = DWM_TNP_VISIBLE | DWM_TNP_OPACITY;
                        props.fVisible = false;
                        props.opacity = 0;
                        DwmUpdateThumbnailProperties(thumbId, ref props);
                        Thumbnails[i] = thumbId;
                    }
                } catch { }
            }
        }
        PulseDisplayExecutionLock();
    }

    private static void PulseDisplayExecutionLock() {
        Interlocked.Increment(ref _pulseRef);
        SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
        ThreadPool.QueueUserWorkItem(delegate {
            try {
                Thread.Sleep(2000);
            } catch { }
            finally {
                if (Interlocked.Decrement(ref _pulseRef) == 0) {
                    SetThreadExecutionState(ES_CONTINUOUS);
                }
            }
        });
    }

    private static void Enforce() {
        if (PinnedCount == 0) return;

        for (int i = PinnedCount - 1; i >= 0; i--) {
            if (!IsWindow(Pinned[i])) {
                UnpinIndex(i);
            }
        }

        if (PinnedCount == 0) return;

        IntPtr fg = GetForegroundWindow();
        if (fg == IntPtr.Zero) return;

        IntPtr fgRoot = GetTrueRoot(fg);
        if (fgRoot == IntPtr.Zero) fgRoot = fg;

        bool fgChanged = (fgRoot != _lastForeground);
        _lastForeground = fgRoot;

        for (int i = 0; i < PinnedCount; i++) {
            IntPtr p = Pinned[i];
            if (IsIconic(p) || !IsWindowVisible(p)) continue;

            if (fgChanged || GetWindow(p, GW_HWNDPREV) != IntPtr.Zero) {
                SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
            }
        }

        for (int i = 0; i < PinnedCount; i++) {
            if (NoActivateApplied[i]) {
                SuppressTaskbarIfFullscreen(Pinned[i]);
            }
        }

        try { DwmFlush(); } catch { }
    }

    private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        if (idObject != 0) return;
        Enforce();
    }

    private static void OnHeartbeat(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime) {
        Enforce();
    }

    private static bool IsSystemShellWindow(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return true;
        StringBuilder sb = new StringBuilder(64);
        if (GetClassName(hwnd, sb, sb.Capacity) > 0) {
            string cls = sb.ToString();
            if (cls == "Shell_TrayWnd" || cls == "Shell_SecondaryTrayWnd" ||
                cls == "Progman" || cls == "WorkerW" ||
                cls == "Windows.UI.Core.CoreWindow" ||
                cls == "MultitaskingViewFrame" ||
                cls == "NativeTopmostHostClass") {
                return true;
            }
        }
        return false;
    }

    private static bool IsGameOrProtectedClass(IntPtr hwnd) {
        StringBuilder sb = new StringBuilder(64);
        if (GetClassName(hwnd, sb, sb.Capacity) > 0) {
            string cls = sb.ToString();
            if (cls.Contains("Valve001") || cls.Contains("UnrealWindow") || cls.Contains("UnityWndClass")) {
                return true;
            }
        }
        return false;
    }

    private static bool IsTextInputClass(IntPtr hwnd) {
        StringBuilder sb = new StringBuilder(64);
        if (GetClassName(hwnd, sb, sb.Capacity) > 0) {
            string cls = sb.ToString();
            if (cls == "Notepad" || cls == "Edit" || cls.Contains("Console") || cls.Contains("Terminal")) {
                return true;
            }
        }

        uint pid;
        uint tid = GetWindowThreadProcessId(hwnd, out pid);
        if (tid != 0) {
            GUITHREADINFO gui = new GUITHREADINFO();
            gui.cbSize = Marshal.SizeOf(typeof(GUITHREADINFO));
            if (GetGUIThreadInfo(tid, ref gui)) {
                if (gui.hwndCaret != IntPtr.Zero) {
                    return true;
                }
            }
        }

        return false;
    }
}
