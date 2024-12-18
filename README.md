[![Build XOutput](https://github.com/csutorasa/XOutput/workflows/Build%20XOutput/badge.svg)](https://github.com/csutorasa/XOutput/actions?query=branch%3Amaster)
[![Github All Releases](https://img.shields.io/github/downloads/csutorasa/XOutput/total.svg)](https://github.com/csutorasa/XOutput/releases/)
[![Download XOutput](https://img.shields.io/sourceforge/dt/xoutput.svg)](https://sourceforge.net/projects/xoutput/files/latest/download)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=XOutput&metric=alert_status)](https://sonarcloud.io/dashboard?id=XOutput)
[![Discord](https://img.shields.io/discord/673150216169455637?label=Discord&logo=Discord)](https://discord.gg/Y628tcp)

# XOutput

If you have an older or not supported game controller (gamepad, wheel, joystick, etc.),
but XBox 360 controllers are supported you can use this software and enjoy gaming with your controller.

XOutput is a software that can convert [DirectInput](https://docs.microsoft.com/en-us/previous-versions/windows/desktop/ee416842(v=vs.85)) into [XInput](https://docs.microsoft.com/en-us/windows/win32/xinput).
DirectInput data is read and sent to a virtual XInput (Xbox 360 Controller) device.
XInput is the new standard game controller input on windows, and DirectInput can no longer be used with [Universal Windows Platform](https://docs.microsoft.com/en-us/windows/uwp/) software, but with this tool you can use DirectInput devices as well.

## General information

To join the discussion or just say hi, there is a [discord channel](https://discord.gg/Y628tcp).

**This application is deprecated as the emulation softwares it used are no longer supported.**
**This version is unreleased. Please use [3.x](https://github.com/csutorasa/XOutput/tree/3.x) version for the latest version**

| Version                                              | State                            | Support               |
| ---------------------------------------------------- | -------------------------------- | --------------------- |
| **[4.x](https://github.com/csutorasa/XOutput)**      | **No releases**                  | Unreleased            |
| [3.x](https://github.com/csutorasa/XOutput/tree/3.x) | Unsupported                      | Expired at Dec 3 2024 |
| 2.x                                                  | Old version (superseeded by 3.x) | Expired at Mar 7 2018 |
| 1.x                                                  | Old version (superseeded by 3.x) | Expired at Mar 7 2018 |

## How to install

### Install emulation software

Install one of the two libraries. ViGEm (preferred) or SCPToolkit (unsupported, legacy)

| Software                                            | State                         | Download                                                            |
| --------------------------------------------------- | ----------------------------- | ------------------------------------------------------------------- |
| [ViGEmBus](https://github.com/nefarius/ViGEmBus)    | Deprecated since Nov 2, 2023  | [ViGEmBus](https://github.com/nefarius/ViGEmBus/releases)           |
| [ScpToolkit](https://github.com/nefarius/ScpServer) | Deprecated since Mar 20, 2023 | [ScpToolkit](https://github.com/nefarius/ScpServer/releases/latest) |

### Install dependencies

Install:

-   DirectX (any version that contains dinput8.dll)
-   all the drivers for your controllers
-   [.NET 7 ASP.NET and Desktop runtime](https://dotnet.microsoft.com/download)
-   if you have issues, install the official [XBox 360 controller drivers](https://www.microsoft.com/accessories/en-gb/d/xbox-360-controller-for-windows).

### Download XOutput

-   Download the [latest stable release](https://github.com/csutorasa/XOutput/releases/latest)
-   Unzip to any directory

## How to upgrade

Download the application:

-   Download the [latest stable release](https://github.com/csutorasa/XOutput/releases/latest)
-   Unzip to the same directory as before
-   If there are errors, remove the existing `conf` directory

## Hiding original input (exclusive mode)

| Software                                               | State                         | Download                                                                                    |
| ------------------------------------------------------ | ----------------------------- | ------------------------------------------------------------------------------------------- |
| [HidHide](https://github.com/nefarius/hidhide)         | Active development            | [HidHide](https://github.com/nefarius/HidHide/releases)                                     |
| [HidGuardian](https://github.com/nefarius/hidguardian) | Deprecated since Jul 27, 2023 | [HidGuardian](https://docs.nefarius.at/projects/HidGuardian/HidGuardian-Gen1-Installation/) |

### HidGuardian setup

Affected devices and whitelist can be managed by the application, but it needs administrator priviledges.

1.  Enable HidGuardian initialization in the settings.
2.  Restart the application as administrator.
3.  Add device to affected devices in the input settings.
4.  Reconnect device or reboot PC

Add affected device manually (use this method only, if you want to hide non recognized HID devices):

1.  Run `regedit` and go to `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters`
2.  Create `multi-string` value with the name `AffectedDevices`
3.  Open Device Manager, find your input device and open properties
4.  Go to details, select `Hardware ID` and copy string that looks like `HID\VID_046D&PID_C219&REV_0200` and `HID\VID_046D&PID_C219`
5.  Paste the values into `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters`
6.  Reconnect devices or reboot PC

Manage process whitelist manually (use this method only, if you want to use HidGuardian for other applications):

1.  Start XOutput (or any other application)
2.  Open Task Manager, find your application, then go to details.
3.  Remember the value of the PID column.
4.  Run `regedit` and go to `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters\Whitelist`
5.  Create subkey with the PID you found before.

## Technical problem

Problem visualisation

```mermaid
graph TD
    subgraph XInput
        X1(Xbox controller 1) -->|Read hardware input| XD[Xbox controller driver - xusb22.sys]
        X2(Xbox controller 2) -->|Read hardware input| XD
        XD -->|Read input| XR[XInput handler - xinput1_3.dll]
    end
    subgraph DirectInput
        D1(fa:fa-gamepad Older controller) -->|Read hardware input| D1D[Controller driver]
        D2(Joystick) -->|Read hardware input| D2D[Joystick driver]
        D3(Gaming wheel) -->|Read hardware input| D3D[Gaming wheel driver]
        D1D -->|Read input| DR[Direct input handler - dinput8.dll]
        D2D -->|Read input| DR
        D3D -->|Read input| DR
    end
    subgraph Games
        XR -->|Read XInput| NG[Newer games]
        XR -->|Read XInput| CG[Compatible games]
        DR -->|Read DInput| CG[Compatible games]
        DR -->|Read DInput| OG[Older games]
    end
```

Offered solution

```mermaid
graph TD
    subgraph XInput
        X1(Xbox controller 1) -->|Read hardware input| XD[Xbox controller driver - xusb22.sys]
        X2(Xbox controller 2) -->|Read hardware input| XD
        XD -->|Read input| XR[XInput handler - xinput1_3.dll]
    end
    subgraph DirectInput
        D1(fa:fa-gamepad Older controller) -->|Read hardware input| D1D[Controller driver]
        D2(Joystick) -->|Read hardware input| D2D[Joystick driver]
        D3(Gaming wheel) -->|Read hardware input| D3D[Gaming wheel driver]
        D1D -->|Read input| DR[Direct input handler - dinput8.dll]
        D2D -->|Read input| DR
        D3D -->|Read input| DR
    end
    DR -->|Read DInput| XOA[XOutput.App]
    subgraph Solution
        DR -->|Read HTML5 gamepad API| B[Browser]
        B -->|Browser| XOS[XOutput.Server]
        XOA -->|Sends input| XOS[XOutput.Server]
        XOS -->|Control emulation| V[ViGEm]
    end
    V -->|Emulate controller| XD
    subgraph Games
        XR -->|Read XInput| NG[Newer games]
        XR -->|Read XInput| CG[Compatible games]
        DR -->|Read DInput| CG[Compatible games]
        DR -->|Read DInput| OG[Older games]
    end
```

## How it works

| Component         | Implementation | Description                                                                                                                     |
| ----------------- | -------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Device emualation | ViGEm          | External software that emulates XBox and DS4 controllers                                                                        |
| Device emualation | SCP toolkit    | Legacy external software that emulates XBox controllers                                                                         |
| Backend           | XOutput.Server | Server that collects data from clients and sends to the emulation software                                                      |
| Client            | XOutput.App    | Desktop application that collects data from the devices and sends to the backend                                                |
| Client            | Web controller | Web application that allows to emulate a device                                                                                 |
| Client            | Web client     | Web application that uses the [Gamepad API](https://developer.mozilla.org/en-US/docs/Web/API/Gamepad_API/Using_the_Gamepad_API) |
