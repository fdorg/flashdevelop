:: Builds the binary locally and launches the installer

:: Set paths
set PATH=%PATH%;C:\Program Files (x86)\MSBuild\14.0\Bin\
set PATH=%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin
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
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild
ping -n 5 127.0.0.1 > nul
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86 /t:Rebuild

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

:: Remove unnecessary files
rd "FlashDevelop\Bin\Debug\Tools\flexpmd" /s /q
rd "FlashDevelop\Bin\Debug\Tools\flexlibs\frameworks\libs\player" /s /q
for /d %%G in ("FlashDevelop\Bin\Debug\Projects\*ActionScript 3*") do rd /s /q "%%~G"
del "FlashDevelop\Bin\Debug\StartPage\images\*.*" /q

:: Copy distro files
xcopy Distros\HaxeDevelop /s /e /y

:: Build the PluginCore
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform="AnyCPU" /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Extract version from HEAD
call SetVersion.bat

:: Build the solutions
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild
ping -n 5 127.0.0.1 > nul
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86 /t:Rebuild

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Rename binaries
ren FlashDevelop\Bin\Debug\FlashDevelop.exe HaxeDevelop.exe
ren FlashDevelop\Bin\Debug\FlashDevelop64.exe HaxeDevelop64.exe
ren FlashDevelop\Bin\Debug\FlashDevelop.exe.config HaxeDevelop.exe.config
ren FlashDevelop\Bin\Debug\FlashDevelop64.exe.config HaxeDevelop64.exe.config

: Remove files after build
del "FlashDevelop\Bin\Debug\Plugins\CodeAnalyzer.dll" /q

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Create the installer
makensis FlashDevelop\Installer\Installer.nsi

:: Check for nsis errors
if %errorlevel% neq 0 goto :error

:: Create the archive
7z a -tzip FlashDevelop\Installer\Binary\HaxeDevelop.zip .\FlashDevelop\Bin\Debug\* -xr!.empty

:finish

:: Revert distro changes with backup
git stash save "Local CI Backup..."

:: Done, Run FD
start FlashDevelop\Installer\Binary\FlashDevelop.exe
exit 0

:error
pause
exit -1
