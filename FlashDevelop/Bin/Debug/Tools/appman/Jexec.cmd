: Init

@echo off

: Set vars

if exist "..\..\.local" set dir=%cd%\Archive
if not exist "..\..\.local" set dir=%LOCALAPPDATA%\FlashDevelop\Data\AppMan\Archive
set file=

: Get file

for /r %dir% %%i in (*%1) do set file=%%i

: Shift params

shift

: Execute jar

java -jar %file% %1 %2 %3 %4 %5 %6 %7 %8 %9
