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
	Set-Service wuauserv -StartupType Manual
	cinst.exe haxe --version 4.0.5 -y --no-progress
	$env:HAXEPATH = "c:\ProgramData\chocolatey\lib\haxe\"
	RefreshEnv
}

If ((Get-Command "nunit3-console.exe" -ErrorAction SilentlyContinue) -ne $null)
{
    cd "FlashDevelop\Bin\Debug"
    $testFiles = [System.IO.Directory]::GetFiles(".", "*.Tests.dll")
    IF ($testFiles.Count -eq 0)
    {
        Write-Output "No test assemblies found"
    }
    ELSE
    {
        nunit3-console.exe $testFiles --result=myresults.xml;format=AppVeyor
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
}
ELSE
{
    Write-Output "NUnit runner not found"
}
