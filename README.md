# native-topmost

Ultra-lean Win32 topmost enforcer. Built to fix Microsoft PowerToys Always-on-Top (`Win + Ctrl + T`) which fails whenever any application goes fullscreen.

Living-off-the-land (LOTL) implementation. No AutoHotkey, no background runtimes, no third-party circus.

---

### Why this is required itself?

PowerToys Always-on-Top sets `HWND_TOPMOST` only once on hotkey trigger. When another application enters fullscreen and gains focus, Windows Desktop Window Manager (DWM) brings the active window to the front of the Z-stack. Result: pinned window goes underneath itself.

`native-topmost` resolves this properly:
1. **Root Handle Resolution**: Uses `GetAncestor(hwnd, GA_ROOT)` so Chromium, Electron (Discord, VS Code, Slack), and UWP windows pin the actual parent frame, not internal child controls.
2. **Cursor-Aware Targeting**: For floating media and Picture-in-Picture (PiP) viewports, hovering the cursor over the window and pressing `Win + Ctrl + T` targets the window directly without requiring keyboard focus.
3. **Focus-Free Media Viewports**: Pinned Picture-in-Picture windows receive `WS_EX_NOACTIVATE`. Play, pause, and scrubber clicks pass directly to media controls without stealing keyboard focus from underlying applications.
4. **Dynamic Topmost Stripping and Demotion**: If an unpinned fullscreen app sets `WS_EX_TOPMOST` to steal focus, its topmost bit is stripped immediately and sent to `HWND_NOTOPMOST`. When unpinning a window, it is cleanly demoted behind active applications.
5. **Continuous Z-Guard**: Dual-trigger architecture (`EVENT_SYSTEM_FOREGROUND` hook + 350ms heartbeat) guarantees pinned windows stay at apex. When zero windows are pinned, the heartbeat timer terminates completely to ensure zero wakeups per second.
6. **DWM Syscall Gate**: Checks `GetWindow(p, GW_HWNDPREV)`. If window is already at apex, syscalls are bypassed. Zero redundant IPC to DWM.

---

### Specifications

- **Binary Footprint**: 18,432 bytes (18 KB)
- **RAM (Working Set)**: ~2.5 MB
- **Steady-State Allocations**: 0 bytes ($GC_0 = 0$)
- **Internal Storage**: 128 bytes contiguous flat array (2 L1 cache lines)
- **Subsystem**: Pure Windows GUI (`winexe`, zero terminal flash)

---

### Installation & Usage

> [!IMPORTANT]
> **Mandatory Prerequisite**: If you have Microsoft PowerToys installed, open **PowerToys Settings -> Always on Top** and toggle it **OFF**. Ensure PowerToys native "Always on Top" is toggled OFF in PowerToys Settings to avoid hotkey collision.

#### 1. Setup (Two Options)

##### Option A: Grab Pre-Built Release (Easiest)
1. Download source zip or binary from [Releases](https://github.com/pagar-ani/native-topmost/releases).
2. Double-click `install.bat`.
   - **Where does it go?** It automatically deploys `TopmostDaemon.exe` into `%LOCALAPPDATA%\TopmostDaemon\`. Even if you empty your `Downloads` folder later, it will not break.
   - **Safe Execution**: Uses scoped execution bypass for that command only. Zero weakening of your system-wide PowerShell `ExecutionPolicy`. No registry tampering.

##### Option B: Build Cleanly from Source
Clone the repo and run:
```cmd
build.bat
```
*(Uses native `csc.exe` already present in `%SystemRoot%\Microsoft.NET`. No SDKs or Visual Studio needed).*

Then run:
```cmd
install.bat
```

---

#### 2. Hotkey
- **Toggle Pin / Unpin**: `Win + Ctrl + T` on any active window or while hovering over floating media.
- **Audio indicator**: High beep on pin, low beep on unpin.

*Note: Ensure PowerToys native "Always on Top" is toggled OFF in PowerToys Settings to avoid hotkey collision.*

---

#### 3. Uninstallation
To completely remove at any time, simply run:
```cmd
uninstall.bat
```
This stops the process, unregisters the Task Scheduler entry, and wipes `%LOCALAPPDATA%\TopmostDaemon` cleanly from disk.

---

### Boundary Conditions and Operational Provisions

1. **Administrative Domain Restrictions (UIPI)**: Where a designated target window functions under elevated administrative integrity (e.g., Task Manager or executables fortified with anti-cheat protection), Windows User Interface Privilege Isolation strictly impedes unprivileged messaging. In such eventualities, it is necessary to register the underlying Task Scheduler entry with elevated credentials (`-RunLevel Highest`). Right-click `install.bat` and select **Run as Administrator** to enable this automatically.
2. **Hardware Exclusive Fullscreen (FSE)**: Should legacy applications bypass the Desktop Window Manager (DWM) composition pipeline via exclusive hardware scanout acquisition, verify that **"Disable fullscreen optimizations"** remains unchecked within executable properties, thereby preserving standard DWM flip presentation semantics.
3. **Multi-Plane Overlay (MPO) Contention Remediation**: When a hardware-accelerated video or detached Picture-in-Picture (PiP) canvas plays while a windowed borderless application or desktop resides behind it, systems with Intel integrated graphics (Gen 9.5 / UHD 630) or dual-GPU Optimus configurations suffer physical compositor failure under Windows 11 WDDM 3.x. The display engine places the direct-flip video on Plane 2 while the desktop and Taskbar reside on Plane 1. When a window is pinned above Plane 2, the display controller cannot physically scan out Plane 1 in front of Plane 2. DWM triggers an emergency dynamic demotion during which the Taskbar Acrylic blur compute shader encounters a read-lock collision with the scanline readout FIFO, aborting composition and causing the **Taskbar to turn pitch black**. Simultaneously, the dedicated line FIFO for the hardware cursor (Plane 3) starves, causing the **mouse cursor to freeze or disappear**. Furthermore, Chromium viewports spawn with `WS_EX_TOPMOST` natively, which prevents standard unpin routines from demoting the surface.

   Our production engine permanently resolves this condition through silicon-level geometry disqualification (`DWMWCP_ROUNDSMALL`), hidden thumbnail redirection observers, bare layered fallbacks, and focus arbitration guards. For the comprehensive architectural root cause analysis and verbatim production source routines, please consult [MPO_FIX.md](MPO_FIX.md).

---

### License

This software is dual-licensed under the GNU Affero General Public License v3.0 (AGPL-3.0) and a separate Commercial License.

#### Open Source Use (AGPL-3.0)
Permission is granted to use, modify, and distribute this software free of charge under the terms of the [GNU AGPL-3.0](LICENSE). Under this license:
* You must make all modifications and integrated source code available under the AGPL-3.0.
* Network access to a modified version triggers the requirement to provide the complete source code to all network users.

#### Commercial and Proprietary Exemption
If you intend to incorporate this software into proprietary products, distribute it within closed-source environments, or deploy it without complying with the copyleft obligations of the AGPL-3.0, you must obtain a commercial license.

For commercial licensing, enterprise deployment terms, or custom agreements, contact:
* GitHub: [@pagar-ani](https://github.com/pagar-ani)
