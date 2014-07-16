:: Package
@chocolatey pack flashdevelop-package\flashdevelop.nuspec 
@chocolatey pack flashdevelop.dev-package\flashdevelop.dev.nuspec 

:: Test
:: cinst flashdevelop.dev -source %cd%