[![Build XOutput](https://github.com/csutorasa/XOutput/workflows/Build%20XOutput/badge.svg)](https://github.com/csutorasa/XOutput/actions?query=branch%3Amaster)
[![Github All Releases](https://img.shields.io/github/downloads/csutorasa/XOutput/total.svg)](https://github.com/csutorasa/XOutput/releases/)
[![Download XOutput](https://img.shields.io/sourceforge/dt/xoutput.svg)](https://sourceforge.net/projects/xoutput/files/latest/download)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/csutorasa)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=XOutput&metric=alert_status)](https://sonarcloud.io/dashboard?id=XOutput)
[![Discord](https://img.shields.io/discord/673150216169455637?label=Discord&logo=Discord)](https://discord.gg/Y628tcp)

# XOutput

If you have an older or not supported game controller (gamepad, wheel, joystick, etc.), but XBox 360 controllers are supported you can use this software and enjoy gaming with your controller.

XOutput is a software that can convert DirectInput into XInput. DirectInput data is read and sent to a virtual XInput (Xbox 360 Controller) device. XInput is the new standard game controller input on windows, and DirectInput can no longer be used with Universal Windows Platform software, but with this tool you can use DirectInput devices as well.

## General information

If you have any questions or feature requests feel free to create [Github Issue](https://github.com/csutorasa/XOutput/issues).

Any contribution is welcome, open your [Pull request](https://github.com/csutorasa/XOutput/pulls).

To join the discussion or just say hi in the [discord channel](https://discord.gg/Y628tcp).

This version is currently under development.

| Version                                              | State                                   | Support                                  |
| ---------------------------------------------------- | --------------------------------------- | ---------------------------------------- |
| **[4.x](https://github.com/csutorasa/XOutput)**      | **Under development (no releases yet)** | **Active development with new features** |
| [3.x](https://github.com/csutorasa/XOutput/tree/3.x) | Stable                                  | Bugfixes only                            |
| 2.x                                                  | Old version (superseeded by 3.x)        | Expired at Mar 7 2018                    |
| 1.x                                                  | Old version (superseeded by 3.x)        | Expired at Mar 7 2018                    |

## How to install

### Install emulation software

Install one of the two libraries. ViGEm (preferred) or SCPToolkit (unsupported, legacy)

a) Install [VIGEm framework](https://github.com/ViGEm/ViGEmBus/releases) (recommended)

b) Install [ScpToolkit](https://github.com/nefarius/ScpServer/releases/latest) and all of its dependencies described [here](https://github.com/nefarius/ScpToolkit/blob/master/README.md#installation-requirements)

### Install dependencies

Install:

-   DirectX
-   Visual C++ Redistributable for Visual Studio 2015 32-bit
-   Visual C++ Redistributable for Visual Studio 2015 64-bit
-   all the drivers for your controllers
-   [.Net Framework 4.5.2](https://www.microsoft.com/en-us/download/details.aspx?id=42642) if you do not have Windows 10
-   if you have issues, install the official [XBox 360 controller drivers](https://www.microsoft.com/accessories/en-gb/d/xbox-360-controller-for-windows).

### Download XOutput

-   Download the [latest stable release](https://github.com/csutorasa/XOutput/releases/latest)
-   Download the [latest development release](https://ci.appveyor.com/project/csutorasa/xoutput/build/artifacts), development release is not stable, should be used only to test new features and bugfixes before release
-   Unzip to any directory

## How to upgrade

Download the application:

-   Download the [latest stable release](https://github.com/csutorasa/XOutput/releases/latest)
-   Download the [latest development release](https://ci.appveyor.com/project/csutorasa/xoutput/build/artifacts), development release is not stable, should be used only to test new features and bugfixes before release
-   Unzip to the same directory as before
-   Delete the old `settings.json` file before starting the application

## How to use

If all the requirements are installed, the software should start up without any error messages.

### Input devices

Input devices can be checked and configured individually.
Input values can be seen in the input fields.
Force feedback can be disabled for each device.

### Game controllers

The available input devices are shown in the Game Controllers section. Choose a  device and click 'Edit'.

On the configuration screen there are 3 blocks. The left block shows the input, the right one shows the emulated output and in the middle is where the mapping can be set. For each output axis or button, you can choose from the input axes and buttons.

1.  Press 'Configure All' to set the mapping all at once, or press 'Configure' on each field to set them individually.
2.  Press the button or move the axis from one end to the other.
3.  Check your mapping comparing the input and output blocks.
4.  If needed, you can apply deadzone values to axes. (more info below)
5.  Close the configuration window
6.  Save the settings using the 'File->Save' menu or the button located at the bottom right of the main window.
7.  Select 'Start' on the device.

You can check if it is working in the Windows settings, or just select 'File->Game controllers', that opens the Windows calibration for you. An Xbox gamepad should have appeared in the list.

## HidGuardian (exclusive mode)

-   Install [HidGuardian](https://forums.vigem.org/topic/271/hidguardian-v1-driver-installation).

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
## 4.x Information

The 4.x version is built with .NET 5, you will need to [download it](https://dotnet.microsoft.com/download).

| Action                         | Required dependency  |
| ------------------------------ | -------------------- |
| Running the server application | ASP.NET Core Runtime |
| Running the reader application | .NET Desktop Runtime |
| Building the application       | SDK                  |

However these runtimes and SDKs are cross platform the application is only compatible with Windows.

### Server application

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

### Reader application

The reader application is a GUI Windows application which can read input values.

| Interface    | Devices                       | Requirements                            |
| ------------ | ----------------------------- | --------------------------------------- |
| Windows API  | Mouse and keyboard            | None                                    |
| Raw input    | Mouse, keyboard and joysticks | USB device with drivers                 |
| Direct input | Joysticks                     | DirectX compatible devices with drivers |
| XInput       | Joysticks                     | XInput compatible devices               |

### Client project

The client project enabled developers to create a new type of input.
It can be used to integrate with the server application.
