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
    #Write-Output "Type: $($artifact.type)"
    #Write-Output "Path: $($artifact.path)"
    #Write-Output "Url: $($artifact.url)"
    
    CI\ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "downloads/builds/$($artifact.name)-$projectVersion.zip"
}
