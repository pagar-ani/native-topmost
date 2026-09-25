$ErrorActionPreference = "Stop"

$targetDir = Join-Path $env:LOCALAPPDATA "TopmostDaemon"
$targetExe = Join-Path $targetDir "TopmostDaemon.exe"
$sourceExe = Join-Path $PSScriptRoot "TopmostDaemon.exe"

# If binary is missing in source folder, compile it first
if (-not (Test-Path $sourceExe)) {
    Write-Host "[*] TopmostDaemon.exe not found. Compiling from source..." -ForegroundColor Yellow
    & "$PSScriptRoot\build.bat"
    if (-not (Test-Path $sourceExe)) {
        Write-Error "[!] Build failed. Exiting."
        exit 1
    }
}

# Create permanent user application directory
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

# Stop running daemon before updating binary
Stop-ScheduledTask -TaskName "TopmostDaemon" -ErrorAction SilentlyContinue
Stop-Process -Name "TopmostDaemon" -Force -ErrorAction SilentlyContinue
Start-Sleep -Milliseconds 300

# Deploy binary to permanent home
Copy-Item -Path $sourceExe -Destination $targetExe -Force
Write-Host "[+] Binary deployed to permanent location: $targetExe" -ForegroundColor Cyan

# Register scheduled task to run on current user's logon (no admin required)
Write-Host "[*] Registering Scheduled Task 'TopmostDaemon' for silent startup..." -ForegroundColor Cyan
$action = New-ScheduledTaskAction -Execute $targetExe
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit 0

Register-ScheduledTask -TaskName "TopmostDaemon" -Action $action -Trigger $trigger -Settings $settings -Force | Out-Null
Start-ScheduledTask -TaskName "TopmostDaemon"

Write-Host "[+] Installation complete. TopmostDaemon is actively running." -ForegroundColor Green
Write-Host "[+] Hotkey: Win + Ctrl + T is armed." -ForegroundColor Green
