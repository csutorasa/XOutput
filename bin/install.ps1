$exepath = Join-Path $PSScriptRoot "XOutput.Server.exe"
schtasks.exe /create /f /sc onstart /rl highest /tn "XOutput" /tr $exepath
schtasks.exe /run /tn "XOutput"