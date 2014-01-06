:: Init

@echo off
setlocal enabledelayedexpansion

:: Check params

if /i [%1]==[] goto help
if /i [%1]==[-help] goto help
if /i [%2]==[] goto help

:: Set vars

if exist "..\..\.local" set dir=%cd%\Apps
if not exist "..\..\.local" set dir=%LOCALAPPDATA%\FlashDevelop\Apps
set file=
set args=

:: Get file

for /r %dir%\%1 %%i in (*%2) do set file=%%i

:: Remove id and jar

set args=%*
call set args=%%args:%1=%%
call set args=%%args:%2=%%

:: Execute jar

java -jar %file% %args%
goto:eof

:help

echo Jexec 1.0 - AppMan Java Jar execution helper.
echo Usage: Jexec.cmd app_id file_name [?options]
