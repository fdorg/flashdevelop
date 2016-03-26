@echo off
cd /d %~dp0
echo FlashDevelop Maintenance Tool - v1.0.0
echo.
echo ! Make sure you have admin rights or run this as an admin !
echo.

:menu
echo *** Available tasks ***
echo.
echo  [1] Toggle standalone mode
echo  [2] Toggle multi instance mode
echo  [3] Exit, I'm done.
echo.
set /p option="What do you want me to do? "
if "%option%" == "1" goto :local
if "%option%" == "2" goto :multi
if "%option%" == "3" goto :end
echo Invalid option, try again.
echo.
goto :menu

:local
echo.
if exist .local (
	echo Deleting .local, restart FlashDevelop.
	del .local
) else (
	echo Creating .local, restart FlashDevelop.
    echo. > .local
)
echo.
goto :menu

:multi
echo.
if exist .multi (
	echo Deleting .multi, restart FlashDevelop.
	del .multi
) else (
	echo Creating .multi, restart FlashDevelop.
	echo. > .multi
)
echo.
goto :menu

:end
exit 0
