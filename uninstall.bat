@echo off
setlocal
echo [*] Uninstalling TopmostDaemon...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0uninstall.ps1"
endlocal
