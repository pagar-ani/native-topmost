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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetProcessDpiAwarenessContext(IntPtr value);

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

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint SetThreadExecutionState(uint esFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

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
        public int OrigCorner;
        public long OrigExStyle;
        public byte OrigAlpha;
        public uint OrigLwaFlags;
        public bool CornerApplied;
        public bool LayeredApplied;
        public bool NoActivateApplied;
        public bool WasIconic;
    }

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

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
    private const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
    private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;
    private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    private const uint WINEVENT_SKIPOWNPROCESS = 0x0002;

    private const uint GA_ROOT = 2;
    private const uint GW_HWNDPREV = 3;
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_LAYERED = 0x00080000L;
    private const long WS_EX_NOACTIVATE = 0x08000000L;
    private const long WS_EX_APPWINDOW = 0x00040000L;
    private const uint LWA_ALPHA = 0x00000002;

    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWA_CLOAKED = 14;
    private const int DWMWCP_DEFAULT = 0;
    private const int DWMWCP_ROUNDSMALL = 3;
    private const int CBINT = 4;

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
    private const uint HEARTBEAT_MS = 350;
    private const int SMALL_W = 1280;
    private const int SMALL_H = 720;
    private const uint DPI_CORNER_MAX = 168;
    private const ulong PULSE_MS = 2000;
    private const int CLASS_BUF = 256;
    private const uint ERROR_ACCESS_DENIED = 5;

    private static readonly IntPtr DPI_AWARE_V2 = new IntPtr(-4);

    private static readonly PinnedEntry[] Pinned = new PinnedEntry[MAX_PINNED];
    private static int PinnedCount = 0;

    private static IntPtr _hHostWnd = IntPtr.Zero;
    private static UIntPtr _timerId = UIntPtr.Zero;
    private static IntPtr _hHook = IntPtr.Zero;
    private static IntPtr _hMinHook = IntPtr.Zero;
    private static bool _isCleaningUp = false;

    private static ulong _pulseUntil = 0;
    private static bool _pulseActive = false;

    private static WndProcDelegate _wndProcDelegate;
    private static WinEventDelegate _winEventDelegate;
    private static TimerDelegate _timerDelegate;
    private static ConsoleCtrlDelegate _consoleCtrlDelegate;

    private static readonly int SizeOfMonitorInfo = Marshal.SizeOf(typeof(MONITORINFO));
    private static readonly int SizeOfGuiInfo = Marshal.SizeOf(typeof(GUITHREADINFO));
    private static readonly int SizeOfWndClass = Marshal.SizeOf(typeof(WNDCLASSEX));

    [STAThread]
    private static void Main() {
        bool createdNew = false;
        Mutex singleInstanceMutex = null;
        try {
            singleInstanceMutex = new Mutex(true, @"Local\NativeTopmostDaemon_SingleInstanceMutex", out createdNew);
        } catch (AbandonedMutexException ex) {
            createdNew = true;
            try { singleInstanceMutex = ex.Mutex; } catch { singleInstanceMutex = null; }
            if (singleInstanceMutex == null) {
                try { singleInstanceMutex = new Mutex(true, @"Local\NativeTopmostDaemon_SingleInstanceMutex", out createdNew); }
                catch { return; }
            }
        } catch {
            return;
        }

        using (singleInstanceMutex) {
            if (!createdNew) {
                MessageBeep(0x00000010);
                return;
            }

            TryEnableDpiAwareness();

            _consoleCtrlDelegate = OnConsoleCtrl;
            SetConsoleCtrlHandler(_consoleCtrlDelegate, true);

            IntPtr hInstance = GetModuleHandle(null);
            _wndProcDelegate = HostWndProc;
            WNDCLASSEX wc = new WNDCLASSEX();
            wc.cbSize = (uint)SizeOfWndClass;
            wc.lpfnWndProc = _wndProcDelegate;
            wc.hInstance = hInstance;
            wc.lpszClassName = "NativeTopmostHostClass";

            ushort regResult = RegisterClassEx(ref wc);
            if (regResult == 0 && Marshal.GetLastWin32Error() != 1410) {
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            _hHostWnd = CreateWindowEx(
                0,
                "NativeTopmostHostClass",
                "NativeTopmostHost",
                0,
                0, 0, 0, 0,
                HWND_MESSAGE, IntPtr.Zero, hInstance, IntPtr.Zero
            );

            if (_hHostWnd == IntPtr.Zero) {
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            if (!RegisterHotKey(_hHostWnd, HOTKEY_ID, MOD_CONTROL | MOD_WIN, VK_T)) {
                MessageBeep(0x00000010);
                DestroyWindow(_hHostWnd);
                _hHostWnd = IntPtr.Zero;
                SetConsoleCtrlHandler(_consoleCtrlDelegate, false);
                return;
            }

            try { WTSRegisterSessionNotification(_hHostWnd, NOTIFY_FOR_THIS_SESSION); } catch { }

            _winEventDelegate = OnForegroundChanged;
            _hHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
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
                    exitCode = msg.wParam.ToInt32();
                } else {
                    exitCode = 1;
                }
            } finally {
                Cleanup();
                Environment.ExitCode = exitCode;
            }
        }
    }

    private static void TryEnableDpiAwareness() {
        try {
            IntPtr prev = SetProcessDpiAwarenessContext(DPI_AWARE_V2);
            if (prev == IntPtr.Zero && Marshal.GetLastWin32Error() != 0) {
                try { SetProcessDPIAware(); } catch { }
            }
        } catch {
            try { SetProcessDPIAware(); } catch { }
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

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static bool OnConsoleCtrl(uint ctrlType) {
        try {
            if (_hHostWnd != IntPtr.Zero) {
                PostMessageW(_hHostWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        } catch { }
        return true;
    }

    private static void Cleanup() {
        if (_isCleaningUp) return;
        _isCleaningUp = true;

        for (int i = 0; i < PinnedCount; i++) {
            IntPtr hw = Pinned[i].Hwnd;
            if (hw == IntPtr.Zero) continue;
            if (IsWindow(hw)) {
                if (Pinned[i].WasIconic) {
                    long curEx = GetWindowLongPtr(hw, GWL_EXSTYLE).ToInt64();
                    long nextEx = curEx;
                    if (!Pinned[i].LayeredApplied && (Pinned[i].OrigExStyle & WS_EX_LAYERED) == 0) {
                        nextEx &= ~WS_EX_LAYERED;
                    }
                    if ((Pinned[i].OrigExStyle & WS_EX_APPWINDOW) == 0) {
                        nextEx &= ~WS_EX_APPWINDOW;
                    }
                    if (Pinned[i].NoActivateApplied) {
                        nextEx |= WS_EX_NOACTIVATE;
                    }
                    if (nextEx != curEx) {
                        try { SetWindowLongPtr(hw, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
                    }
                    if ((nextEx & WS_EX_LAYERED) != 0) {
                        try {
                            SetLayeredWindowAttributes(hw, 0, Pinned[i].OrigAlpha, (Pinned[i].OrigLwaFlags != 0) ? Pinned[i].OrigLwaFlags : LWA_ALPHA);
                        } catch { }
                    }
                }
                RestoreMpoDefense(i);
                SetWindowPos(hw, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
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

        if (_hMinHook != IntPtr.Zero) {
            UnhookWinEvent(_hMinHook);
            _hMinHook = IntPtr.Zero;
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
            if (!IsSystemShellWindow(rootUnder) && !IsCloaked(rootUnder) &&
                IsWindowVisible(rootUnder) && !IsIconic(rootUnder) &&
                !IsTransientPopup(rootUnder)) {
                long ex = 0;
                try { ex = GetWindowLongPtr(rootUnder, GWL_EXSTYLE).ToInt64(); } catch { ex = 0; }
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

        if (IsSystemShellWindow(target)) {
            MessageBeep(0x00000010);
            return;
        }

        if (IsCloaked(target)) {
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

        int idx = PinnedCount;
        Pinned[idx] = new PinnedEntry();
        Pinned[idx].Hwnd = target;
        Pinned[idx].WasIconic = false;

        ApplyMpoDefense(target, idx);

        PinnedCount++;

        if (PinnedCount == 1 && _timerId == UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            _timerId = SetTimer(_hHostWnd, new UIntPtr(1), HEARTBEAT_MS, _timerDelegate);
        }

        if (!SetWindowPos(target, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS)) {
            int err = Marshal.GetLastWin32Error();
            RestoreMpoDefense(idx);
            Pinned[idx] = new PinnedEntry();
            PinnedCount--;
            if (PinnedCount == 0 && _timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
                KillTimer(_hHostWnd, _timerId);
                _timerId = UIntPtr.Zero;
                ResetDisplayPulse();
            }
            MessageBeep(0x00000010);
            return;
        }

        MessageBeep(0x00000040);
    }

    private static void UnpinIndex(int i) {
        if (i < 0 || i >= PinnedCount) return;
        PinnedEntry e = Pinned[i];
        IntPtr hwnd = e.Hwnd;
        bool alive = (hwnd != IntPtr.Zero) && IsWindow(hwnd);

        if (alive) {
            if (e.WasIconic) {
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
                if (nextEx != curEx) {
                    try { SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
                }
                if ((nextEx & WS_EX_LAYERED) != 0) {
                    try {
                        SetLayeredWindowAttributes(hwnd, 0, e.OrigAlpha, (e.OrigLwaFlags != 0) ? e.OrigLwaFlags : LWA_ALPHA);
                    } catch { }
                }
            }
            RestoreMpoDefense(i);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
        }

        int tail = --PinnedCount;
        if (i != tail) {
            Pinned[i] = Pinned[tail];
        }
        Pinned[tail] = new PinnedEntry();

        if (PinnedCount == 0 && _timerId != UIntPtr.Zero && _hHostWnd != IntPtr.Zero) {
            KillTimer(_hHostWnd, _timerId);
            _timerId = UIntPtr.Zero;
            ResetDisplayPulse();
        }
    }

    private static void ApplyMpoDefense(IntPtr hwnd, int idx) {
        PinnedEntry e = Pinned[idx];
        e.CornerApplied = false;
        e.LayeredApplied = false;
        e.NoActivateApplied = false;
        e.WasIconic = false;
        e.OrigCorner = DWMWCP_DEFAULT;
        e.OrigExStyle = 0;
        e.OrigAlpha = 255;
        e.OrigLwaFlags = 0;

        long origEx = 0;
        try { origEx = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64(); } catch { origEx = 0; }
        e.OrigExStyle = origEx;

        if ((origEx & WS_EX_LAYERED) != 0) {
            try {
                uint key;
                byte alpha;
                uint flags;
                if (GetLayeredWindowAttributes(hwnd, out key, out alpha, out flags)) {
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

        if (GetSystemMetrics(SM_REMOTESESSION) != 0) {
            Pinned[idx] = e;
            PulseDisplayExecutionLock();
            return;
        }

        uint dpi = 96;
        try {
            dpi = GetDpiForWindow(hwnd);
            if (dpi == 0) dpi = 96;
        } catch { dpi = 96; }

        bool isGame = false;
        try { isGame = IsGameOrProtectedClass(hwnd); } catch { isGame = false; }

        bool isFs = false;
        bool isZoom = false;
        try { isZoom = IsZoomed(hwnd); } catch { isZoom = false; }
        try { isFs = IsFullscreenWindow(hwnd); } catch { isFs = false; }

        bool skipMpo = isGame || isFs || isZoom;

        if (!skipMpo && dpi < DPI_CORNER_MAX) {
            int pref = DWMWCP_ROUNDSMALL;
            try {
                if (DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT) == 0) {
                    e.CornerApplied = true;
                }
            } catch { e.CornerApplied = false; }
        }

        if (!e.CornerApplied && !skipMpo) {
            if ((origEx & WS_EX_LAYERED) == 0) {
                try {
                    IntPtr res = SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(origEx | WS_EX_LAYERED));
                    if (res != IntPtr.Zero || Marshal.GetLastWin32Error() == 0) {
                        e.LayeredApplied = true;
                        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                    }
                } catch { e.LayeredApplied = false; }
            }
        }

        try {
            RECT rc;
            if (!skipMpo && GetWindowRect(hwnd, out rc)) {
                int w = rc.right - rc.left;
                int h = rc.bottom - rc.top;
                int maxW = (int)(((long)SMALL_W * (long)dpi) / 96L);
                int maxH = (int)(((long)SMALL_H * (long)dpi) / 96L);
                bool small = (w <= maxW && h <= maxH);
                bool textInput = IsTextInputClass(hwnd);
                if (small && !textInput) {
                    long cur = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
                    if ((cur & WS_EX_NOACTIVATE) == 0) {
                        try {
                            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(cur | WS_EX_NOACTIVATE));
                            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED | SWP_NOZORDER);
                            e.NoActivateApplied = true;
                        } catch { e.NoActivateApplied = false; }
                    }
                }
            }
        } catch { }

        Pinned[idx] = e;
        PulseDisplayExecutionLock();
    }

    private static void RestoreMpoDefense(int idx) {
        if (idx < 0 || idx >= MAX_PINNED) return;
        PinnedEntry e = Pinned[idx];
        IntPtr hwnd = e.Hwnd;
        if (hwnd == IntPtr.Zero || !IsWindow(hwnd)) {
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
            IntPtr hwnd = Pinned[i].Hwnd;
            if (hwnd == IntPtr.Zero || !IsWindow(hwnd)) continue;

            if (Pinned[i].CornerApplied) {
                int pref = DWMWCP_ROUNDSMALL;
                try { DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT); } catch { }
            }
        }
        PulseDisplayExecutionLock();
    }

    private static void PulseDisplayExecutionLock() {
        try {
            _pulseUntil = GetTickCount64() + PULSE_MS;
            _pulseActive = true;
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
        } catch { }
    }

    private static void MaintainDisplayPulse() {
        if (!_pulseActive) return;
        try {
            if (GetTickCount64() >= _pulseUntil) {
                SetThreadExecutionState(ES_CONTINUOUS);
                _pulseActive = false;
                _pulseUntil = 0;
            }
        } catch { }
    }

    private static void ResetDisplayPulse() {
        if (!_pulseActive) { _pulseUntil = 0; return; }
        try { SetThreadExecutionState(ES_CONTINUOUS); } catch { }
        _pulseActive = false;
        _pulseUntil = 0;
    }

    private static void Enforce() {
        if (PinnedCount == 0) return;

        for (int i = PinnedCount - 1; i >= 0; i--) {
            IntPtr hw = Pinned[i].Hwnd;
            if (hw == IntPtr.Zero || !IsWindow(hw)) {
                UnpinIndex(i);
            }
        }

        if (PinnedCount == 0) return;

        for (int i = 0; i < PinnedCount; i++) {
            IntPtr p = Pinned[i].Hwnd;
            if (p == IntPtr.Zero || !IsWindow(p)) continue;

            if (IsIconic(p)) {
                if (!Pinned[i].WasIconic) {
                    PinnedEntry e = Pinned[i];
                    e.WasIconic = true;
                    Pinned[i] = e;
                    if (p != IntPtr.Zero && IsWindow(p)) {
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
                    }
                }
                continue;
            }

            if (Pinned[i].WasIconic) {
                PinnedEntry e = Pinned[i];
                e.WasIconic = false;
                Pinned[i] = e;
                if (p != IntPtr.Zero && IsWindow(p)) {
                    long curEx = GetWindowLongPtr(p, GWL_EXSTYLE).ToInt64();
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
                    if (nextEx != curEx) {
                        try { SetWindowLongPtr(p, GWL_EXSTYLE, new IntPtr(nextEx)); } catch { }
                    }
                    if ((nextEx & WS_EX_LAYERED) != 0) {
                        try {
                            SetLayeredWindowAttributes(p, 0, e.OrigAlpha, (e.OrigLwaFlags != 0) ? e.OrigLwaFlags : LWA_ALPHA);
                        } catch { }
                    }
                    if (e.CornerApplied) {
                        int pref = DWMWCP_ROUNDSMALL;
                        try { DwmSetWindowAttribute(p, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, CBINT); } catch { }
                    }
                    SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS | SWP_FRAMECHANGED);
                }
            }

            if (!IsWindowVisible(p)) continue;

            if (GetWindow(p, GW_HWNDPREV) != IntPtr.Zero) {
                SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_STEADY_FLAGS);
            }
        }

        MaintainDisplayPulse();
    }

    private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        if (idObject != 0) return;
        if (PinnedCount == 0) return;
        if (eventType == EVENT_SYSTEM_FOREGROUND) {
            Enforce();
            return;
        }
        if (hwnd != IntPtr.Zero) {
            IntPtr root = GetTrueRoot(hwnd);
            for (int i = 0; i < PinnedCount; i++) {
                if (Pinned[i].Hwnd == root || Pinned[i].Hwnd == hwnd) {
                    Enforce();
                    return;
                }
            }
        }
    }

    private static void OnHeartbeat(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime) {
        if (PinnedCount == 0) return;
        Enforce();
    }

    private static bool IsTransientPopup(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return true;
        StringBuilder sb = new StringBuilder(CLASS_BUF);
        try {
            if (GetClassName(hwnd, sb, sb.Capacity) > 0) {
                string cls = sb.ToString();
                if (cls == "tooltips_class32" || cls == "#32768") {
                    return true;
                }
                if (cls == "MSCTFIME UI" || cls == "IME") {
                    return true;
                }
            }
        } catch { }
        return false;
    }

    private static bool IsSystemShellWindow(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return true;
        StringBuilder sb = new StringBuilder(CLASS_BUF);
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
        StringBuilder sb = new StringBuilder(CLASS_BUF);
        if (GetClassName(hwnd, sb, sb.Capacity) > 0) {
            string cls = sb.ToString();
            if (cls.Contains("Valve001") || cls.Contains("UnrealWindow") || cls.Contains("UnityWndClass") ||
                cls.Contains("GLFW") || cls.Contains("SDL_app")) {
                return true;
            }
        }
        return false;
    }

    private static bool IsTextInputClass(IntPtr hwnd) {
        StringBuilder sb = new StringBuilder(CLASS_BUF);
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
            gui.cbSize = SizeOfGuiInfo;
            try {
                if (GetGUIThreadInfo(tid, ref gui)) {
                    if (gui.hwndCaret != IntPtr.Zero) {
                        return true;
                    }
                }
            } catch { }
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
        try {
            IntPtr hMon = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (hMon == IntPtr.Zero) return false;
            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = SizeOfMonitorInfo;
            if (!GetMonitorInfo(hMon, ref mi)) return false;
            RECT rc;
            if (!GetWindowRect(hwnd, out rc)) return false;
            if (rc.left <= mi.rcMonitor.left && rc.top <= mi.rcMonitor.top &&
                rc.right >= mi.rcMonitor.right && rc.bottom >= mi.rcMonitor.bottom) {
                return true;
            }
        } catch { }
        return false;
    }
}

