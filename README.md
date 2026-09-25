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

### Performance Metrics

Tested on Windows 11 x64:

| Metric | PowerShell Version | `native-topmost.exe` |
| :--- | :--- | :--- |
| **Binary Footprint** | N/A (Script) | **7,680 bytes** (7.5 KB) |
| **RAM (Working Set)** | ~73.0 MB | **~2.5 MB** |
| **Steady-State Allocations** | Dynamic Heap | **0 bytes** ($GC_0 = 0$) |
| **Cache Storage** | Dynamic Map | **Contiguous 128 bytes** (2 L1 lines) |
| **Subsystem** | Console (`conhost.exe`) | Windows GUI (`winexe`, zero terminal flash) |

---

### How to use

#### 1. Hotkey
- **Toggle Pin / Unpin**: `Win + Ctrl + T` on any active window.
- **Audio indicator**: High beep on pin, low beep on unpin.

*Note: Ensure PowerToys native "Always on Top" is toggled OFF in PowerToys Settings to avoid hotkey collision.*

#### 2. Build from Source
Compile directly using the C# compiler already present in your Windows installation:

```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /optimize+ /platform:x64 /debug- /nologo /out:TopmostDaemon.exe TopmostDaemon.cs
```

#### 3. Run on Startup (Living-off-the-land)
Register native Windows Scheduled Task so it runs silently on logon:

```powershell
$action = New-ScheduledTaskAction -Execute "$PWD\TopmostDaemon.exe"
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit 0
Register-ScheduledTask -TaskName "TopmostDaemon" -Action $action -Trigger $trigger -Settings $settings -Force
```

To run immediately without logging off:
```powershell
Start-ScheduledTask -TaskName "TopmostDaemon"
```

---

### Boundary Conditions & Technical Doubts

1. **Administrator Applications (UIPI)**: If target window runs elevated (Task Manager, anti-cheat games), Windows User Interface Privilege Isolation blocks standard user processes. For such cases, run the scheduled task with highest privileges (`-RunLevel Highest`).
2. **Hardware Exclusive Fullscreen (FSE)**: If legacy games bypass DWM compositor completely via exclusive hardware scanout, ensure **"Disable fullscreen optimizations"** is UNCHECKED in executable properties so DWM flip presentation remains active.

---

### License
MIT. Do the needful and use freely.
