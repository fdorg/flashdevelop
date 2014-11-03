@echo off
if %1 == debug (
	bin\Main-debug.exe
) else (
	bin\Main.exe
)
pause