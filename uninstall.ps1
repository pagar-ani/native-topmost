$ErrorActionPreference = "SilentlyContinue"
Write-Host "[*] Uninstalling TopmostDaemon..." -ForegroundColor Yellow

# 1. Graceful close so Cleanup() restores user window styles
Stop-ScheduledTask -TaskName "TopmostDaemon"
$running = Get-Process -Name "TopmostDaemon" -ErrorAction SilentlyContinue
if ($running) {
    $running.CloseMainWindow() | Out-Null
    $running | Wait-Process -Timeout 2 -ErrorAction SilentlyContinue
    if (Get-Process -Name "TopmostDaemon" -ErrorAction SilentlyContinue) {
        Stop-Process -Name "TopmostDaemon" -Force
        Start-Sleep -Milliseconds 200
    }
}

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    $task = Get-ScheduledTask -TaskName "TopmostDaemon" -ErrorAction SilentlyContinue
    if ($task -and $task.Principal.RunLevel -eq "Highest") {
        Write-Host "[!] WARNING: TopmostDaemon was registered with Administrator privileges." -ForegroundColor Yellow
        Write-Host "[!] Please run uninstall.bat as Administrator to fully unregister the scheduled task." -ForegroundColor Yellow
    }
}

Unregister-ScheduledTask -TaskName "TopmostDaemon" -Confirm:$false | Out-Null

$targetDir = Join-Path $env:LOCALAPPDATA "TopmostDaemon"
if (Test-Path $targetDir) {
    for ($i = 0; $i -lt 5; $i++) {
        Remove-Item -Path $targetDir -Recurse -Force
        if (-not (Test-Path $targetDir)) { break }
        Start-Sleep -Milliseconds 200
    }
}

Write-Host "[+] TopmostDaemon completely removed from startup and disk." -ForegroundColor Green
