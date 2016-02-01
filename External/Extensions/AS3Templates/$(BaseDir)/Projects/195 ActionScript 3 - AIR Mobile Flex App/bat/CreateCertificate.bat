@echo off

:: Set working dir
cd %~dp0 & cd ..

set PAUSE_ERRORS=1
call bat\SetupSDK.bat
call bat\SetupApp.bat

:: Generate
echo.
echo Generating a self-signed certificate for Android packaging
call adt -certificate -validityPeriod 25 -cn %AND_CERT_NAME% 2048-RSA "%AND_CERT_FILE%" %AND_CERT_PASS%
if errorlevel 1 goto failed

:succeed
echo.
echo Certificate created: %AND_CERT_FILE% with password "%AND_CERT_PASS%"
echo.
if "%AND_CERT_PASS%" == "fd" echo Note: You did not change the default password
echo.
echo HINTS: 
echo - you only need to generate this certificate once,
echo - wait a minute before using this certificate to package your AIR application.
echo.
goto end

:failed
echo.
echo Certificate creation FAILED.
echo.

:end
pause
