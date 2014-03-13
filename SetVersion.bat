@echo off
cd "%1"
"External\Tools\SetVersion\SetVersion.exe" "FlashDevelop\Properties\AssemblyInfo.cs" "%APPVEYOR_BUILD_NUMBER%"

