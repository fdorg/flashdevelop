:: Builds the binary on the server for CI

:: Set paths
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\
set PATH=%PATH%;C:\Program Files (x86)\Git\bin\
set PATH=%PATH%;C:\Program Files (x86)\NSIS
set PATH=%PATH%;C:\Program Files\7-Zip\

:flashdevelop

:: Extract version from HEAD
call SetVersion.bat

:: Build the main solution and run tests
msbuild FlashDevelop.sln /p:Configuration=Release+Tests /p:Platform="x86" /t:Rebuild %MSBuildLogger%

:: Check for build errors
if %errorlevel% neq 0 goto :error

if "%AppVeyorCI%" neq "" powershell.exe -file ci\tests.ps1

:: Check for build errors
if %errorlevel% neq 0 goto :error

:: Remove testing binaries so we can reuse the current build
del "FlashDevelop\Bin/Debug\*.Tests.*" /Q
del "FlashDevelop\Bin/Debug\NSubstitute.*" /Q
del "FlashDevelop\Bin/Debug\nunit.framework.*" /Q

:: Check if the build was triggered by a pull request
if "%APPVEYOR_PULL_REQUEST_NUMBER%" neq "" (
    :: Create the archive
    7z a -tzip FlashDevelop\Installer\Binary\FlashDevelopPR_%APPVEYOR_PULL_REQUEST_NUMBER%.zip .\FlashDevelop\Bin\Debug\* -xr!.empty
    exit 0
)

:: Build AnyCPU version for 64bits support
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform="Any CPU" /t:Build %MSBuildLogger%

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

:: Done
exit 0

:error
exit -1
