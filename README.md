# XOutput

If you have an older or not supported game controller (gamepad, wheel, joystick, etc.), but XBox 360 controllers are supported you can use this software and enjoy gaming with your controller.

XOutput is a software that can convert DirectInput into XInput. DirectInput data is read and sent to a virtual XInput (Xbox 360 Controller) device. XInput is the new standard game controller input on windows, and DirectInput can no longer be used with Universal Windows Platform software, but with this tool you can use DirectInput devices as well.

## How to install

Install one of the two libraries. ViGEm (preferred) or SCPToolkit (unsupported, legacy)

  a) Install [VIGEm framework](https://github.com/nefarius/ViGEm/wiki/Driver-Installation) (Recommended)
  b) Install [ScpToolkit](https://github.com/nefarius/ScpServer/releases/latest) and all of its dependencies described [here](https://github.com/nefarius/ScpToolkit/blob/master/README.md#installation-requirements)

Download the application:

* Download the [latest stable release](https://github.com/csutorasa/XOutput/releases/latest)

## How to use

If all the requirements are installed, the software should start up without any error messages.

The available input devices are shown in the Game Controllers section. Choose a  device and click 'Edit'.

On the configuration screen there are 3 blocks. The left block shows the input, the right one shows the emulated output and in the middle is where the mapping can be set. For each output axis or button, you can choose from the input axes and buttons.

1. Press 'Configure All' to set the mapping all at once, or press 'Configure' on each field to set them individually.
2. Press the button or move the axis from one end to the other.
3. Check your mapping comparing the input and output blocks. If needed, you can fine tune the configuration manually with the dropdown menus and percentage value textboxes. (more info below)
4. Close the configuration window
5. Save the settings using the 'File->Save' menu or the button located at the bottom right of the main window.
6. Select 'Start' on the device.

You can check if it is working in the Windows settings, or just select 'File->Game controllers', that opens the Windows calibration for you. An Xbox gamepad should have appeared in the list.

## Manual configuration (usually not needed)

After selecting the input, the minimum and maximum values can be configured. Some examples are shown below:

| Minimum | Maximum | Effect                                                  |
|---------|---------|---------------------------------------------------------|
| 0       | 100     | The axis is mapped as is                                |
| 100     | 0       | The axis is inverted                                    |
| 0       | 50      | Lower half of the axis                                  |
| 50      | 100     | Upper half of the axis                                  |
| 50      | 0       | Lower half of the axis inverted                         |
| 100     | 50      | Upper half of the axis inverted                         |
| 0       | 0       | The axis is always at the bottom-left, input is ignored |
| 50      | 50      | The axis is always centered, input is ignored           |
| 100     | 100     | The axis is always at the upper-right, input is ignored |

### Deadzone

If your analogue stick isn't in perfect condition, you may have what is called a deadzone, which means that a part of the axis isn't working right, usually it's the center part, syndromes of this are: wrong center position and unwanted movement of the camera, character, etc. To solve go to the game settings and apply a bit of deadzone, how much depends on the device in question.

## Command line arguments

- `--start="controller-displayname"` - defines a part of the display name of the controller to be started on application startup.
- `--minimized` - starts the application minimized to tray

## Developer release [![Build](https://ci.appveyor.com/api/projects/status/github/csutorasa/XOutput)](https://ci.appveyor.com/project/csutorasa/xoutput/)

If you want to test the latest, often unstable, features before the stable releases, you can check the [AppVeyor](https://ci.appveyor.com/project/csutorasa/xoutput/build/artifacts) builds.
