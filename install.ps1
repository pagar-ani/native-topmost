$ErrorActionPreference = "Stop"

$targetDir = Join-Path $env:LOCALAPPDATA "TopmostDaemon"
$targetExe = Join-Path $targetDir "TopmostDaemon.exe"
$sourceExe = Join-Path $PSScriptRoot "TopmostDaemon.exe"

if (-not (Test-Path $sourceExe)) {
    Write-Host "[*] TopmostDaemon.exe not found. Compiling from source..." -ForegroundColor Yellow
    & "$PSScriptRoot\build.bat"
    if (-not (Test-Path $sourceExe)) {
        Write-Error "[!] Build failed. Exiting."
        exit 1
    }
}

if (Get-Process -Name "PowerToys.AlwaysOnTop" -ErrorAction SilentlyContinue) {
    Write-Host "[!] WARNING: Microsoft PowerToys Always-on-Top is currently RUNNING." -ForegroundColor Yellow
    Write-Host "[!] Please open PowerToys Settings -> 'Always on Top' and turn it OFF to avoid shortcut collision." -ForegroundColor Yellow
}

if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
}

Stop-ScheduledTask -TaskName "TopmostDaemon" -ErrorAction SilentlyContinue
$running = Get-Process -Name "TopmostDaemon" -ErrorAction SilentlyContinue
if ($running) {
    Stop-Process -Name "TopmostDaemon" -Force -ErrorAction SilentlyContinue
    $running | Wait-Process -Timeout 3 -ErrorAction SilentlyContinue
}
Start-Sleep -Milliseconds 200

$copied = $false
for ($i = 0; $i -lt 5; $i++) {
    try {
        Copy-Item -Path $sourceExe -Destination $targetExe -Force
        $copied = $true
        break
    } catch {
        Start-Sleep -Milliseconds 200
    }
}
if (-not $copied) {
    Copy-Item -Path $sourceExe -Destination $targetExe -Force
}
Write-Host "[+] Binary deployed to permanent location: $targetExe" -ForegroundColor Cyan

Write-Host "[*] Registering Scheduled Task 'TopmostDaemon' for silent startup..." -ForegroundColor Cyan
$action = New-ScheduledTaskAction -Execute $targetExe
$trigger = New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -ExecutionTimeLimit 0

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if ($isAdmin) {
    Write-Host "[+] Administrative elevation detected: arming with Highest RunLevel (UIPI immunity enabled)." -ForegroundColor Green
    $principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -LogonType Interactive -RunLevel Highest
    Register-ScheduledTask -TaskName "TopmostDaemon" -Action $action -Trigger $trigger -Settings $settings -Principal $principal -Force | Out-Null
} else {
    Write-Host "[*] Standard user session detected: registering task without elevation." -ForegroundColor Cyan
    Write-Host "[i] (To enable UIPI immunity for elevated windows, run install.bat as Administrator)." -ForegroundColor Yellow
    Register-ScheduledTask -TaskName "TopmostDaemon" -Action $action -Trigger $trigger -Settings $settings -Force | Out-Null
}
Start-ScheduledTask -TaskName "TopmostDaemon"

Write-Host "[+] Installation complete. TopmostDaemon is actively running." -ForegroundColor Green
Write-Host "[+] Hotkey: Win + Ctrl + T is armed." -ForegroundColor Green
