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
    ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "downloads/builds/$($artifact.name)"
}
