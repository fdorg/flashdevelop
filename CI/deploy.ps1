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
    ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "downloads/builds/$name-$projectBuildNumber$ext";
}
