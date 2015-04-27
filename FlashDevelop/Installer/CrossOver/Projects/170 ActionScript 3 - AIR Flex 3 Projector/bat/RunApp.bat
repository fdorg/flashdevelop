@echo off

:: Set working dir
cd %~dp0 & cd ..

set PAUSE_ERRORS=1
call bat\SetupSDK.bat
call bat\SetupApp.bat

echo.
echo Starting AIR Debug Launcher...
echo.

:: Native exec tool
set FDEXE=%FDBRIDGE%/FDEXE.sh

:: Run webserver if needed, and wait the server to start and prefix out file
if [%USE_SERVER%] == [true] (
	start /B "%PROGRAMFILES%\FlashDevelop\Tools\webserver\server.exe" -document_root "%CD%" -listening_port 8080
	set OUT_FILE=http://localhost:8080/%OUT_FILE%
	ping -n 2 127.0.0.1>nul
)

:: Handle selected target
if [%1] == [unix_adl] (
	echo Running Mac/Linux ADL...
	%FDEXE% "%UNIX_FLEX_SDK%/bin/adl" -profile extendedDesktop %APP_XML:\=/% %APP_DIR:\=/%
) else if [%1] == [win_adl] (
	echo Running Windows ADL...
	adl.exe -profile extendedDesktop "%APP_XML%" "%APP_DIR%"
) else if [%1] == [unix_player] (
	echo Running Mac/Linux Flash Player...
	%FDEXE% %OUT_FILE%
) else if [%1] == [win_player] (
	echo Running Windows Flash Player...
	start %OUT_FILE%
) else if [%1] == [unix_browser] (
	echo Running Mac/Linux Browser...
	:: Mac and Linux commands, select correct:
	%FDEXE% open -a Safari %OUT_FILE%
	:: %FDEXE% xdg-open %OUT_FILE%
) else if [%1] == [win_ie] (
	echo Running Windows IE...
	start iexplore %OUT_FILE%
)
if errorlevel 1 goto error
goto end

:error
pause

:end
