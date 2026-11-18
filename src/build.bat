@echo off
rem Reconstruieste SageataDreapta.exe folosind compilatorul C# care vine cu Windows.
setlocal
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
cd /d "%~dp0"
if not exist icon.ico powershell -NoProfile -ExecutionPolicy Bypass -File make-icon.ps1
"%CSC%" /nologo /target:winexe /optimize+ /codepage:65001 /win32icon:icon.ico /out:..\SageataDreapta.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll SageataDreapta.cs
if errorlevel 1 (echo. & echo EROARE la compilare. & pause & exit /b 1)
echo Gata: ..\SageataDreapta.exe
