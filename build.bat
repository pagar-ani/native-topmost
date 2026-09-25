@echo off
setlocal enabledelayedexpansion
echo [*] Compiling native-topmost using built-in Windows csc.exe...

rem Terminate running daemon if locked for compilation
taskkill /f /im TopmostDaemon.exe >nul 2>&1

set "CSC="
if exist "%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" (
    set "CSC=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
    set "PLATFORM=x64"
) else if exist "%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe" (
    set "CSC=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
    set "PLATFORM=x86"
)

if "%CSC%"=="" (
    echo [!] Built-in .NET Framework csc.exe not found under %SystemRoot%\Microsoft.NET.
    exit /b 1
)

pushd "%~dp0"
"%CSC%" /target:winexe /optimize+ /platform:%PLATFORM% /debug- /nologo /out:TopmostDaemon.exe TopmostDaemon.cs
set BUILD_ERR=%ERRORLEVEL%
popd

if %BUILD_ERR% equ 0 (
    echo [+] Compilation successful: TopmostDaemon.exe
) else (
    echo [!] Compilation failed with code %BUILD_ERR%.
)

exit /b %BUILD_ERR%
