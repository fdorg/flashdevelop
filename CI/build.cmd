:: reset local files to original state
@if "%1" neq "full" goto build
del /S /Q FlashDevelop\Bin\Debug 
git reset HEAD --hard

:build
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform=x86
copy CI\files\AssemblyInfo.cs FlashDevelop\Properties\AssemblyInfo.cs
msbuild FlashDevelop\FlashDevelop.csproj /p:Configuration=Release /p:Platform=x86
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86

:: remove debug files
cd FlashDevelop\Bin\Debug
del /S *.vshost.exe
del /S *.vshost.exe.config
del /S *.vshost.exe.manifest

pause
