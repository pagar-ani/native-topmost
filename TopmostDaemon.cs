using System;
using System.Runtime.InteropServices;

internal static class Program {
    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "SetWindowLongPtrW")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int GetMessageW(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr DispatchMessageW(ref MSG lpMsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerDelegate lpTimerFunc);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern bool MessageBeep(uint uType);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    private delegate void TimerDelegate(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

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
    }

    // Win32 Constants
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    private const uint SWP_FLAGS = 0x0001 | 0x0002 | 0x0010 | 0x0040; // SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW
    private const uint SWP_DEMOTE_FLAGS = 0x0001 | 0x0002 | 0x0010;   // SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_WIN = 0x0008;
    private const uint VK_T = 0x54;
    private const int HOTKEY_ID = 9001;
    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    private const uint GA_ROOT = 2;
    private const uint GW_HWNDPREV = 3;
    private const int GWL_EXSTYLE = -20;
    private const long WS_EX_TOPMOST = 0x00000008L;

    // Cache-aligned flat storage (16 * 8 = 128 bytes = exactly 2 L1 Cache Lines)
    // Zero heap allocations, O(1) swap-removal, zero GC pressure
    private static readonly IntPtr[] Pinned = new IntPtr[16];
    private static int PinnedCount = 0;

    private static WinEventDelegate _hookDelegate;
    private static TimerDelegate _timerDelegate;
    private static IntPtr _lastForeground = IntPtr.Zero;

    [STAThread]
    private static void Main() {
        if (!RegisterHotKey(IntPtr.Zero, HOTKEY_ID, MOD_CONTROL | MOD_WIN, VK_T)) {
            return;
        }

        _hookDelegate = OnForegroundChanged;
        IntPtr hHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _hookDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

        _timerDelegate = OnHeartbeat;
        UIntPtr timerId = SetTimer(IntPtr.Zero, UIntPtr.Zero, 350, _timerDelegate);

        // Strip working set to bare metal baseline (< 2 MB RAM)
        SetProcessWorkingSetSize(GetCurrentProcess(), new IntPtr(-1), new IntPtr(-1));

        MSG msg;
        while (GetMessageW(out msg, IntPtr.Zero, 0, 0) > 0) {
            if (msg.message == 0x0312 && msg.wParam.ToInt32() == HOTKEY_ID) {
                ToggleActiveWindow();
            }
            TranslateMessage(ref msg);
            DispatchMessageW(ref msg);
        }

        if (timerId != UIntPtr.Zero) KillTimer(IntPtr.Zero, timerId);
        if (hHook != IntPtr.Zero) UnhookWinEvent(hHook);
        UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
    }

    private static IntPtr GetTrueRoot(IntPtr hwnd) {
        if (hwnd == IntPtr.Zero) return IntPtr.Zero;
        IntPtr root = GetAncestor(hwnd, GA_ROOT);
        return (root != IntPtr.Zero) ? root : hwnd;
    }

    private static bool Contains(IntPtr hwnd) {
        for (int i = 0; i < PinnedCount; i++) {
            if (Pinned[i] == hwnd) return true;
        }
        return false;
    }

    private static void ToggleActiveWindow() {
        IntPtr fg = GetForegroundWindow();
        IntPtr root = GetTrueRoot(fg);
        if (root == IntPtr.Zero) return;

        for (int i = 0; i < PinnedCount; i++) {
            if (Pinned[i] == root) {
                // Unpin: O(1) swap with tail
                Pinned[i] = Pinned[--PinnedCount];
                Pinned[PinnedCount] = IntPtr.Zero;
                SetWindowPos(root, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_FLAGS);
                MessageBeep(0x00000000);
                return;
            }
        }

        if (PinnedCount < Pinned.Length) {
            Pinned[PinnedCount++] = root;
            SetWindowPos(root, HWND_TOPMOST, 0, 0, 0, 0, SWP_FLAGS);
            MessageBeep(0x00000040);
        }
    }

    private static void Enforce() {
        if (PinnedCount == 0) return;

        // O(1) pruning of destroyed windows - zero allocations
        for (int i = PinnedCount - 1; i >= 0; i--) {
            if (!IsWindow(Pinned[i])) {
                Pinned[i] = Pinned[--PinnedCount];
                Pinned[PinnedCount] = IntPtr.Zero;
            }
        }

        if (PinnedCount == 0) return;

        IntPtr fg = GetForegroundWindow();
        IntPtr fgRoot = GetTrueRoot(fg);

        // State divergence check: suppress redundant Win32/DWM syscalls if state is canonical
        bool fgChanged = (fgRoot != _lastForeground);
        _lastForeground = fgRoot;

        // If foreground is an unpinned fullscreen/topmost competitor, demote it
        if (fgRoot != IntPtr.Zero && !Contains(fgRoot)) {
            long exStyle = GetWindowLongPtr(fgRoot, GWL_EXSTYLE).ToInt64();
            if ((exStyle & WS_EX_TOPMOST) != 0) {
                SetWindowLongPtr(fgRoot, GWL_EXSTYLE, new IntPtr(exStyle & ~WS_EX_TOPMOST));
                SetWindowPos(fgRoot, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_DEMOTE_FLAGS);
            }
        }

        // Re-assert apex position for pinned handles only when not already at apex
        for (int i = 0; i < PinnedCount; i++) {
            IntPtr p = Pinned[i];
            // Hardware/DWM fast path: If window has no predecessor in Z-order, it is ALREADY top!
            // Skip SetWindowPos syscall if already apex and foreground hasn't shifted
            if (fgChanged || GetWindow(p, GW_HWNDPREV) != IntPtr.Zero) {
                SetWindowPos(p, HWND_TOPMOST, 0, 0, 0, 0, SWP_FLAGS);
            }
        }
    }

    private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
        if (idObject != 0) return;
        Enforce();
    }

    private static void OnHeartbeat(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime) {
        Enforce();
    }
}
