<#
    .EXAMPLE
      .\version-upgrade.ps1
      .\version-upgrade.ps1 -version 4.0.0
      Set-ExecutionPolicy Bypass -Scope Process -Force; .\version-upgrade.ps1
#>

param (
    [Parameter(Mandatory=$true)][string]$version
)
$ErrorActionPreference = 'Stop';
Get-Content .\.github\workflows\build.yml | `
ForEach-Object {$_ -replace "  VERSION: .*","  VERSION: $version"} | `
Out-File -Encoding utf8 -FilePath .\.github\workflows\build.yml

[IO.File]::WriteAllLines("$pwd\appveyor.yml", ` 
Get-Content .\appveyor.yml | `
ForEach-Object {$_ -replace "version: .*-{branch}-{build}","version: $version-{branch}-{build}"} | `
ForEach-Object {$_ -replace "versionnumber: .*","versionnumber: $version"} )
