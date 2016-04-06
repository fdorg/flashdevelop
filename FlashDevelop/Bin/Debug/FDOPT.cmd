@echo off
echo FlashDevelop OPtimizer Tool - v1.0.0
echo.
echo ! Make sure you have admin rights or run this as an admin !
echo.

:: Set paths
cd %~dp0%
set CDIR=%~dp0%
set PDIR=%~dp0%Plugins
set NGEN=%WINDIR%\Microsoft.NET\Framework\v2.0.50727\ngen.exe

if "%1" == "install" goto :install
if "%1" == "uninstall" goto :uninstall

:menu
echo *** Available tasks ***
echo.
echo  [1] Optimize with NGEN
echo  [2] Remove NGEN optimizations
echo  [3] Exit, I'm done.
echo.
set /p option="What do you want me to do? "
if "%option%" == "1" goto :install
if "%option%" == "2" goto :uninstall
if "%option%" == "3" goto :end
echo Invalid option, try again.
echo.
goto :menu

:install

echo Optimizing with NGEN...
if exist "%CDIR%HaxeDevelop.exe" "%NGEN%" install "%CDIR%HaxeDevelop.exe" /AppBase:"%CDIR%\"
if exist "%CDIR%FlashDevelop.exe" "%NGEN%" install "%CDIR%FlashDevelop.exe" /AppBase:"%CDIR%\"
"%NGEN%" install "%CDIR%Aga.dll" /AppBase:"%CDIR%\"
"%NGEN%" install "%CDIR%SwfOp.dll" /AppBase:"%CDIR%\"
"%NGEN%" install "%CDIR%Antlr3.dll" /AppBase:"%CDIR%\"
"%NGEN%" install "%CDIR%Scripting.dll" /AppBase:"%CDIR%\"
"%NGEN%" install "%CDIR%Tools\fdbuild\fdbuild.exe" /AppBase:"%CDIR%Tools\fdbuild"
"%NGEN%" install "%CDIR%Tools\asdocgen\ASDocGen.exe" /AppBase:"%CDIR%Tools\asdocgen"
"%NGEN%" install "%CDIR%Tools\appman\AppMan.exe" /AppBase:"%CDIR%Tools\appman"
:: Install plugins?
:: for %%G in ("%PDIR%\*.dll") do "%NGEN%" install "%%G" /AppBase:"%CDIR%\"
goto :end

:uninstall

echo Removing NGEN optimizations...
if exist "%CDIR%HaxeDevelop.exe" "%NGEN%" uninstall "%CDIR%HaxeDevelop.exe"
if exist "%CDIR%FlashDevelop.exe" "%NGEN%" uninstall "%CDIR%FlashDevelop.exe"
"%NGEN%" uninstall "%CDIR%Aga.dll"
"%NGEN%" uninstall "%CDIR%SwfOp.dll"
"%NGEN%" uninstall "%CDIR%Antlr3.dll"
"%NGEN%" uninstall "%CDIR%Scripting.dll"
"%NGEN%" uninstall "%CDIR%Tools\fdbuild\fdbuild.exe"
"%NGEN%" uninstall "%CDIR%Tools\asdocgen\ASDocGen.exe"
"%NGEN%" uninstall "%CDIR%Tools\appman\AppMan.exe"
:: Uninstall plugins?
:: for %%G in ("%PDIR%\*.dll") do "%NGEN%" uninstall "%%G"
goto :end

:end
echo Done.
exit 0
