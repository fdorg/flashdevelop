:: reset local files to original state
@if "%1" neq "full" goto build
del /S /Q FlashDevelop\Bin\Debug 
git reset HEAD --hard

:build
msbuild PluginCore\PluginCore.csproj /p:Configuration=Debug /p:Platform=x86
call SetVersion
msbuild FlashDevelop.sln /p:Configuration=Debug /p:Platform=x86

:: remove debug files
cd FlashDevelop\Bin\Debug
del .multi
del /S *.pdb
