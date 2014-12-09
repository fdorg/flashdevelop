:: Packages all themes from sources

:: Set 7-Zip path
set PATH=%PATH%;C:\Program Files\7-Zip\

:: Path up twice
cd ..
cd ..

:: Package full themes
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\DefaultTheme.fdz .\External\Themes\FullThemes\DefaultTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\DimGrayTheme.fdz .\External\Themes\FullThemes\DimGrayTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\ObsidianTheme.fdz .\External\Themes\FullThemes\ObsidianTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\FullThemes\ThyleusTheme.fdz .\External\Themes\FullThemes\ThyleusTheme\* -xr!.empty

:: Package syntax themes
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\DefaultTheme.fdz .\External\Themes\SyntaxThemes\DefaultTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\DimGrayTheme.fdz .\External\Themes\SyntaxThemes\DimGrayTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\ObsidianTheme.fdz .\External\Themes\SyntaxThemes\ObsidianTheme\* -xr!.empty
7z a -tzip .\FlashDevelop\Bin\Debug\Settings\Themes\SyntaxThemes\ThyleusTheme.fdz .\External\Themes\SyntaxThemes\ThyleusTheme\* -xr!.empty

:: Exit
exit