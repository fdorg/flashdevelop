:: Additionals for cmd only build
:: set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\
:: cd ..

:: reset local files to original state
@if "%1" neq "full" goto build
del /S /Q FlashDevelop\Bin\Debug 
git reset HEAD --hard

:build
msbuild PluginCore\PluginCore.csproj /p:Configuration=Release /p:Platform=x86
call SetVersion
msbuild FlashDevelop.sln /p:Configuration=Release /p:Platform=x86

