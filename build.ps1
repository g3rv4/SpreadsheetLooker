$basePath = Get-Location
$publishPath = Join-Path $basePath .publish
$packagesPath = Join-Path $basePath .packages

if (Test-Path $publishPath -PathType Container) {
    rm -rf $publishPath
}
if (Test-Path $packagesPath -PathType Container) {
    rm -rf $packagesPath
} else {
    New-Item -Path $packagesPath -ItemType "directory"
}

$uid = sh -c 'id -u'
$gid = sh -c 'id -g'

docker run --rm -v "$($basePath):/var/src" -v "$($publishPath):/var/publish" mcr.microsoft.com/dotnet/sdk:6.0.101-alpine3.14 ash -c "dotnet publish -c Release /var/src/SpreadsheetLooker.Web/SpreadsheetLooker.Web.csproj -o /var/publish && chown -R $($uid):$($gid) /var/publish"

$version = [version](Get-Item "$publishPath/SpreadsheetLooker.Web.dll").VersionInfo.FileVersion
$version = "$($version.Major).$($version.Minor).$($version.Build)"

Write-Output "Version is $version"

$nuspecPath = Join-Path $publishPath spreadsheetlooker.web.nuspec
(Get-Content spreadsheetlooker.web.nuspec).Replace('$version$', $version) | Set-Content $nuspecPath

$nupkgPath = Join-Path $packagesPath "spreadsheetlooker.web.$($version).nupkg"
Compress-Archive -Path "$($publishPath)/*" -DestinationPath $nupkgPath

if ($env:GITHUB_ENV) {
    Write-Output "VERSION=$version" | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
}