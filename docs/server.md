# Server

## Prerequisites

The 4.x version backend is built with .NET 7, you will need to [download it](https://dotnet.microsoft.com/download).

| Action                         | Required dependency  |
| ------------------------------ | -------------------- |
| Running the server application | ASP.NET Core Runtime |
| Building the application       | SDK                  |

However these runtimes and SDKs are cross platform the application is only compatible with Windows.

## Server application

The server application is responsible for:

- collecting input from various sources
- mapping these sources
- emulating devices
- configuring all the above

It is recommended to create a Windows task from the server,
so it can start at computer startup (with Administrator priviledges without UAC).
Help can be found in the [bin directory](./bin).

Server application might write the registry, therefore it needs administrator prividledges.
Alternatively it can be started without admin access, but then when it tries to write the registry it will prompt UAC.
