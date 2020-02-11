<#
    .EXAMPLE
      .\ci.ps1 -version 3.23
      Set-ExecutionPolicy Bypass -Scope Process -Force; .\ci.ps1
#>

param (
    [Parameter(Mandatory=$true)][string]$version
)
$ErrorActionPreference = 'Stop';

# Configuration
$sonarCloudUser='csutorasa'
# $sonarCloudToken from global scope

cd $PSScriptRoot

# Pre .NET build
dotnet restore
dotnet tool install --global dotnet-sonarscanner
dotnet sonarscanner begin /k:"XOutput" /d:"sonar.host.url=https://sonarcloud.io" /o:$sonarCloudUser /d:"sonar.login=$sonarCloudToken" /d:sonar.cs.opencover.reportsPaths="*Tests\coverage.*.opencover.xml" /d:sonar.coverage.exclusions="**Tests*.cs,**/*Configuration.cs"

# .NET build
dotnet msbuild -p:Configuration=Release -p:Version=$version -p:AssemblyVersion=$version -p:FileVersion=$version
# dotnet build -c Release -p:Version=$version -p:AssemblyVersion=$version -p:FileVersion=$version

# .NET test
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet sonarscanner end /d:"sonar.login=$sonarCloudToken"

# npm build
npm install

# Creating package
$target='net452'
7z a XOutput.zip -- `
$PSScriptRoot\XOutput.Core\bin\Release\$target\XOutput.Core.dll `
$PSScriptRoot\XOutput.Core\bin\Release\$target\Newtonsoft.Json.dll `
$PSScriptRoot\XOutput.Core\bin\Release\$target\NLog.dll `
$PSScriptRoot\XOutput.Api\bin\Release\$target\XOutput.Api.dll `
$PSScriptRoot\XOutput.Server\bin\Release\$target\XOutput.Server.* `
$PSScriptRoot\XOutput.Server\bin\Release\$target\Nefarius.ViGEm.Client.dll `
$PSScriptRoot\XOutput\bin\Release\$target\XOutput.* `
$PSScriptRoot\XOutput\bin\Release\$target\SharpDX.dll `
$PSScriptRoot\XOutput\bin\Release\$target\SharpDX.DirectInput.dll `
$PSScriptRoot\XOutput\bin\Release\$target\Hardcodet.Wpf.TaskbarNotification.dll `
$PSScriptRoot\Web\webapp
