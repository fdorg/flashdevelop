: Init

@echo off
setlocal enabledelayedexpansion

: Set vars

if exist "..\..\.local" set dir=%cd%\Archive
if not exist "..\..\.local" set dir=%LOCALAPPDATA%\FlashDevelop\Data\AppMan\Archive
set file=
set args=

: Get file

for /r %dir%\%1 %%i in (*%2) do set file=%%i

: Remove id and jar

set args=%*
call set args=%%args:%1=%%
call set args=%%args:%2=%%

: Execute jar

java -jar %file% %args%
