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
3. Check your mapping comparing the input and output blocks.
4. If needed, you can apply deadzone values to axes. (more info below)
5. Close the configuration window
6. Save the settings using the 'File->Save' menu or the button located at the bottom right of the main window.
7. Select 'Start' on the device.

You can check if it is working in the Windows settings, or just select 'File->Game controllers', that opens the Windows calibration for you. An Xbox gamepad should have appeared in the list.

### Deadzone

If your analogue stick isn't in perfect condition, you may have what is called a deadzone, which means that a part of the axis isn't working right, usually it's the center part, syndromes of this are: wrong center position and unwanted movement of the camera, character, etc. To solve apply a bit of deadzone in the mapping settings, how much depends on the device in question.

## Diagnostics

XOutput has a diagnostics screen. A few tests are run to check if the application is working correctly.

| Image                                    | Meaning                                                                                                  |
| ---------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| green circle with a tick                 | Everything is optimal.                                                                                  |
| yellow triangle with an exclamation mark | The experience may be sub-optimal. Some functions may not work, but the application is functional.  |
| red circle with a minus sign             | Something is not working, the application cannot function properly. Some core features may be unusable. |

## Command line arguments

- `--start="controller-displayname"` - defines a part of the display name of the controller to be started on application startup.
- `--minimized` - starts the application minimized to tray

## Developer release [![Build](https://ci.appveyor.com/api/projects/status/github/csutorasa/XOutput?branch=master&svg=true)](https://ci.appveyor.com/project/csutorasa/xoutput/)

If you want to test the latest, often unstable, features before the stable releases, you can check the [AppVeyor](https://ci.appveyor.com/project/csutorasa/xoutput/build/artifacts) builds.
