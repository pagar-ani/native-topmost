$ErrorActionPreference = "Stop"
$exePath = Join-Path $PSScriptRoot "TopmostDaemon.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "[*] Executable not found. Compiling first..." -ForegroundColor Yellow
    & "$PSScriptRoot\build.bat"
}

Write-Host "[*] Registering Scheduled Task 'TopmostDaemon' for silent auto-start on logon..." -ForegroundColor Cyan
$action = New-ScheduledTaskAction -Execute $exePath
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit 0

Register-ScheduledTask -TaskName "TopmostDaemon" -Action $action -Trigger $trigger -Settings $settings -Force | Out-Null
Start-ScheduledTask -TaskName "TopmostDaemon"

Write-Host "[+] Installation complete. TopmostDaemon is running in the background." -ForegroundColor Green
Write-Host "[+] Hotkey: Win + Ctrl + T to pin/unpin any active window." -ForegroundColor Green
