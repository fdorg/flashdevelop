# Deploys the built binaries to the FD.org server

Param (
    $variables = @{},   
    $artifacts = @{},
    $scriptPath,
    $buildFolder,
    $srcFolder,
    $outFolder,
    $tempFolder,
    $projectName,
    $projectVersion,
    $projectBuildNumber
)

$login = $variables["SecureLogin"]
$pass = $variables["SecurePass"]

foreach($artifact in $artifacts.values)
{
    Write-Output "Upload artifact: $($artifact.name)"
    $ext = [System.IO.Path]::GetExtension($artifact.name)
    $name = [System.IO.Path]::GetFileNameWithoutExtension($artifact.name)
    ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "downloads/builds/$name$ext";
}

Write-Output "Create and upload LATEST_BUILD.txt"
$date = Get-Date
$file = [System.IO.Path]::GetTempFileName()
$data = "Build Number: $projectBuildNumber`r`nTime: $date"
$data | Set-Content $file
ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$file" "downloads/builds/LATEST_BUILD.txt";
