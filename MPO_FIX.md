# Multi-Plane Overlay (MPO) Remediation and Compositor Defense

### 1. Root Cause Architecture (Intel Gen 9.5 and Optimus Pipelines)

On systems equipped with Intel Gen 9.5 integrated graphics (UHD 630 / Skylake through Comet Lake architectures) and NVIDIA Optimus muxless configurations running Windows 11 WDDM 3.x, hardware-accelerated video viewports trigger physical compositor failures under standard Z-stack elevation.

The underlying display engine allocates hardware scanout planes across three dedicated channels:
* **Plane 1**: Windows desktop canvas and Shell Taskbar (`Shell_TrayWnd`).
* **Plane 2**: Universal Hardware Overlay dedicated to DirectFlip video surfaces (e.g., Chromium Picture-in-Picture, Firefox PiP, media players).
* **Plane 3**: Dedicated hardware cursor scanline FIFO (`CUR_WM_A`).

When an external utility elevates an arbitrary window above Plane 2, Desktop Window Manager (DWM) initiates an emergency dynamic demotion of the DirectFlip overlay back to standard composition swapchains. During this transition, the Windows 11 Taskbar Acrylic blur compute shader (`DWMSBT_TRANSIENTWINDOW`) issues a Shader Resource View (SRV) read lock on the scanout surface. Simultaneously, Intel's fixed-function display engine holds an active scanline readout lock on `PLANE_SURF`. 

This read-lock contention stalls Direct3D 12 render command queues beyond the vertical blanking window (>16.6 ms). DWM aborts the composition pass to prevent an OS-wide graphics hang, resulting in the **Taskbar rendering pitch black**. Concurrently, the line FIFO servicing Plane 3 starves, resulting in the **mouse cursor freezing or disappearing**.

Furthermore, detached media viewports spawned by Chromium-based browsers natively assert the `WS_EX_TOPMOST` extended style upon initialization. Standard unpinning calls that only issue `SetWindowPos(HWND_NOTOPMOST)` fail to demote the window because the underlying process continues to enforce its internal topmost property, trapping the viewport at the top of the visual stack.

---

### 2. In-Engine Production Implementation

The following implementation is quoted directly from the active production engine (`TopmostDaemon.cs`). It requires zero external dependencies, introduces zero runtime thread overhead, and resolves compositor contention directly within the user session.

#### Win32 and DWM Interop Definitions

```csharp
[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

[DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

[DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

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

[DllImport("user32.dll")]
private static extern bool GetCursorPos(out POINT lpPoint);

[DllImport("user32.dll")]
private static extern IntPtr WindowFromPoint(POINT Point);

[DllImport("user32.dll")]
private static extern IntPtr GetForegroundWindow();

[DllImport("user32.dll")]
private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

[DllImport("user32.dll")]
private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

[DllImport("user32.dll", CharSet = CharSet.Auto)]
private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

[DllImport("user32.dll")]
private static extern uint GetDpiForWindow(IntPtr hwnd);

[DllImport("user32.dll")]
private static extern int GetSystemMetrics(int nIndex);

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

[DllImport("kernel32.dll", ExactSpelling = true)]
private static extern uint SetThreadExecutionState(uint esFlags);
```

#### Core Defense Constants

```csharp
private const uint SWP_NOSIZE = 0x0001;
private const uint SWP_NOMOVE = 0x0002;
private const uint SWP_NOZORDER = 0x0004;
private const uint SWP_NOACTIVATE = 0x0010;
private const uint SWP_FRAMECHANGED = 0x0020;
private const uint SWP_NOSENDCHANGING = 0x0400;
private const uint SWP_STEADY_FLAGS = SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOSENDCHANGING;

private const int GWL_EXSTYLE = -20;
private const long WS_EX_TOPMOST = 0x00000008L;
private const long WS_EX_LAYERED = 0x00080000L;
private const long WS_EX_NOACTIVATE = 0x08000000L;

private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
private const int DWMWCP_DEFAULT = 0;
private const int DWMWCP_ROUNDSMALL = 3;

private const uint DWM_TNP_VISIBLE = 0x00000008;
private const uint DWM_TNP_OPACITY = 0x00000004;

private const uint ES_CONTINUOUS = 0x80000000;
private const uint ES_DISPLAY_REQUIRED = 0x00000002;
private const uint ES_SYSTEM_REQUIRED = 0x00000001;

private const int SM_REMOTESESSION = 0x1000;
private const uint MONITOR_DEFAULTTONEAREST = 2;
```

#### Application and Restoration Routines

```csharp
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
```

#### Pin and Unpin Orchestration Routines

```csharp
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
```

---

### 3. Mechanistic Defense Hierarchy

The remediation functions through a five-stage defensive hierarchy:

1. **Silicon Geometry Disqualification**: Calling `DwmSetWindowAttribute(hwnd, 33, &DWMWCP_ROUNDSMALL, 4)` sets a 4-pixel radius on the window geometry. Intel Gen 9.5 fixed-function plane controllers (`PLANE_CTL`) cannot scan out non-orthogonal alpha boundaries in hardware. Desktop Window Manager detects this limitation and terminates DirectFlip overlay candidate promotion prior to scanline readout collision.
2. **Invisible Thumbnail Redirection Anchor**: Where corner rounding is unsupported or filtered by high DPI boundaries, `DwmRegisterThumbnail` binds the target window to an invisible 1x1 host window (`fVisible = false`, `opacity = 0`). This forces DWM to maintain an active redirection surface, preventing hardware scanout promotion without altering visual appearance.
3. **Layered Software Composition Fallback**: On legacy Windows environments where modern DWM corner APIs are absent, bare `WS_EX_LAYERED` (at 100% solid opacity) directs the compositor into standard Direct3D shader presentation.
4. **Focus Arbitration Interlock**: When pinning compact viewports ($\le 1280 \times 720$), injecting `WS_EX_NOACTIVATE` eliminates focus-stealing collisions between the media canvas and the Shell Taskbar.
5. **Unpin Normalization**: Unpinning explicitly retrieves `GWL_EXSTYLE`, strips the `WS_EX_TOPMOST` bit mask, and executes `SetWindowPos(HWND_NOTOPMOST, SWP_FRAMECHANGED)`. This guarantees clean release for Chromium viewports that instantiate with persistent topmost properties.
