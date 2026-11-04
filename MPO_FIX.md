# Windows 11 Multi-Plane Overlay (MPO) Fix

### What is the problem?

On Windows 11 systems with Intel integrated graphics (UHD 630 and newer) or dual-GPU (Nvidia Optimus) laptops, watching hardware-accelerated video (YouTube, Netflix, VLC, or browser Picture-in-Picture) can trigger a known graphics compositor glitch:

1. **The Taskbar turns pitch black** when you pin a window over a playing video.
2. **The mouse cursor stutters or freezes** intermittently.
3. **Picture-in-Picture windows won't stay behind other apps** after unpinning because Chromium-based browsers natively force `WS_EX_TOPMOST` on floating viewports.

---

### Why does this happen?

Modern graphics drivers use a feature called **Multi-Plane Overlays (MPO)**. Instead of drawing the desktop, the video, and the mouse cursor together in one pass, the GPU splits them into separate hardware scanout planes:

- **Plane 1**: The desktop canvas and Taskbar.
- **Plane 2**: The direct hardware video plane (YouTube / media players).
- **Plane 3**: The hardware mouse cursor.

When a standard window is pinned on top of the video (Plane 2), Windows Desktop Window Manager (DWM) tries to dynamically demote the video plane back into normal software composition. On many Intel and Optimus driver versions, this handoff causes a timing conflict between the Taskbar blur shader and the display engine.

DWM aborts the composition pass to avoid a total graphics crash, leaving the Taskbar unrendered (black) and starving the hardware cursor line buffer (frozen pointer).

---

### How native-topmost fixes it

`native-topmost` handles this at the window management level without requiring you to disable hardware acceleration in your browser or edit registry MPO keys:

1. **Subtle Corner Disqualification**: On standard displays (< 168 DPI), the daemon applies `DWMWCP_ROUNDSMALL` (micro-rounded corners) or layered attributes to pinned media windows. This signals Windows DWM to safely composite the video surface through the standard composition swapchain rather than a conflicting direct-flip hardware plane.
2. **Dynamic Suspension for Gaming & Fullscreen**: When a pinned app is maximized or enters true fullscreen (F11), all MPO overrides are automatically suspended so you get 100% full hardware scanout performance for gaming and video playback.
3. **Clean Unpinning**: When unpinning a browser Picture-in-Picture window, the daemon strips the browser's native `WS_EX_TOPMOST` extended style and demotes it cleanly behind active applications.

---

### Verifying the Fix

You can test this easily:
1. Open a hardware-accelerated video in Chrome, Edge, or Firefox.
2. Pop it out into Picture-in-Picture mode.
3. Press `Win + Ctrl + T` to pin another window directly on top of it.
4. Your Taskbar and mouse cursor will remain fully responsive and visible.