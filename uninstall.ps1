$ErrorActionPreference = "SilentlyContinue"
Write-Host "[*] Uninstalling TopmostDaemon..." -ForegroundColor Yellow

Stop-ScheduledTask -TaskName "TopmostDaemon"
$running = Get-Process -Name "TopmostDaemon" -ErrorAction SilentlyContinue
if ($running) {
    Stop-Process -Name "TopmostDaemon" -Force
    $running | Wait-Process -Timeout 3 -ErrorAction SilentlyContinue
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
