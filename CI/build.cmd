:: Local tweaks
::set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\
::set PATH=%PATH%;C:\Program Files (x86)\Git\bin\
::set PATH=%PATH%;C:\Program Files\7-Zip\
::cd ..

:: Reset bin files
git clean -f -x -d FlashDevelop\Bin\Debug

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Build the PluginCore
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform=x86

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Extract version from HEAD
call SetVersion.bat

:: Build the solution
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Create the installer
CI\nsis\makensis.exe FlashDevelop\Installer\Installer.nsi

:: Check for nsis errors
if %errorlevel% neq 0 goto :error

:: Create the archive
7z a -tzip FlashDevelop\Installer\Binary\FlashDevelop.zip .\FlashDevelop\Bin\Debug\*

:: Check for 7zip errors
if %errorlevel% neq 0 goto :error

:: Done
exit

:error
exit -1
