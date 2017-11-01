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
	cinst.exe haxe --version 3.4.3 -y
	$env:HAXEPATH = "c:\ProgramData\chocolatey\lib\haxe\"
	RefreshEnv
}

If ((Get-Command "nunit-console-x86.exe" -ErrorAction SilentlyContinue) -ne $null)
{
    $testFiles = [System.IO.Directory]::GetFiles("FlashDevelop\Bin\Debug", "*.Tests.dll")
    IF ($testFiles.Count -eq 0)
    {
        Write-Output "No test assemblies found"
    }
    ELSE
    {
        #Maybe in the future we want to set categories and priorities
        nunit-console-x86.exe $testFiles
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
