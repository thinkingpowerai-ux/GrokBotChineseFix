@echo off
setlocal
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
  echo .NET Framework 4.x C# compiler was not found.
  exit /b 1
)
"%CSC%" /nologo /target:winexe /platform:anycpu /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /out:"%~dp0GrokBotChineseFix.exe" "%~dp0GrokBotChineseFix.cs"
exit /b %ERRORLEVEL%
