<#
    .EXAMPLE
      .\version-upgrade.ps1
      .\version-upgrade.ps1 -version 3.30
      Set-ExecutionPolicy Bypass -Scope Process -Force; .\version-upgrade.ps1
#>

param (
    [Parameter(Mandatory=$true)][string]$version
)
$ErrorActionPreference = 'Stop';
$new_content = Get-Content .\XOutput\UpdateChecker\Version.cs | ForEach-Object {$_ -replace "public const string AppVersion = `".*`";","public const string AppVersion = `"$version`";"}
$new_content | Out-File -Encoding utf8 -FilePath .\XOutput\UpdateChecker\Version.cs

Get-Content .\.github\workflows\build3x.yml | `
ForEach-Object {$_ -replace "  VERSION: .*","  VERSION: $version"} | `
Out-File -Encoding utf8 -FilePath .\.github\workflows\build.yml

[IO.File]::WriteAllLines("$pwd\appveyor.yml", ` 
Get-Content .\appveyor.yml | `
ForEach-Object {$_ -replace "version: .*-{branch}-{build}","version: $version-{branch}-{build}"} | `
ForEach-Object {$_ -replace "versionnumber: .*","versionnumber: $version"} )

$new_content = Get-Content .\xoutput.nuspec | ForEach-Object {$_ -replace "<version>.*</version>","<version>$version</version>"}
[IO.File]::WriteAllLines("$pwd\xoutput.nuspec", $new_content)
