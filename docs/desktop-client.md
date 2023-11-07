# Desktop client

## Prerequisites

The 4.x version client is built with .NET 7, you will need to [download it](https://dotnet.microsoft.com/download).

| Action                         | Required dependency  |
| ------------------------------ | -------------------- |
| Running the reader application | .NET Desktop Runtime |
| Building the application       | SDK                  |

The SDK is only compatible with Windows.

```shell
dotnet restore
dotnet run
```

### Reader application

The reader application is a GUI Windows application which can read input values.

| Interface    | Devices                       | Requirements                            |
| ------------ | ----------------------------- | --------------------------------------- |
| Windows API  | Mouse and keyboard            | None                                    |
| Raw input    | Mouse, keyboard               | USB device with drivers                 |
| Direct input | Joysticks                     | DirectX compatible devices with drivers |
| XInput       | Joysticks                     | XInput compatible devices               |
