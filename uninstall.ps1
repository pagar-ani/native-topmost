$ErrorActionPreference = "SilentlyContinue"
Write-Host "[*] Uninstalling TopmostDaemon..." -ForegroundColor Yellow

Stop-ScheduledTask -TaskName "TopmostDaemon"
Stop-Process -Name "TopmostDaemon" -Force
Unregister-ScheduledTask -TaskName "TopmostDaemon" -Confirm:$false | Out-Null

$targetDir = Join-Path $env:LOCALAPPDATA "TopmostDaemon"
if (Test-Path $targetDir) {
    Remove-Item -Path $targetDir -Recurse -Force
}

Write-Host "[+] TopmostDaemon completely removed from startup and disk." -ForegroundColor Green
