: Init

@echo off
setlocal enabledelayedexpansion

: Set vars

if exist "..\..\.local" set dir=%cd%\Archive
if not exist "..\..\.local" set dir=%LOCALAPPDATA%\FlashDevelop\Data\AppMan\Archive
set paths=

: Get paths

for /d %%i IN (*.*) do for /d %%j IN (%%i\*.*) do set paths=!paths!%dir%\%%j\;

: Add to PATH

set PATH=%PATH%;%paths%

: Execute

start "Start" /b %*
