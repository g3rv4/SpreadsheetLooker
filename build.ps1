param (
    [Parameter(Mandatory=$true)][int]$BuildNumber,
    [Parameter(Mandatory=$false)][string]$CommitSHA
)

$basePath = Get-Location
$csProjPath = Join-Path $basePath SpreadsheetLooker.Web/SpreadsheetLooker.Web.csproj
$buildPath = Join-Path $basePath bin/build

[xml]$xmlDoc = Get-Content $csProjPath
$versionElement = $xmlDoc['Project']['PropertyGroup']['Version']
$version = [version]$versionElement.InnerText
$newVersion = "$($version.Major).$($version.Minor).$($BuildNumber)"

$versionWithoutHash = $newVersion
if ($CommitSHA) {
    $newVersion = "$($newVersion)+$($CommitSHA.SubString(0, 7))"
}

$versionElement.InnerText = $newVersion
$xmlDoc.Save($csProjPath)

if (Test-Path $buildPath -PathType Container) {
    rm -rf $buildPath
}

$uid = sh -c 'id -u'
$gid = sh -c 'id -g'

docker run --rm -v "$($basePath):/var/src" mcr.microsoft.com/dotnet/sdk:5.0.401-alpine3.14 ash -c "dotnet publish -c Release /var/src/SpreadsheetLooker.Web/SpreadsheetLooker.Web.csproj -o /var/src/bin/build && chown -R $($uid):$($gid) /var/src"

$nuspecPath = Join-Path $buildPath spreadhseetlooker.web.nuspec
$nupkgPath = Join-Path $buildPath "spreadhseetlooker.web.$($newVersion).nupkg"
cp spreadhseetlooker.web.nuspec $nuspecPath

[xml]$xmlDoc = Get-Content $nuspecPath
$xmlDoc['package']['metadata']['version'].InnerText = $versionWithoutHash
$xmlDoc.Save($nuspecPath)

Compress-Archive -Path "$($buildPath)/*" -DestinationPath $nupkgPath

if ($env:GITHUB_ENV) {
    Write-Output "VERSION_WITHOUT_HASH=$versionWithoutHash" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
    Write-Output "VERSION=$newVersion" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
    Write-Output "PKG_PATH=$nupkgPath" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
}