param (
    [Parameter(Mandatory=$true)][string]$Version,
    [Parameter(Mandatory=$true)][string]$Apikey
)

$ErrorActionPreference = 'Stop';
choco uninstall xoutput -fy
choco pack
choco install xoutput -fdvy -s $pwd
choco apikey --key $apikey --source https://push.chocolatey.org/
choco push xoutput.$version.nupkg --source https://push.chocolatey.org/
