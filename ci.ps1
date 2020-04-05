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
$sonarCloudToken=$env:sonarcloudtoken

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

# Creating package
$target='netcoreapp3.1'
7z a XOutput.zip -- `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.Core.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.Api.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.Server.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.Devices.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.App.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\XOutput.App.exe `
$PSScriptRoot\XOutput.App\bin\Release\$target\Newtonsoft.Json.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\NLog.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\Nefarius.ViGEm.Client.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\SharpDX.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\SharpDX.DirectInput.dll `
$PSScriptRoot\XOutput.App\bin\Release\$target\Hardcodet.Wpf.TaskbarNotification.dll `
$PSScriptRoot\Web\webapp
