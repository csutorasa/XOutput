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
$target='netcoreapp5.0'
$guitarget='net5.0-windows'
7z a XOutput.zip -- `
$PSScriptRoot\XOutput.Core\bin\Release\$target\XOutput.Core.dll `
$PSScriptRoot\XOutput.Core\bin\Release\$target\NLog.dll `
$PSScriptRoot\XOutput.Api\bin\Release\$target\XOutput.Api.dll `
$PSScriptRoot\XOutput.Server\bin\Release\$target\XOutput.Server.dll `
$PSScriptRoot\XOutput.Server\bin\Release\$target\XOutput.Server.exe `
$PSScriptRoot\XOutput.Emulation\bin\Release\$target\XOutput.Emulation.dll `
$PSScriptRoot\XOutput.Emulation\bin\Release\$target\Nefarius.ViGEm.Client.dll `
$PSScriptRoot\XOutput.Mapping\bin\Release\$target\XOutput.Mapping.dll `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\XOutput.App.dll `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\XOutput.App.exe `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\XOutput.App.runtimeconfig.json `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\SharpDX.dll `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\SharpDX.DirectInput.dll `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\Hardcodet.Wpf.TaskbarNotification.dll `
$PSScriptRoot\XOutput.App\bin\Release\$guitarget\HIDSharp.dll `
$PSScriptRoot\Web\webapp `
$PSScriptRoot\bin\*
