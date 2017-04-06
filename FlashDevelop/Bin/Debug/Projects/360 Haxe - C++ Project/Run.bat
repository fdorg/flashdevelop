@echo off
cd bin
if "%1"=="debug" (
  :: run debug
  Main-Debug.exe
) else (
  :: run release
  Main.exe
)
pause