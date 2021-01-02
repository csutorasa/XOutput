# Bin

For PowerShell scripts you might need to set the execution policy.
To enable running powershell script from the current process use:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force
```

## install

This registers a task, which upon logging into Windows, will start the Server application as administrator.

*This needs to be run as administrator.*

## uninstall

This unregisters the installed task.

*This needs to be run as administrator.*

## error-log

Reads the Windows logs and writes it to `Windows EventLog errors.txt`.
It makes tracing crashes easier.

*This needs to be run as administrator.*
