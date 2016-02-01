:: Builds the binary locally and launches the installer

:: Set paths
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\
set PATH=%PATH%;C:\Program Files\Git\bin\
set PATH=%PATH%;C:\Program Files (x86)\Git\bin\
set PATH=%PATH%;C:\Program Files\7-Zip\
set PATH=%PATH%;C:\Program Files (x86)\7-Zip\
set PATH=%PATH%;C:\Program Files (x86)\NSIS

:: Need path up
cd ..

:flashdevelop

:: Reset bin files
git clean -f -x -d FlashDevelop\Bin\Debug
del FlashDevelop\Installer\Binary\*.exe /Q
del FlashDevelop\Installer\Binary\*.zip /Q

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Build the PluginCore
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform="AnyCPU" /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Extract version from HEAD
call SetVersion.bat

:: Build the solutions
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86 /t:Rebuild
ping -n 5 127.0.0.1 > nul
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Create the installer
makensis FlashDevelop\Installer\Installer.nsi

:: Check for nsis errors
if %errorlevel% neq 0 goto :error

:: Create the archive
7z a -tzip FlashDevelop\Installer\Binary\FlashDevelop.zip .\FlashDevelop\Bin\Debug\* -xr!.empty

:: Check for 7zip errors
if %errorlevel% neq 0 goto :error

:haxedevelop

:: Reset bin files
git clean -f -x -d FlashDevelop\Bin\Debug

:: Remove bad files
del FlashDevelop\Bin\Debug\FlashDevelop.exe.config
del FlashDevelop\Bin\Debug\FlashDevelopx64.exe.config
del FlashDevelop\Bin\Debug\StartPage\images\*.* /Q
for /d %%G in ("FlashDevelop\Bin\Debug\Projects\*ActionScript 3*") do rd /s /q "%%~G"

:: Copy distro files
xcopy Distros\HaxeDevelop /s /e /y

:: Build the PluginCore
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform="AnyCPU" /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Extract version from HEAD
call SetVersion.bat

:: Build the solutions
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86 /t:Rebuild
ping -n 5 127.0.0.1 > nul
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Create the installer
makensis FlashDevelop\Installer\Installer.nsi

:: Check for nsis errors
if %errorlevel% neq 0 goto :error

:: Create the archive
7z a -tzip FlashDevelop\Installer\Binary\HaxeDevelop.zip .\FlashDevelop\Bin\Debug\* -xr!.empty

: finish

:: Done, Run FD
start FlashDevelop\Installer\Binary\FlashDevelop.exe
exit

:error
pause
exit -1
