@echo off
setlocal
echo [*] Compiling native-topmost using built-in Windows csc.exe...
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC%" (
    echo [!] csc.exe not found at %CSC%
    exit /b 1
)

"%CSC%" /target:winexe /optimize+ /platform:x64 /debug- /nologo /out:TopmostDaemon.exe TopmostDaemon.cs

if %ERRORLEVEL% equ 0 (
    echo [+] Compilation successful: TopmostDaemon.exe
) else (
    echo [!] Compilation failed.
)
endlocal
