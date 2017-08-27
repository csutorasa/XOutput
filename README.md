# XOutput

XOutput is a software that can convert DirectInput into XInput. DirectInput data is read and sent to a virtual XInput (Xbox 360 Controller) device. XInput is the new standard game controller input on windows, and DirectInput can no longer be used with Universal Windows Platform softwares, but with this tool you can use DirectInput devices as well. 

To use the software you need to:
* Install Microsoft .NET 4.5.2 (or newer)
* Install Visual C++ 2013 (or newer)
* Install official Xbox 360 Controller driver
* Download and install [ScpToolkit](https://github.com/nefarius/ScpServer/releases/latest)
* Download the [latest release](https://github.com/csutorasa/XOutput/releases/latest) with SlimDX.dll included
* Connect your DirectInput device
* Set your mappings and save the settings
* Start the emulation

# How to use

If all the requirements are installed, the software should start up without any error messages.
The available input devices are shown in the Game Controllers section. To configure a device click open.

On the configuration screen there are 3 blocks. The left block is for the input device, the right one for the emulated output. In the middle the mapping can be set.
For each output axis or button, you can choose from the input axes and buttons. If you want a quick configure instead of the dropdown lists, you can choose auto configure, where each pressed or release button or axis is registered automaticly.

After selecting the input the minimum and the maximum value can be configured. Some examples are shown below:

| Minimum | Maximum | Effect                          |
|---------|---------|---------------------------------|
| 0       | 100     | Axis is mapped as is            |
| 100     | 0       | Axis is reverted                |
| 0       | 50      | Lower half of the axis          |
| 100     | 50      | Upper half of the axis reverted |

After these you can close the configuration window. You can save the settings in the File menu.

The emulation can be started with the start button next to the Open. If the emulation is started, you can check in the windows settings, or just select File/Game controllers, that opens the windows calibration for you.
