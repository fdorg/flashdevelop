:: Script for installing OpenFL and it's dependencies.
@echo off

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
goto :done

:install_openfl

echo Installing OpenFL...

haxelib install openfl
haxelib run openfl setup

:: Check for errors
if %errorlevel% neq 0 goto :install_error

goto :done

:haxelib_error

echo Haxe is not installed. Please install Haxe first or if it's installed, restart Windows before continuing.
pause
exit -1

:install_error

echo Could not install required Haxelib library: openfl.
pause
exit -1

:done

echo OpenFL installed successfully.
pause
exit 0
