: Init. Echo off to suppress detailed info

@echo off
setlocal enabledelayedexpansion
set params=

: List js files under src/js for GCC with --js

for /r "src/js" %%i in (*.js) do set params=!params! --js %%i

: Call GCC with provided params, add js files

java -jar %*%params%
