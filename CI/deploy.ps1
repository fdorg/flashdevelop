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

$fd = $false;
$hd = $false;
$login = $variables["SecureLogin"]
$pass = $variables["SecurePass"]

foreach($artifact in $artifacts.values)
{
    Write-Output "Upload artifact: $($artifact.name)"
    $ext = [System.IO.Path]::GetExtension($artifact.name)
    $name = [System.IO.Path]::GetFileNameWithoutExtension($artifact.name)
    IF ($name -match "FlashDevelop") { $fd = $true; }
    IF ($name -match "HaxeDevelop") { $hd = $true; }
    IF ($ext -eq ".xml")
    {
        # Upload appman.xml file
        ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "$name$ext";
    }
    ELSE
    {
        # Upload the installer and the zip archive
        ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$($artifact.path)" "downloads/builds/$name$ext";
    }
}

Write-Output "Create and upload build info."
$date = Get-Date
$file = [System.IO.Path]::GetTempFileName()
$data = "Build: $projectVersion`r`nTime: " + $date.ToUniversalTime() + " GMT"
$data | Set-Content $file
IF ($fd)
{
    ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$file" "downloads/builds/FlashDevelop-$env:APPVEYOR_REPO_BRANCH.txt";
}
IF ($hd)
{
    ncftpput.exe -u "$login" -p "$pass" -C ftp.flashdevelop.org "$file" "downloads/builds/HaxeDevelop-$env:APPVEYOR_REPO_BRANCH.txt";
}
