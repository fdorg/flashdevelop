# Runs the unit tests, and uploads them to the CI server

Param (
    $variables = @{},   
    $scriptPath,
    $buildFolder,
    $srcFolder,
    $outFolder,
    $tempFolder,
    $projectName,
    $projectVersion,
    $projectBuildNumber
)

If ($env:HAXEPATH -eq $null)
{
	# https://help.appveyor.com/discussions/problems/5616-not-able-to-build-due-to-problem-in-chocolateyinstallps1
	#Set-Service wuauserv -StartupType Manual
	cinst.exe haxe --version 4.1.4 -y --no-progress
	#$env:HAXEPATH = "c:\ProgramData\chocolatey\lib\haxe\"
	#RefreshEnv

    Write-Output "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++"
    Write-Output $env:HAXEPATH
    haxe --version
}

If ((Get-Command "nunit3-console.exe" -ErrorAction SilentlyContinue) -ne $null)
{
    $path = [System.IO.Directory]::GetCurrentDirectory() + "\FlashDevelop\Bin\Debug"
    $testFiles = [System.IO.Directory]::GetFiles($path, "*.Tests.dll")
    IF ($testFiles.Count -eq 0)
    {
        Write-Output "No test assemblies found"
        exit 1
    }

    cd $path
    #nunit3-console.exe $testFiles --result=myresults.xml;format=AppVeyor
    nunit3-console.exe $testFiles --x86
    #nunit3-console.exe $testFiles

    #It turns out it's not needed to upload the file
    #if ((Test-Path env:\APPVEYOR_JOB_ID) -And (Test-Path TestResult.xml))
    #{
    #    $wc = New-Object 'System.Net.WebClient'
    #    $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path .\TestResult.xml))
    #}
        
    if ($LASTEXITCODE -ne 0)
    {
        exit 1
    }
}
ELSE
{
    Write-Output "NUnit runner not found"
    exit 1
}