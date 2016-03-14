@echo off

:: Script for installing HaxeFlixel and it's dependencies.
:: Path refresh adapted from: https://github.com/chocolatey/chocolatey/blob/master/src/redirects/RefreshEnv.cmd

:: Skip PATH refresh on Wine/CrossOver
if "%WINEPREFIX%"=="" (goto :refresh_path) else (goto :check_haxelib)

:setfromreg

"%WinDir%\System32\Reg" QUERY "%~1" /v "%~2" > "%TEMP%\_envset.tmp" 2>NUL
for /f "usebackq skip=2 tokens=2,*" %%A IN ("%TEMP%\_envset.tmp") do (
	echo/set %~3=%%B
)
goto :EOF

:getregenv

"%WinDir%\System32\Reg" QUERY "%~1" > "%TEMP%\_envget.tmp"
for /f "usebackq skip=2" %%A IN ("%TEMP%\_envget.tmp") do (
	if /I not "%%~A"=="Path" (
		call :setfromreg "%~1" "%%~A" "%%~A"
	)
)
goto :EOF

:refresh_path

echo Refreshing PATH from registry...
echo/@echo off >"%TEMP%\_env.cmd"

:: Slowly generating final file
call :getregenv "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" >> "%TEMP%\_env.cmd"
call :getregenv "HKCU\Environment">>"%TEMP%\_env.cmd" >> "%TEMP%\_env.cmd"

:: Special handling for PATH - mix both User and System
call :setfromreg "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" Path Path_HKLM >> "%TEMP%\_env.cmd"
call :setfromreg "HKCU\Environment" Path Path_HKCU >> "%TEMP%\_env.cmd"

:: Caution: do not insert space-chars before >> redirection sign
echo/set Path=%%Path_HKLM%%;%%Path_HKCU%% >> "%TEMP%\_env.cmd"

:: Cleanup
del /f /q "%TEMP%\_envset.tmp" 2>nul
del /f /q "%TEMP%\_envget.tmp" 2>nul

:: Set these variables
call "%TEMP%\_env.cmd"

echo OK
goto :check_haxelib

:check_haxelib

echo Checking Haxelib...

haxelib help > nul

:: Check for errors
if %errorlevel% neq 0 goto :haxelib_error

echo OK
goto :check_openfl

:check_openfl

echo Checking OpenFL...

haxelib path openfl > nul

:: Check for errors
if %errorlevel% neq 0 goto :install_openfl

echo OK
goto :check_flixel

:install_openfl

echo Installing OpenFL...

haxelib install openfl
haxelib run openfl setup

:: Check for errors
if %errorlevel% neq 0 goto :install_error

goto :options_vs

:options_vs

set /p option="Also install free Visual Studio for C++ Windows applications? [y/n] "
if "%option%" == "y" goto :install_vs
if "%option%" == "n" goto :check_flixel
echo Invalid value, try again.

goto :options_vs

:install_vs

lime setup windows

:: Check for errors
if %errorlevel% neq 0 goto :install_error

goto :check_flixel

:check_flixel

echo Checking HaxeFlixel...

haxelib path flixel > nul

:: Check for errors
if %errorlevel% neq 0 goto :install_flixel

echo OK
goto :done

:install_flixel

echo Installing HaxeFlixel...

haxelib install flixel

:: Check for errors
if %errorlevel% neq 0 goto :install_error

goto :done

:haxelib_error

echo Haxe seems not to be installed. Please install Haxe first or if it's installed, restart Windows before continuing.
pause
exit -1

:install_error

echo Could not install the requested tools.
pause
exit -1

:done

echo OpenFL + HaxeFlixel installed successfully.
pause
exit 0
