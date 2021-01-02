@schtasks /create /f /sc onstart /rl highest /tn "XOutput" /tr "%~dp0XOutput.Server.exe"
@schtasks /run /tn "XOutput"