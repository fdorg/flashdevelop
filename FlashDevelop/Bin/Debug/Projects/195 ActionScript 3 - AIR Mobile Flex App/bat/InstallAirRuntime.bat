@echo off

:: Set working dir
cd %~dp0 & cd ..

set PAUSE_ERRORS=1
call bat\SetupSDK.bat

:: AIR runtime installer
set AIR_INSTALLER=%FLEX_SDK%\runtimes\air\android\device\runtime.apk

:: Install
adb devices
echo.
echo Installing AIR runtime on current device:
echo %AIR_INSTALLER%
echo.
adb install "%AIR_INSTALLER%"
echo.
if errorlevel 1 goto failed
goto end

:failed
echo Troubleshooting:
echo - one, and only one, Android device should be connected
echo - verify 'bat\SetupSDK.bat'
echo.
goto end

:end
pause
