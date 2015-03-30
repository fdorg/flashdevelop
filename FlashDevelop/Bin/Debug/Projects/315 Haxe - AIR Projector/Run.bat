@echo off
set PAUSE_ERRORS=1
call bat\SetupSDK.bat
call bat\SetupApplication.bat

echo.
echo Starting AIR Debug Launcher...
echo.

:: Read supported profiles and remove desktop
for /f "tokens=3 delims=<>" %%a in ('findstr /R /C:"^[ 	]*<supportedProfiles>" %APP_XML%') do set PROFILES=%%a
set PROFILES=%PROFILES:desktop =%

:: Exec adl with profile if its not desktop
if %PROFILES% == "" ( adl "%APP_XML%" "%APP_DIR%" ) else ( adl -profile %PROFILES% "%APP_XML%" "%APP_DIR%" )
if errorlevel 1 goto error
goto end

:error
pause

:end