$ErrorActionPreference = "SilentlyContinue"
Write-Host "[*] Stopping and removing TopmostDaemon..." -ForegroundColor Yellow
Stop-ScheduledTask -TaskName "TopmostDaemon"
Stop-Process -Name "TopmostDaemon" -Force
Unregister-ScheduledTask -TaskName "TopmostDaemon" -Confirm:$false
Write-Host "[+] TopmostDaemon uninstalled successfully." -ForegroundColor Green
