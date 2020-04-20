$exepath = Join-Path $PSScriptRoot "XOutput.App.exe"
schtasks.exe /create /f /sc onstart /rl highest /tn "XOutput" /tr $exepath
schtasks.exe /run /tn "XOutput"