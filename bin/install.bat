@schtasks /create /f /sc onstart /rl highest /tn "XOutput" /tr "%~dp0XOutput.exe"
@schtasks /run /tn "XOutput"