# native-topmost

Ultra-lean Win32 topmost enforcer. Built to fix Microsoft PowerToys Always-on-Top (`Win + Ctrl + T`) which fails whenever any application goes fullscreen.

Living-off-the-land (LOTL) implementation. No AutoHotkey, no background runtimes, no third-party circus.

---

### Why this is required itself?

PowerToys Always-on-Top sets `HWND_TOPMOST` only once on hotkey trigger. When another application enters fullscreen and gains focus, Windows Desktop Window Manager (DWM) brings the active window to the front of the Z-stack. Result: pinned window goes underneath itself.

`native-topmost` resolves this properly:
1. **Root Handle Resolution**: Uses `GetAncestor(hwnd, GA_ROOT)` so Chromium, Electron (Discord, VS Code, Slack), and UWP windows pin the actual parent frame, not internal child controls.
2. **Dynamic Topmost Stripping**: If an unpinned fullscreen app sets `WS_EX_TOPMOST` to steal focus, its topmost bit is stripped immediately and sent to `HWND_NOTOPMOST`.
3. **Continuous Z-Guard**: Dual-trigger architecture (`EVENT_SYSTEM_FOREGROUND` hook + 350ms heartbeat) guarantees pinned windows stay at apex.
4. **DWM Syscall Gate**: Checks `GetWindow(p, GW_HWNDPREV)`. If window is already at apex, syscalls are bypassed. Zero redundant IPC to DWM.

---

### Specifications

- **Binary Footprint**: 7,680 bytes (7.5 KB)
- **RAM (Working Set)**: ~2.5 MB
- **Steady-State Allocations**: 0 bytes ($GC_0 = 0$)
- **Internal Storage**: 128 bytes contiguous flat array (2 L1 cache lines)
- **Subsystem**: Pure Windows GUI (`winexe`, zero terminal flash)

---

### How to use

#### 1. Download or Build
- **Download**: Grab `TopmostDaemon.exe` directly from [Releases](https://github.com/pagar-ani/native-topmost/releases).
- **Or Build from Source**: Run `build.bat` (uses native Windows C# compiler; zero installs needed).

#### 2. Hotkey
- **Toggle Pin / Unpin**: `Win + Ctrl + T` on any active window.
- **Audio indicator**: High beep on pin, low beep on unpin.

*Note: Ensure PowerToys native "Always on Top" is toggled OFF in PowerToys Settings to avoid hotkey collision.*

#### 3. Run on Startup
Run `install.ps1` from PowerShell to register silent auto-start on logon:

```powershell
.\install.ps1
```

To remove at any time:
```powershell
.\uninstall.ps1
```

---

### Boundary Conditions & Technical Doubts

1. **Administrator Applications (UIPI)**: If target window runs elevated (Task Manager, anti-cheat games), Windows User Interface Privilege Isolation blocks standard user processes. For such cases, run the scheduled task with highest privileges (`-RunLevel Highest`).
2. **Hardware Exclusive Fullscreen (FSE)**: If legacy games bypass DWM compositor completely via exclusive hardware scanout, ensure **"Disable fullscreen optimizations"** is UNCHECKED in executable properties so DWM flip presentation remains active.

---

### License
MIT. Do the needful and use freely.
