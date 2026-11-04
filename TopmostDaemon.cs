using System;
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

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

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
    private static extern bool IsZoomed(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsHungAppWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex) {
        return (IntPtr.Size == 8)
            ? GetWindowLongPtr64(hWnd, nIndex)
            : new IntPtr(GetWindowLong32(hWnd, nIndex));
    }

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
        return (IntPtr.Size == 8)
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out uint pcrKey, out byte pbAlpha, out uint pdwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessageW(ref MSG lpMsg);

    [DllImport("user32.dll")]
    private static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessageW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerDelegate lpTimerFunc);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern UIntPtr SetCoalescableTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerDelegate lpTimerFunc, uint uToleranceDelay);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);

    [DllImport("user32.dll")]
    private static extern bool MessageBeep(uint uType);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, uint uType);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern ushort RegisterClassExW(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr DefWindowProcW(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr RegisterSuspendResumeNotification(IntPtr hRecipient, uint Flags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterSuspendResumeNotification(IntPtr Handle);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    private static extern bool SetPropW(IntPtr hWnd, string lpString, IntPtr hData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr GetPropW(IntPtr hWnd, string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr RemovePropW(IntPtr hWnd, string lpString);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetPriorityClass(IntPtr hProcess, uint dwPriorityClass);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetProcessInformation(IntPtr hProcess, int ProcessInformationClass, ref PROCESS_POWER_THROTTLING_STATE ProcessInformation, uint ProcessInformationSize);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint SetThreadExecutionState(uint esFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

    [DllImport("kernel32.dll")]
    private static extern ulong GetTickCount64();

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, uint dwFlags);

    [DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_POWER_THROTTLING_STATE {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
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

    private struct PinnedEntry {
        public IntPtr Hwnd;
        public IntPtr LastOccluder;
        public ulong LastReassertTick;
        public uint OrigProcessId;
        public int ConflictCount;
        public int OrigCorner;
        public long OrigExStyle;
        public uint OrigCrKey;
        public byte OrigAlpha;
        public uint OrigLwaFlags;
        public bool IsGame;
        public bool CornerApplied;
        public bool LayeredApplied;
        public bool NoActivateApplied;
        public bool WasIconic;
    }

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_NOSENDCHANGING = 0x0400;
    private const uint SWP_ASYNCWINDOWPOS = 0x4000;

    private const uint SWP_STEADY_FLAGS = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOSENDCHANGING;
    private const uint SWP_ASYNC_STEADY_FLAGS = SWP_STEADY_FLAGS | SWP_ASYNCWINDOWPOS;

    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_WIN = 0x0008;
    private const uint MOD_NOREPEAT = 0x4000;
    private const uint VK_T = 0x54;
    private const int HOTKEY_ID = 9001;

    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
    private const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
    private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
    private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    private const uint WINEVENT_SKIPOWNPROCESS = 0x0002;

    private const uint GA_ROOT = 2;
    private const uint GA_ROOTOWNER = 3;
    private const uint GW_HWNDPREV = 3;
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOPMOST = 0x00000008L;
    private const long WS_EX_APPWINDOW = 0x00040000L;
    private const long WS_EX_LAYERED = 0x00080000L;
    private const long WS_EX_NOACTIVATE = 0x08000000L;
    private const uint LWA_ALPHA = 0x00000002;

    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWA_CLOAKED = 14;
    private const int DWMWCP_DEFAULT = 0;
    private const int DWMWCP_ROUNDSMALL = 3;
    private const int CBINT = 4;

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;

    private const int ProcessPowerThrottling = 4;
    private const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1;
    private const uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 0x1;
    private const uint PROCESS_POWER_THROTTLING_IGNORE_TIMER_RESOLUTION = 0x4;
    private const uint BELOW_NORMAL_PRIORITY_CLASS = 0x00004000;

    private const uint DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
    private const uint SHUTDOWN_NORETRY = 0x00000001;

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

    private const uint MB_ICONERROR = 0x00000010;
    private const uint MB_TOPMOST = 0x00040000;

    private const int SM_REMOTESESSION = 0x1000;
    private const uint MONITOR_DEFAULTTONEAREST = 2;

    private const int MAX_PINNED = 16;
    private const uint HEARTBEAT_MIN_MS = 250;
    private const uint HEARTBEAT_MAX_MS = 2500;

    private const ulong CONFLICT_WINDOW_MS = 1500;
    private const int CONFLICT_YIELD_THRESHOLD = 4;

    private const int SMALL_W = 1280;
    private const int SMALL_H = 720;
    private const int SMALL_HYSTERESIS_PX = 32;
    private const uint DPI_CORNER_MAX = 168;
    private const ulong PULSE_MS = 2000;
    private const int CLASS_BUF = 256;

    private const string PROP_ORIG_EX = "NativeTopmostDaemon_OrigEx";
    private const long PROP_SENTINEL_BIT = 1L << 32;

    private static readonly IntPtr DPI_AWARE_V2 = new IntPtr(-4);

    private static readonly PinnedEntry[] Pinned = new PinnedEntry[MAX_PINNED];
    private static int PinnedCount = 0;

    private static readonly StringBuilder _classSb = new StringBuilder(CLASS_BUF);

    private static IntPtr _hHostWnd = IntPtr.Zero;
    private static IntPtr _hPowerNotify = IntPtr.Zero;
    private static UIntPtr _timerId = UIntPtr.Zero;
    private static uint _currentHeartbeatMs = HEARTBEAT_MIN_MS;
    private static IntPtr _hHook = IntPtr.Zero;
    private static IntPtr _hMoveHook = IntPtr.Zero;
    private static IntPtr _hMinHook = IntPtr.Zero;
    private static bool _isCleaningUp = false;
    private static readonly ManualResetEvent _cleanupGate = new ManualResetEvent(false);

    private static ulong _pulseUntil = 0;
    private static bool _pulseActive = false;

    private static WndProcDelegate _wndProcDelegate;
    private static WinEventDelegate _winEventDelegate;
    private static TimerDelegate _timerDelegate;
    private static ConsoleCtrlDelegate _consoleCtrlDelegate;

    private static readonly int SizeOfMonitorInfo = Marshal.SizeOf(typeof(MONITORINFO));
    private static readonly int SizeOfGuiInfo = Marshal.SizeOf(typeof(GUITHREADINFO));
    private static readonly int SizeOfWndClass = Marshal.SizeOf(typeof(WNDCLASSEX));
    private static readonly uint SizeOfPowerThrottling = (uint)Marshal.SizeOf(typeof(PROCESS_POWER_THROTTLING_STATE));

    [STAThread]
    private static void Main() {
        Mutex singleInstanceMutex = null;
        bool hasHandle = false;
        try {
            singleInstanceMutex = new Mutex(false, @"Local\NativeTopmostDaemon_SingleInstanceMutex");
            try {
                hasHandle = singleInstanceMutex.WaitOne(0, false);
            } catch (AbandonedMutexException) {
                hasHandle = true;
            }
        } catch {
            return;
        }

        using (singleInstanceMutex) {
            if (!hasHandle) {
                MessageBeep(0x00000010);
                return;
            }

            TryEnableDpiAwareness();
            TryEnableEcoQoS();
            SetProcessShutdownParameters(0x3FF, SHUTDOWN_NORETRY);
            RecoverOrphanedWindows();

            _consoleCtrlDelegate = OnConsoleCtrl;
            SetConsoleCtrlHandler(_consoleCtrlDelegate, true);

            IntPtr hInstance = GetModuleHandle(null);
            _wndProcDelegate = HostWndProc;
            WNDCLASSEX wc = new WNDCLASSEX();
            wc.cbSize = (uint)SizeOfWndClass;
            wc.lpfnWndProc = _wndProcDelegate;
            wc.hInstance = hInstance;
            wc.lpszClassName = "NativeTopmostHostClass";

            ushort regResult = RegisterClassExW(ref wc);
            if (regResult == 0 && Marshal.GetLastWin32Error() != 1410) {
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            _hHostWnd = CreateWindowExW(
                WS_EX_TOOLWINDOW | (uint)WS_EX_NOACTIVATE,
                "NativeTopmostHostClass",
                "NativeTopmostHost",
                WS_POPUP,
                0, 0, 0, 0,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero
            );

            if (_hHostWnd == IntPtr.Zero) {
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            if (!RegisterHotKey(_hHostWnd, HOTKEY_ID, MOD_CONTROL | MOD_WIN | MOD_NOREPEAT, VK_T) &&
                !RegisterHotKey(_hHostWnd, HOTKEY_ID, MOD_CONTROL | MOD_WIN, VK_T)) {
                MessageBoxW(
                    IntPtr.Zero,
                    "Ctrl+Win+T hotkey is already registered by another application (e.g., PowerToys AlwaysOnTop).",
                    "NativeTopmostDaemon",
                    MB_ICONERROR | MB_TOPMOST
                );
                DestroyWindow(_hHostWnd);
                _hHostWnd = IntPtr.Zero;
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            try { WTSRegisterSessionNotification(_hHostWnd, NOTIFY_FOR_THIS_SESSION); } catch { }
            try { _hPowerNotify = RegisterSuspendResumeNotification(_hHostWnd, DEVICE_NOTIFY_WINDOW_HANDLE); } catch { _hPowerNotify = IntPtr.Zero; }

            _winEventDelegate = OnWinEvent;
            _hHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventDelegate,
                0, 0,
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
            );
            _hMoveHook = SetWinEventHook(
                EVENT_SYSTEM_MOVESIZEEND, EVENT_SYSTEM_MOVESIZEEND,
                IntPtr.Zero, _winEventDelegate,
                0, 0,
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
            );
            _hMinHook = SetWinEventHook(
                EVENT_SYSTEM_MINIMIZESTART, EVENT_SYSTEM_MINIMIZEEND,
                IntPtr.Zero, _winEventDelegate,
                0, 0,
                WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS
            );

            _timerDelegate = OnHeartbeat;

            int exitCode = 0;
            try {
                MSG msg;
                int r;
                while ((r = GetMessageW(out msg, IntPtr.Zero, 0, 0)) > 0) {
                    TranslateMessage(ref msg);
                    DispatchMessageW(ref msg);
                }
                if (r == 0) {
                    exitCode = unchecked((int)msg.wParam.ToInt64());
                } else {
                    exitCode = 1;
                }
            } finally {
                Cleanup();
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                if (hasHandle) {
                    try { singleInstanceMutex.ReleaseMutex(); } catch { }
                }
                Environment.ExitCode = exitCode;
                GC.KeepAlive(_wndProcDelegate);
                GC.KeepAlive(_winEventDelegate);
                GC.KeepAlive(_timerDelegate);
                GC.KeepAlive(_consoleCtrlDelegate);
            }
        }
    }

    private static void RecoverOrphanedWindows() {
        try {
            EnumWindows(RecoverOrphanCallback, IntPtr.Zero);
        } catch { }
    }

    private static bool RecoverOrphanCallback(IntPtr hwnd, IntPtr lParam) {
        IntPtr prop = GetPropW(hwnd, PROP_ORIG_EX);
        if (prop == IntPtr.Zero) return true;

        RemovePropW(hwnd, PROP_ORIG_EX);
        if (IsHungAppWindow(hwnd)) return true;

        long raw = prop.ToInt64();
        long origEx = raw & ~PROP_SENTINEL_BIT;
        long curEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        long nextEx = curEx;
        if ((curEx & WS_EX_LAYERED) != 0) {
            try { SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA); } catch { }
        }

        if ((origEx & WS_EX_NOACTIVATE) == 0) nextEx &= ~WS_EX_NOACTIVATE;
        if ((origEx & WS_EX_LAYERED) == 0) nextEx &= ~WS_EX_LAYERED;
        if ((origEx & WS_EX_APPWINDOW) == 0) nextEx &= ~WS_EX_APPWINDOW;

        if (nextEx != curEx) {
            try { SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
        }

        int defCorner = DWMWCP_DEFAULT;
        try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref defCorner, CBINT); } catch { }

        IntPtr insertAfter = ((origEx & WS_EX_TOPMOST) != 0) ? HWND_TOPMOST : HWND_NOTOPMOST;
        SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
        return true;
    }

    private static void TryEnableDpiAwareness() {
        try {
            if (!SetProcessDpiAwarenessContext(DPI_AWARE_V2)) {
                SetProcessDPIAware();
            }
        } catch {
            try { SetProcessDPIAware(); } catch { }
        }
    }

    private static void TryEnableEcoQoS() {
        try {
            IntPtr hProc = GetCurrentProcess();
            SetPriorityClass(hProc, BELOW_NORMAL_PRIORITY_CLASS);
            PROCESS_POWER_THROTTLING_STATE state = new PROCESS_POWER_THROTTLING_STATE();
            state.Version = PROCESS_POWER_THROTTLING_CURRENT_VERSION;
            state.ControlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED | PROCESS_POWER_THROTTLING_IGNORE_TIMER_RESOLUTION;
            state.StateMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED | PROCESS_POWER_THROTTLING_IGNORE_TIMER_RESOLUTION;
            SetProcessInformation(hProc, ProcessPowerThrottling, ref state, SizeOfPowerThrottling);
        } catch { }
    }

    private static void ScheduleHeartbeat(uint intervalMs) {
        if (_hHostWnd == IntPtr.Zero || PinnedCount == 0) return;
        _currentHeartbeatMs = intervalMs;
        uint tolerance = intervalMs >> 2;
        try {
            UIntPtr id = SetCoalescableTimer(_hHostWnd, new UIntPtr(1), intervalMs, _timerDelegate, tolerance);
            if (id != UIntPtr.Zero) {
                _timerId = id;
                return;
            }
        } catch { }
        _timerId = SetTimer(_hHostWnd, new UIntPtr(1), intervalMs, _timerDelegate);
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

            case WM_POWERBROADCAST: {
                long pbt = wParam.ToInt64();
                if (pbt == PBT_APMRESUMEAUTOMATIC || pbt == PBT_APMRESUMESUSPEND) {
                    ReArmAll();
                }
                return IntPtr.Zero;
            }

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
                    PostQuitMessage(0);
                }
                return IntPtr.Zero;

            case WM_CLOSE:
            case WM_DESTROY:
                Cleanup();
                PostQuitMessage(0);
                return IntPtr.Zero;
        }

        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    private static bool OnConsoleCtrl(uint ctrlType) {
        IntPtr host = _hHostWnd;
        if (host != IntPtr.Zero) {
            PostMessageW(host, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _cleanupGate.WaitOne(2500);
        }
        return true;
    }

    private static void RestoreIconicCloak(ref PinnedEntry e, IntPtr hwnd) {
        long curEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        long nextEx = curEx;
        if (!e.LayeredApplied && (e.OrigExStyle & WS_EX_LAYERED) == 0) {
            nextEx &= ~WS_EX_LAYERED;
        }
        if ((e.OrigExStyle & WS_EX_APPWINDOW) == 0) {
            nextEx &= ~WS_EX_APPWINDOW;
        }
        if (e.NoActivateApplied) {
            nextEx |= WS_EX_NOACTIVATE;
        }
        try {
            SetLayeredWindowAttributes(hwnd, e.OrigCrKey, e.OrigAlpha, (e.OrigLwaFlags != 0) ? e.OrigLwaFlags : LWA_ALPHA);
        } catch { }
        if (nextEx != curEx) {
            try { SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
        }
        if ((nextEx & WS_EX_LAYERED) != 0) {
            try {
                SetLayeredWindowAttributes(hwnd, e.OrigCrKey, e.OrigAlpha, (e.OrigLwaFlags != 0) ? e.OrigLwaFlags : LWA_ALPHA);
            } catch { }
        }
    }

    private static void Cleanup() {
        if (_isCleaningUp) {
            _cleanupGate.Set();
            return;
        }
        _isCleaningUp = true;

        try {
            for (int i = 0; i < PinnedCount; i++) {
                IntPtr hw = Pinned[i].Hwnd;
                if (hw == IntPtr.Zero) continue;
                if (IsValidPinnedTarget(i)) {
                    RemovePropW(hw, PROP_ORIG_EX);
                    IntPtr insertAfter = ((Pinned[i].OrigExStyle & WS_EX_TOPMOST) != 0) ? HWND_TOPMOST : HWND_NOTOPMOST;
                    if (!IsHungAppWindow(hw)) {
                        if (Pinned[i].WasIconic) {
                            RestoreIconicCloak(ref Pinned[i], hw);
                        }
                        RestoreMpoDefense(i);
                        SetWindowPos(hw, insertAfter, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
                    } else {
                        SetWindowPos(hw, insertAfter, 0, 0, 0, 0, SWP_ASYNC_STEADY_FLAGS);
                    }
                }
                Pinned[i] = new PinnedEntry();
            }
            PinnedCount = 0;

            ResetDisplayPulse();

            if (_timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
                KillTimer(_hHostWnd, _timerId);
                _timerId = UIntPtr.Zero;
            }

            if (_hHook != IntPtr.Zero) {
                UnhookWinEvent(_hHook);
                _hHook = IntPtr.Zero;
            }

            if (_hMoveHook != IntPtr.Zero) {
                UnhookWinEvent(_hMoveHook);
                _hMoveHook = IntPtr.Zero;
            }

            if (_hMinHook != IntPtr.Zero) {
                UnhookWinEvent(_hMinHook);
                _hMinHook = IntPtr.Zero;
            }

            if (_hPowerNotify != IntPtr.Zero) {
                try { UnregisterSuspendResumeNotification(_hPowerNotify); } catch { }
                _hPowerNotify = IntPtr.Zero;
            }

            if (_hHostWnd != IntPtr.Zero) {
                UnregisterHotKey(_hHostWnd, HOTKEY_ID);
                try { WTSUnRegisterSessionNotification(_hHostWnd); } catch { }
                DestroyWindow(_hHostWnd);
                _hHostWnd = IntPtr.Zero;
            }
        } finally {
            _cleanupGate.Set();
        }
    }

    private static IntPtr GetTrueRoot(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return IntPtr.Zero;
        IntPtr root = GetAncestor(hwnd, GA_ROOT);
        return (root != IntPtr.Zero) ? root : hwnd;
    }

    private static IntPtr GetTrueRootOwner(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return IntPtr.Zero;
        IntPtr ro = GetAncestor(hwnd, GA_ROOTOWNER);
        return (ro != IntPtr.Zero) ? ro : GetTrueRoot(hwnd);
    }

    private static bool IsValidPinnedTarget(int idx) {
        IntPtr hw = Pinned[idx].Hwnd;
        if (hw == IntPtr.Zero || !IsWindow(hw)) return false;
        uint pid;
        if (GetWindowThreadProcessId(hw, out pid) == 0) return false;
        return pid == Pinned[idx].OrigProcessId;
    }

    private static void PromoteToTopOfPinnedList(IntPtr candidateHwnd) {
        if (PinnedCount <= 1 || candidateHwnd == IntPtr.Zero) return;
        IntPtr root = GetTrueRoot(candidateHwnd);
        IntPtr rootOwner = GetTrueRootOwner(candidateHwnd);
        for (int i = 0; i < PinnedCount - 1; i++) {
            IntPtr p = Pinned[i].Hwnd;
            if (p == root || p == rootOwner) {
                PinnedEntry target = Pinned[i];
                for (int j = i; j < PinnedCount - 1; j++) {
                    Pinned[j] = Pinned[j + 1];
                }
                Pinned[PinnedCount - 1] = target;
                return;
            }
        }
    }

    private static bool IsHigherRankedPinnedOrOwned(int idx, IntPtr curr, out bool isLowerRankedPin) {
        isLowerRankedPin = false;
        IntPtr rootOwner = GetAncestor(curr, GA_ROOTOWNER);
        for (int k = 0; k < PinnedCount; k++) {
            IntPtr p = Pinned[k].Hwnd;
            if (curr == p) {
                if (k > idx) return true;
                if (k < idx) isLowerRankedPin = true;
                return false;
            }
            if (rootOwner != IntPtr.Zero && rootOwner == p) {
                return true;
            }
        }
        return false;
    }

    private static bool RectIntersects(ref RECT a, ref RECT b) {
        return a.left < b.right && a.right > b.left && a.top < b.bottom && a.bottom > b.top;
    }

    private static bool NeedsTopmostReassert(int idx, out IntPtr occluder) {
        occluder = IntPtr.Zero;
        IntPtr hwnd = Pinned[idx].Hwnd;
        long ex = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        if ((ex & WS_EX_TOPMOST) == 0) return true;

        RECT rcTarget;
        bool hasTargetRect = GetWindowRect(hwnd, out rcTarget);

        IntPtr curr = GetWindow(hwnd, GW_HWNDPREV);
        int depth = 0;
        while (curr != IntPtr.Zero && depth < 64) {
            depth++;
            bool isLowerPin;
            if (!IsHigherRankedPinnedOrOwned(idx, curr, out isLowerPin)) {
                if (isLowerPin) {
                    if (IsWindowVisible(curr) && !IsIconic(curr) && !IsCloaked(curr)) {
                        if (!hasTargetRect) return true;
                        RECT rcPin;
                        if (!GetWindowRect(curr, out rcPin) || RectIntersects(ref rcTarget, ref rcPin)) {
                            return true;
                        }
                    }
                } else if (IsWindowVisible(curr) &&
                           !IsIconic(curr) &&
                           !IsCloaked(curr) &&
                           !IsShellOrTransientWindow(curr)) {
                    if (!hasTargetRect) {
                        occluder = GetTrueRoot(curr);
                        return true;
                    }
                    RECT rcCurr;
                    if (!GetWindowRect(curr, out rcCurr) || RectIntersects(ref rcTarget, ref rcCurr)) {
                        occluder = GetTrueRoot(curr);
                        return true;
                    }
                }
            }
            curr = GetWindow(curr, GW_HWNDPREV);
        }
        return false;
    }

    private static void ToggleActiveWindow() {
        IntPtr toggleOff = IntPtr.Zero;
        IntPtr rootUnder = IntPtr.Zero;
        POINT pt;
        if (GetCursorPos(out pt)) {
            IntPtr wndUnder = WindowFromPoint(pt);
            if (wndUnder != IntPtr.Zero) {
                rootUnder = GetTrueRoot(wndUnder);
                for (int i = 0; i < PinnedCount; i++) {
                    if (Pinned[i].Hwnd == rootUnder) {
                        toggleOff = rootUnder;
                        break;
                    }
                }
            }
        }

        IntPtr target = toggleOff;
        if (target == IntPtr.Zero && rootUnder != IntPtr.Zero) {
            if (!IsShellOrTransientWindow(rootUnder) && !IsCloaked(rootUnder) &&
                IsWindowVisible(rootUnder) && !IsIconic(rootUnder)) {
                long ex = GetWindowLongPtr(rootUnder, GWL_EXSTYLE).ToInt64();
                if ((ex & WS_EX_NOACTIVATE) != 0) {
                    target = rootUnder;
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

        if (IsShellOrTransientWindow(target) || IsCloaked(target) || IsHungAppWindow(target)) {
            MessageBeep(0x00000010);
            return;
        }

        for (int i = 0; i < PinnedCount; i++) {
            if (Pinned[i].Hwnd == target) {
                UnpinIndex(i);
                MessageBeep(0x00000000);
                return;
            }
        }

        if (PinnedCount >= MAX_PINNED) {
            MessageBeep(0x00000010);
            return;
        }

        uint pid;
        if (GetWindowThreadProcessId(target, out pid) == 0 || pid == 0) {
            MessageBeep(0x00000010);
            return;
        }

        long origEx = GetWindowLongPtr(target, GWL_EXSTYLE).ToInt64();

        int idx = PinnedCount;
        Pinned[idx] = new PinnedEntry();
        Pinned[idx].Hwnd = target;
        Pinned[idx].OrigProcessId = pid;
        Pinned[idx].OrigExStyle = origEx;
        Pinned[idx].WasIconic = false;

        ApplyMpoDefense(target, idx);

        PinnedCount++;
        ScheduleHeartbeat(HEARTBEAT_MIN_MS);

        if (!SetWindowPos(target, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS)) {
            RestoreMpoDefense(idx);
            Pinned[idx] = new PinnedEntry();
            PinnedCount--;
            if (PinnedCount == 0 && _timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
                KillTimer(_hHostWnd, _timerId);
                _timerId = UIntPtr.Zero;
                _currentHeartbeatMs = HEARTBEAT_MIN_MS;
                ResetDisplayPulse();
            }
            MessageBeep(0x00000010);
            return;
        }

        SetPropW(target, PROP_ORIG_EX, new IntPtr((origEx & 0xFFFFFFFFL) | PROP_SENTINEL_BIT));
        MessageBeep(0x00000040);
    }

    private static void UnpinIndex(int i) {
        if (i < 0 || i >= PinnedCount) return;
        PinnedEntry e = Pinned[i];
        IntPtr hwnd = e.Hwnd;
        long origEx = e.OrigExStyle;
        bool alive = IsValidPinnedTarget(i);

        if (alive) {
            RemovePropW(hwnd, PROP_ORIG_EX);
            IntPtr insertAfter = ((origEx & WS_EX_TOPMOST) != 0) ? HWND_TOPMOST : HWND_NOTOPMOST;
            if (!IsHungAppWindow(hwnd)) {
                if (e.WasIconic) {
                    RestoreIconicCloak(ref e, hwnd);
                    Pinned[i] = e;
                }
                RestoreMpoDefense(i);
                SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
            } else {
                SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, SWP_ASYNC_STEADY_FLAGS);
            }
        }

        int tail = --PinnedCount;
        for (int j = i; j < tail; j++) {
            Pinned[j] = Pinned[j + 1];
        }
        Pinned[tail] = new PinnedEntry();

        if (PinnedCount == 0 && _timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            KillTimer(_hHostWnd, _timerId);
            _timerId = UIntPtr.Zero;
            _currentHeartbeatMs = HEARTBEAT_MIN_MS;
            ResetDisplayPulse();
        }
    }

    private static uint QueryWindowDpi(IntPtr hwnd) {
        try {
            uint dpi = GetDpiForWindow(hwnd);
            if (dpi != 0) return dpi;
        } catch { }
        return 96;
    }

    private static void ApplyMpoDefense(IntPtr hwnd, int idx) {
        PinnedEntry e = Pinned[idx];
        e.CornerApplied = false;
        e.LayeredApplied = false;
        e.NoActivateApplied = false;
        e.WasIconic = false;
        e.LastOccluder = IntPtr.Zero;
        e.LastReassertTick = 0;
        e.ConflictCount = 0;
        e.OrigCorner = DWMWCP_DEFAULT;
        e.OrigCrKey = 0;
        e.OrigAlpha = 255;
        e.OrigLwaFlags = 0;

        long origEx = e.OrigExStyle;

        if ((origEx & WS_EX_LAYERED) != 0) {
            try {
                uint key;
                byte alpha;
                uint flags;
                if (GetLayeredWindowAttributes(hwnd, out key, out alpha, out flags)) {
                    e.OrigCrKey = key;
                    e.OrigAlpha = alpha;
                    e.OrigLwaFlags = flags;
                }
            } catch { }
        }

        int origCorner = DWMWCP_DEFAULT;
        try {
            int tmp;
            if (DwmGetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, out tmp, CBINT) != 0) {
                tmp = DWMWCP_DEFAULT;
            }
            origCorner = tmp;
        } catch {
            origCorner = DWMWCP_DEFAULT;
        }
        e.OrigCorner = origCorner;

        e.IsGame = IsGameOrProtectedClass(hwnd);
        Pinned[idx] = e;

        if (GetSystemMetrics(SM_REMOTESESSION) == 0) {
            SyncDynamicMpoState(idx);
        }

        PulseDisplayExecutionLock();
    }

    private static bool SyncDynamicMpoState(int idx) {
        if (GetSystemMetrics(SM_REMOTESESSION) != 0) return false;

        PinnedEntry e = Pinned[idx];
        IntPtr hwnd = e.Hwnd;
        if (hwnd == IntPtr.Zero || e.WasIconic) return false;

        bool changed = false;
        uint dpi = QueryWindowDpi(hwnd);
        bool skipMpo = e.IsGame || IsZoomed(hwnd) || IsFullscreenWindow(hwnd);

        if (skipMpo) {
            if (e.CornerApplied) {
                int orig = e.OrigCorner;
                try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref orig, CBINT); } catch { }
                e.CornerApplied = false;
                changed = true;
            }
            long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
            long next = cur;
            bool nextLayered = e.LayeredApplied;
            bool nextNoAct = e.NoActivateApplied;
            if (e.LayeredApplied && (e.OrigExStyle & WS_EX_LAYERED) == 0) {
                next &= ~WS_EX_LAYERED;
                nextLayered = false;
            }
            if (e.NoActivateApplied && (e.OrigExStyle & WS_EX_NOACTIVATE) == 0) {
                next &= ~WS_EX_NOACTIVATE;
                nextNoAct = false;
            }
            if (next != cur) {
                IntPtr res = SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(next));
                if (res != IntPtr.Zero || Marshal.GetLastWin32Error() == 0) {
                    e.LayeredApplied = nextLayered;
                    e.NoActivateApplied = nextNoAct;
                    SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                    changed = true;
                }
            }
            Pinned[idx] = e;
            return changed;
        }

        bool wantCorner = dpi < DPI_CORNER_MAX;
        if (wantCorner && !e.CornerApplied) {
            int pref = DWMWCP_ROUNDSMALL;
            try {
                if (DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT) == 0) {
                    e.CornerApplied = true;
                    changed = true;
                }
            } catch { e.CornerApplied = false; }
        } else if (!wantCorner && e.CornerApplied) {
            int orig = e.OrigCorner;
            try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref orig, CBINT); } catch { }
            e.CornerApplied = false;
            changed = true;
        }

        long curEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
        long nextEx = curEx;
        bool stagedLayered = e.LayeredApplied;
        bool stagedNoAct = e.NoActivateApplied;

        if (!e.CornerApplied) {
            if (!e.LayeredApplied && (e.OrigExStyle & WS_EX_LAYERED) == 0) {
                nextEx |= WS_EX_LAYERED;
                stagedLayered = true;
            }
        } else if (e.LayeredApplied && (e.OrigExStyle & WS_EX_LAYERED) == 0) {
            nextEx &= ~WS_EX_LAYERED;
            stagedLayered = false;
        }

        RECT rc;
        if (GetWindowRect(hwnd, out rc)) {
            int w = rc.right - rc.left;
            int h = rc.bottom - rc.top;
            int lowW = (int)(((long)SMALL_W * (long)dpi) / 96L);
            int lowH = (int)(((long)SMALL_H * (long)dpi) / 96L);
            int highW = (int)(((long)(SMALL_W + SMALL_HYSTERESIS_PX) * (long)dpi) / 96L);
            int highH = (int)(((long)(SMALL_H + SMALL_HYSTERESIS_PX) * (long)dpi) / 96L);

            if (!e.NoActivateApplied && w <= lowW && h <= lowH && (e.OrigExStyle & WS_EX_NOACTIVATE) == 0) {
                if (!IsTextInputClass(hwnd)) {
                    nextEx |= WS_EX_NOACTIVATE;
                    stagedNoAct = true;
                }
            } else if (e.NoActivateApplied && (w > highW || h > highH) && (e.OrigExStyle & WS_EX_NOACTIVATE) == 0) {
                nextEx &= ~WS_EX_NOACTIVATE;
                stagedNoAct = false;
            }
        }

        if (nextEx != curEx) {
            bool addingLayered = stagedLayered && !e.LayeredApplied;
            IntPtr res = SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(nextEx));
            if (res != IntPtr.Zero || Marshal.GetLastWin32Error() == 0) {
                if (addingLayered) {
                    SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
                }
                e.LayeredApplied = stagedLayered;
                e.NoActivateApplied = stagedNoAct;
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                changed = true;
            }
        }

        Pinned[idx] = e;
        return changed;
    }

    private static void RestoreMpoDefense(int idx) {
        if (idx < 0 || idx >= MAX_PINNED) return;
        PinnedEntry e = Pinned[idx];
        IntPtr hwnd = e.Hwnd;
        if (!IsValidPinnedTarget(idx)) {
            e.CornerApplied = false;
            e.LayeredApplied = false;
            e.NoActivateApplied = false;
            Pinned[idx] = e;
            return;
        }

        if (e.NoActivateApplied) {
            try {
                long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(cur & ~WS_EX_NOACTIVATE));
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
            } catch { }
            e.NoActivateApplied = false;
        }

        if (e.CornerApplied) {
            try {
                int corner = e.OrigCorner;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref corner, CBINT);
            } catch { }
            e.CornerApplied = false;
        }

        if (e.LayeredApplied) {
            try {
                long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(cur & ~WS_EX_LAYERED));
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
            } catch { }
            e.LayeredApplied = false;
        }

        Pinned[idx] = e;
    }

    private static void ReArmAll() {
        if (PinnedCount == 0) return;
        for (int i = 0; i < PinnedCount; i++) {
            if (!IsValidPinnedTarget(i)) continue;
            IntPtr hwnd = Pinned[i].Hwnd;
            if (IsHungAppWindow(hwnd)) continue;

            if (Pinned[i].CornerApplied && !Pinned[i].WasIconic) {
                int pref = DWMWCP_ROUNDSMALL;
                try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT); } catch { }
            }
            if (!Pinned[i].WasIconic && IsWindowVisible(hwnd) && !IsCloaked(hwnd)) {
                SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
            }
        }
        ScheduleHeartbeat(HEARTBEAT_MIN_MS);
        Enforce();
        PulseDisplayExecutionLock();
    }

    private static void PulseDisplayExecutionLock() {
        _pulseUntil = GetTickCount64() + PULSE_MS;
        _pulseActive = true;
        SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
    }

    private static void MaintainDisplayPulse() {
        if (!_pulseActive) return;
        if (GetTickCount64() >= _pulseUntil) {
            SetThreadExecutionState(ES_CONTINUOUS);
            _pulseActive = false;
            _pulseUntil = 0;
        }
    }

    private static void ResetDisplayPulse() {
        if (!_pulseActive) { _pulseUntil = 0; return; }
        SetThreadExecutionState(ES_CONTINUOUS);
        _pulseActive = false;
        _pulseUntil = 0;
    }

    private static bool Enforce() {
        if (PinnedCount == 0) return false;

        bool mutated = false;

        for (int i = PinnedCount - 1; i >= 0; i--) {
            if (!IsValidPinnedTarget(i)) {
                UnpinIndex(i);
                mutated = true;
            }
        }

        if (PinnedCount == 0) return mutated;

        ulong now = GetTickCount64();
        IntPtr fgRoot = GetTrueRoot(GetForegroundWindow());

        for (int i = 0; i < PinnedCount; i++) {
            IntPtr p = Pinned[i].Hwnd;
            if (p == IntPtr.Zero || !IsWindow(p) || IsHungAppWindow(p)) continue;

            if (IsIconic(p)) {
                if (!Pinned[i].WasIconic) {
                    PinnedEntry e = Pinned[i];
                    e.WasIconic = true;
                    Pinned[i] = e;
                    if (e.CornerApplied) {
                        int orig = e.OrigCorner;
                        try { DwmSetWindowAttribute(p, DWMWA_WINDOW_CORNER_PREFERENCE, ref orig, CBINT); } catch { }
                    }
                    long curEx = GetWindowLongPtr(p, GWL_EXSTYLE).ToInt64();
                    long nextEx = (curEx | WS_EX_LAYERED | WS_EX_APPWINDOW) & ~WS_EX_NOACTIVATE;
                    if (nextEx != curEx) {
                        try { SetWindowLongPtr(p, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
                    }
                    try { SetLayeredWindowAttributes(p, 0, 0, LWA_ALPHA); } catch { }
                    SetWindowPos(p, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
                    mutated = true;
                }
                continue;
            }

            if (Pinned[i].WasIconic) {
                PinnedEntry e = Pinned[i];
                e.WasIconic = false;
                RestoreIconicCloak(ref e, p);
                Pinned[i] = e;
                SyncDynamicMpoState(i);
                if (Pinned[i].CornerApplied) {
                    int pref = DWMWCP_ROUNDSMALL;
                    try { DwmSetWindowAttribute(p, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT); } catch { }
                }
                SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
                mutated = true;
                continue;
            }

            if (!IsWindowVisible(p) || IsCloaked(p)) continue;

            if (SyncDynamicMpoState(i)) {
                mutated = true;
            }

            IntPtr occluder;
            if (NeedsTopmostReassert(i, out occluder)) {
                bool repeatedConflict = false;
                if (occluder != IntPtr.Zero) {
                    PinnedEntry e = Pinned[i];
                    if (occluder == e.LastOccluder && (now - e.LastReassertTick) < CONFLICT_WINDOW_MS) {
                        e.ConflictCount++;
                        repeatedConflict = true;
                    } else {
                        e.LastOccluder = occluder;
                        e.ConflictCount = 1;
                    }
                    e.LastReassertTick = now;
                    Pinned[i] = e;

                    if (e.ConflictCount >= CONFLICT_YIELD_THRESHOLD && occluder == fgRoot) {
                        continue;
                    }
                }

                SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
                if (!repeatedConflict) {
                    mutated = true;
                }
            } else if (Pinned[i].ConflictCount != 0 && (now - Pinned[i].LastReassertTick) >= CONFLICT_WINDOW_MS) {
                PinnedEntry e = Pinned[i];
                e.ConflictCount = 0;
                e.LastOccluder = IntPtr.Zero;
                Pinned[i] = e;
            }
        }

        MaintainDisplayPulse();
        return mutated;
    }

    private static void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        try {
            if (idObject != 0 || PinnedCount == 0) return;

            if (eventType == EVENT_SYSTEM_FOREGROUND) {
                if (hwnd != IntPtr.Zero) {
                    PromoteToTopOfPinnedList(hwnd);
                }
                if (_currentHeartbeatMs != HEARTBEAT_MIN_MS) {
                    ScheduleHeartbeat(HEARTBEAT_MIN_MS);
                }
                Enforce();
                return;
            }

            if (eventType == EVENT_SYSTEM_MOVESIZEEND) {
                if (_currentHeartbeatMs != HEARTBEAT_MIN_MS) {
                    ScheduleHeartbeat(HEARTBEAT_MIN_MS);
                }
                Enforce();
                return;
            }

            if (hwnd != IntPtr.Zero) {
                IntPtr root = GetTrueRoot(hwnd);
                for (int i = 0; i < PinnedCount; i++) {
                    if (Pinned[i].Hwnd == root || Pinned[i].Hwnd == hwnd) {
                        if (_currentHeartbeatMs != HEARTBEAT_MIN_MS) {
                            ScheduleHeartbeat(HEARTBEAT_MIN_MS);
                        }
                        Enforce();
                        return;
                    }
                }
            }
        } catch {
            Cleanup();
            PostQuitMessage(1);
        }
    }

    private static void OnHeartbeat(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime) {
        try {
            if (PinnedCount == 0) return;
            bool mutated = Enforce();
            if (mutated) {
                if (_currentHeartbeatMs != HEARTBEAT_MIN_MS) {
                    ScheduleHeartbeat(HEARTBEAT_MIN_MS);
                }
            } else if (!_pulseActive && _currentHeartbeatMs < HEARTBEAT_MAX_MS) {
                uint next = _currentHeartbeatMs + (_currentHeartbeatMs >> 1);
                if (next > HEARTBEAT_MAX_MS) next = HEARTBEAT_MAX_MS;
                ScheduleHeartbeat(next);
            }
        } catch {
            Cleanup();
            PostQuitMessage(1);
        }
    }

    private static string GetWindowClass(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return string.Empty;
        _classSb.Length = 0;
        int len = GetClassNameW(hwnd, _classSb, CLASS_BUF);
        return (len > 0) ? _classSb.ToString(0, len) : string.Empty;
    }

    private static bool IsShellOrTransientWindow(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return true;
        string cls = GetWindowClass(hwnd);
        if (cls.Length == 0) return false;
        return cls == "tooltips_class32" ||
               cls == "#32768" ||
               cls == "MSCTFIME UI" ||
               cls == "IME" ||
               cls == "Ghost" ||
               cls == "SysShadow" ||
               cls == " SysShadow" ||
               cls == "Shell_TrayWnd" ||
               cls == "Shell_SecondaryTrayWnd" ||
               cls == "Progman" ||
               cls == "WorkerW" ||
               cls == "Windows.UI.Core.CoreWindow" ||
               cls == "MultitaskingViewFrame" ||
               cls == "XamlExplorerHostIslandWindow" ||
               cls == "TopLevelWindowForOverflowXamlIsland" ||
               cls == "ControlCenterWindow" ||
               cls == "NativeTopmostHostClass";
    }

    private static bool IsGameOrProtectedClass(IntPtr hwnd) {
        string cls = GetWindowClass(hwnd);
        if (cls.Length == 0) return false;
        return cls.Contains("Valve001") ||
               cls.Contains("UnrealWindow") ||
               cls.Contains("UnityWndClass") ||
               cls.Contains("GLFW") ||
               cls.Contains("SDL_app");
    }

    private static bool IsTextInputClass(IntPtr hwnd) {
        string cls = GetWindowClass(hwnd);
        if (cls.Length > 0) {
            if (cls == "Notepad" ||
                cls == "Edit" ||
                cls == "CASCADIA_HOSTING_WINDOW_CLASS" ||
                cls == "ConsoleWindowClass" ||
                cls == "ApplicationFrameWindow" ||
                cls == "WinUIDesktopWin32WindowClass" ||
                cls.Contains("Console") ||
                cls.Contains("Terminal")) {
                return true;
            }
        }

        uint pid;
        uint tid = GetWindowThreadProcessId(hwnd, out pid);
        if (tid != 0) {
            GUITHREADINFO gui = new GUITHREADINFO();
            gui.cbSize = SizeOfGuiInfo;
            if (GetGUIThreadInfo(tid, ref gui) && gui.hwndCaret != IntPtr.Zero) {
                return true;
            }
        }

        return false;
    }

    private static bool IsCloaked(IntPtr hwnd) {
        try {
            int cloaked;
            if (DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, out cloaked, CBINT) == 0) {
                return cloaked != 0;
            }
        } catch { }
        return false;
    }

    private static bool IsFullscreenWindow(IntPtr hwnd) {
        IntPtr hMon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (hMon == IntPtr.Zero) return false;
        MONITORINFO mi = new MONITORINFO();
        mi.cbSize = SizeOfMonitorInfo;
        if (!GetMonitorInfo(hMon, ref mi)) return false;
        RECT rc;
        if (!GetWindowRect(hwnd, out rc)) return false;
        return rc.left <= mi.rcMonitor.left &&
               rc.top <= mi.rcMonitor.top &&
               rc.right >= mi.rcMonitor.right &&
               rc.bottom >= mi.rcMonitor.bottom;
    }
}

