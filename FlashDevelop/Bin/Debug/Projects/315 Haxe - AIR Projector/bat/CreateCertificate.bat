@echo off
cd..
set PAUSE_ERRORS=1
call bat\SetupSDK.bat
call bat\SetupApplication.bat

:: Generate
echo.
echo Generating a self-signed certificate...
call adt -certificate -cn %CERT_NAME% 1024-RSA %CERT_FILE% %CERT_PASS%
if errorlevel 1 goto failed

:succeed
echo.
echo Certificate created: %CERT_FILE% with password "%CERT_PASS%"
echo.
if "%CERT_PASS%" == "fd" echo (note: you did not change the default password)
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