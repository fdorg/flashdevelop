:: Packages the selected theme from sources
@echo off

:: Set 7-Zip path
set PATH=%PATH%;C:\Program Files\7-Zip\

:: Path up twice
cd ..
cd ..

:start
echo [1] Default
echo [2] DimGray
echo [3] Obsidian
echo [4] Thyleus
set /p option="Which theme? "
if "%option%" == "1" goto :option1
if "%option%" == "2" goto :option2
if "%option%" == "3" goto :option3
if "%option%" == "4" goto :option4
echo Invalid option, try again.
goto :start

:option1
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\DefaultTheme.fdz .\External\Themes\FullThemes\DefaultTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\DefaultTheme.fdz .\External\Themes\SyntaxThemes\DefaultTheme\* -xr!.empty
exit

:option2
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\DimGrayTheme.fdz .\External\Themes\FullThemes\DimGrayTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\DimGrayTheme.fdz .\External\Themes\SyntaxThemes\DimGrayTheme\* -xr!.empty
exit

:option2
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\ObsidianTheme.fdz .\External\Themes\FullThemes\ObsidianTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\ObsidianTheme.fdz .\External\Themes\SyntaxThemes\ObsidianTheme\* -xr!.empty
exit

:option4
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\ThyleusTheme.fdz .\External\Themes\FullThemes\ThyleusTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\ThyleusTheme.fdz .\External\Themes\SyntaxThemes\ThyleusTheme\* -xr!.empty
exit
