# native-topmost

A tiny, reliable "Always on Top" tool for Windows.

Built because Microsoft PowerToys Always-on-Top (`Ctrl + Win + T`) drops behind windows whenever you open full-screen games, video players, or switch monitors.

---

### Features

- **Stays on top**: Keeps pinned windows above full-screen apps, games, and web browsers.
- **Picture-in-Picture friendly**: Hover your mouse over any floating video player and press the hotkey. Clicks pass right through to media controls without stealing keyboard focus.
- **No graphics glitches**: Fixes the Windows 11 bug where pinning over hardware-accelerated video turns the taskbar black or freezes the mouse cursor.
- **Dialog safety**: "Save As" and file picker popups always stay in front of their parent window.
- **Smart stacking**: Pin multiple windows and they keep their stacking order. Focusing any pinned window (or its dialog) brings it to the top of the group.
- **Zero idle polling**: When zero windows are pinned, the timer is completely stopped (zero CPU wakeups). When windows are pinned, it checks at 250 ms during activity and slows down to 2.5 seconds when idle.
- **Crash recovery**: If the daemon is ever force-closed while windows are pinned, restarting it automatically scans and restores original window styles.
- **Lightweight**: A single 27 KB standalone executable using ~2 MB RAM on Windows 11 Efficiency cores.

---

### How to Use

- **Hotkey**: Press `Ctrl + Win + T` to pin or unpin.
- **Targeting**:
  - *Standard windows*: Click or focus the window first, then press `Ctrl + Win + T`.
  - *Picture-in-Picture*: Hover your mouse cursor over the floating video player and press `Ctrl + Win + T`.
  - *Hover unpin*: Hover your mouse over any already-pinned window and press `Ctrl + Win + T` to unpin it immediately.
- **Sound feedback**:
  - *Asterisk chime* (`MB_ICONASTERISK`): Pinned.
  - *Default beep* (`MB_OK`): Unpinned.
  - *Hand / error sound* (`MB_ICONHAND`): Ignored (system shell window or elevated window requiring Administrator rights).

> **Note**: If you have Microsoft PowerToys installed, turn off its built-in "Always on Top" feature in PowerToys Settings to avoid hotkey conflicts. The daemon will show an alert dialog on startup if the shortcut is already taken.

---

### Installation

1. Download the latest release from [Releases](https://github.com/pagar-ani/native-topmost/releases).
2. Double-click `install.bat`.
   - Copies `TopmostDaemon.exe` to `%LOCALAPPDATA%\TopmostDaemon\` and sets up a background task that starts silently at login.
   - *Optional*: Right-click and choose "Run as Administrator" if you want to pin elevated windows like Task Manager.

To uninstall anytime, double-click `uninstall.bat`.

---

### Building from Source

No SDKs or Visual Studio needed. Uses the C# compiler already built into Windows:

```cmd
build.bat
```

---

### License

Dual-licensed under the [GNU AGPLv3](LICENSE) and a commercial license. For commercial inquiries, contact [@pagar-ani](https://github.com/pagar-ani).