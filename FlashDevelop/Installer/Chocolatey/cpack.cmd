:: Package
@call chocolatey pack flashdevelop-package\flashdevelop.nuspec 
@call chocolatey pack flashdevelop.dev-package\flashdevelop.dev.nuspec 

:: Test
:: cinst flashdevelop.dev -source %cd%