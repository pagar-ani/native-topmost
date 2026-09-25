@echo off
setlocal
echo [*] Installing TopmostDaemon...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0install.ps1"
endlocal
