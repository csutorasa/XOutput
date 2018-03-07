# XOutput

If you have an older or not supported game controller (gamepad, wheel, joystick, etc.), but XBox 360 controllers are supported you can use this software and enjoy the gaming with your controller.

XOutput is a software that can convert DirectInput into XInput. DirectInput data is read and sent to a virtual XInput (Xbox 360 Controller) device. XInput is the new standard game controller input on windows, and DirectInput can no longer be used with Universal Windows Platform softwares, but with this tool you can use DirectInput devices as well.

## How to install

Install one of the two libraries. ViGEm is a newer (preferred), while SCPToolkit is a older (unsupported, legacy) solution.
I recommend using the ViGEm framework.

* Install [VIGEm framework](https://github.com/nefarius/ViGEm/wiki/Driver-Installation)
* Install [ScpToolkit](https://github.com/nefarius/ScpServer/releases/latest) and all of its dependencies described [here](https://github.com/nefarius/ScpToolkit/blob/master/README.md#installation-requirements)

Download the application:

* Download the [latest release](https://github.com/csutorasa/XOutput/releases/latest)

## How to use

If all the requirements are installed, the software should start up without any error messages.
The available input devices are shown in the Game Controllers section. To configure a device click edit.

On the configuration screen there are 3 blocks. The left block is for the input device, the right one for the emulated output. In the middle the mapping can be set. For each output axis or button, you can choose from the input axes and buttons.

You can choose configure to configure your output mapping. You need to press the button or move the axis from one end to the another. After this click save and you can fine tune manually if needed with the dropdown menus and percentage value textboxes.

After these you can close the configuration window. You can save the settings in the File menu.

The emulation can be started with the start button next to the Edit. If the emulation is started, you can check in the windows settings, or just select File/Game controllers, that opens the windows calibration for you.

## Manual config (usually not needed)

After selecting the input the minimum and the maximum value can be configured. Some examples are shown below:

| Minimum | Maximum | Effect                                            |
|---------|---------|---------------------------------------------------|
| 0       | 100     | Axis is mapped as is                              |
| 100     | 0       | Axis is reverted                                  |
| 0       | 50      | Lower half of the axis                            |
| 50      | 100     | Upper half of the axis                            |
| 50      | 0       | Lower half of the axis reverted                   |
| 100     | 50      | Upper half of the axis reverted                   |
| 0       | 0       | Axis is always at the lower end, input is ignored |
| 50      | 50      | Axis is always centered, input is ignored         |
| 100     | 100     | Axis is always at the upper end, input is ignored |
