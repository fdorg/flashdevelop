:: Builds .NET 2.0 binary

:: Set paths
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\

:: Build the solution
msbuild AppMan.sln /p:Configuration=Release /p:Platform=x86 /t:Rebuild /toolsversion:2.0

:: Done
pause
exit