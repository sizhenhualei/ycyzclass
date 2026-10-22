@echo off
setlocal
cd /d "%~dp0"
net session >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Please right-click this file and choose "Run as administrator".
    pause
    exit /b 1
)
echo Installing YcyzClass code signing certificate into LocalMachine stores...
certutil -addstore -f Root "%~dp0YcyzClass-CodeSigning.cer"
certutil -addstore -f TrustedPublisher "%~dp0YcyzClass-CodeSigning.cer"
if errorlevel 1 (
    echo [ERROR] Import failed.
    pause
    exit /b 1
)
echo.
echo Done. Signed YcyzClass builds now show publisher "CN=Si Zhenhualei x DeepSeekV4.1".
pause
