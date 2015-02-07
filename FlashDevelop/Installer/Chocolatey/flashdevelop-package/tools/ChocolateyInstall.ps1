$packageName = 'flashdevelop'
$installerType = 'EXE'
$32BitUrl  = 'http://www.flashdevelop.org/downloads/releases/FlashDevelop-4.7.0.exe'
$silentArgs = '/S'
$validExitCodes = @(0)

Install-ChocolateyPackage "$packageName" "$installerType" "$silentArgs" "$32BitUrl" -validExitCodes $validExitCodes