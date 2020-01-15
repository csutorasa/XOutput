using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Collections.Generic;

namespace XOutput.Devices.XInput.Vigem
{
    /// <summary>
    /// ViGEm XOutput implementation.
    /// </summary>
    public sealed class VigemDevice : IXOutputInterface
    {
        private readonly ViGEmClient client;
        private readonly Dictionary<int, IXbox360Controller> controllers = new Dictionary<int, IXbox360Controller>();
        private readonly Dictionary<XInputTypes, VigemXbox360ButtonMapping> buttonMappings = new Dictionary<XInputTypes, VigemXbox360ButtonMapping>();
        private readonly Dictionary<XInputTypes, VigemXbox360AxisMapping> axisMappings = new Dictionary<XInputTypes, VigemXbox360AxisMapping>();
        private readonly Dictionary<XInputTypes, VigemXbox360SliderMapping> sliderMappings = new Dictionary<XInputTypes, VigemXbox360SliderMapping>();

        public VigemDevice()
        {
            InitMapping();
            client = new ViGEmClient();
        }

        /// <summary>
        /// Gets if <see cref="VigemDevice"/> is available.
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            try
            {
                new ViGEmClient().Dispose();
                return true;
            }
            catch (VigemBusNotFoundException)
            {
                return false;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Implements <see cref="IXOutputInterface.Plugin(int)"/>
        /// </summary>
        /// <param name="controllerCount">number of controller</param>
        /// <returns>If it was successful</returns>
        public bool Plugin(int controllerCount)
        {
            var controller = client.CreateXbox360Controller();
            controller.Connect();
            controllers.Add(controllerCount, controller);
            return true;
        }

        /// <summary>
        /// Implements <see cref="IXOutputInterface.Unplug(int)"/>
        /// </summary>
        /// <param name="controllerCount">number of controller</param>
        /// <returns>If it was successful</returns>
        public bool Unplug(int controllerCount)
        {
            if (controllers.ContainsKey(controllerCount))
            {
                var controller = controllers[controllerCount];
                controllers.Remove(controllerCount);
                controller.Disconnect();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Implements <see cref="IXOutputInterface.Report(int, Dictionary{XInputTypes, double})"/>
        /// </summary>
        /// <param name="controllerCount">Number of controller</param>
        /// <param name="values">values for each XInput</param>
        /// <returns>If it was successful</returns>
        public bool Report(int controllerCount, Dictionary<XInputTypes, double> values)
        {
            if (controllers.ContainsKey(controllerCount))
            {
                var controller = controllers[controllerCount];
                foreach (var value in values)
                {
                    if (value.Key.IsAxis())
                    {
                        var mapping = axisMappings[value.Key];
                        controller.SetAxisValue(mapping.Type, mapping.GetValue(value.Value));
                    }
                    else if (value.Key.IsSlider())
                    {
                        var mapping = sliderMappings[value.Key];
                        controller.SetSliderValue(mapping.Type, mapping.GetValue(value.Value));
                    }
                    else
                    {
                        var mapping = buttonMappings[value.Key];
                        controller.SetButtonState(mapping.Type, mapping.GetValue(value.Value));
                    }
                }
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (var controller in controllers.Values)
            {
                controller.Disconnect();
            }
            client.Dispose();
        }

        public IXbox360Controller GetController(int controllerCount)
        {
            return controllers[controllerCount];
        }

        private void InitMapping()
        {
            buttonMappings.Add(XInputTypes.A, new VigemXbox360ButtonMapping(Xbox360Button.A));
            buttonMappings.Add(XInputTypes.B, new VigemXbox360ButtonMapping(Xbox360Button.B));
            buttonMappings.Add(XInputTypes.X, new VigemXbox360ButtonMapping(Xbox360Button.X));
            buttonMappings.Add(XInputTypes.Y, new VigemXbox360ButtonMapping(Xbox360Button.Y));
            buttonMappings.Add(XInputTypes.L1, new VigemXbox360ButtonMapping(Xbox360Button.LeftShoulder));
            buttonMappings.Add(XInputTypes.R1, new VigemXbox360ButtonMapping(Xbox360Button.RightShoulder));
            buttonMappings.Add(XInputTypes.Back, new VigemXbox360ButtonMapping(Xbox360Button.Back));
            buttonMappings.Add(XInputTypes.Start, new VigemXbox360ButtonMapping(Xbox360Button.Start));
            buttonMappings.Add(XInputTypes.Home, new VigemXbox360ButtonMapping(Xbox360Button.Guide));
            buttonMappings.Add(XInputTypes.R3, new VigemXbox360ButtonMapping(Xbox360Button.RightThumb));
            buttonMappings.Add(XInputTypes.L3, new VigemXbox360ButtonMapping(Xbox360Button.LeftThumb));

            buttonMappings.Add(XInputTypes.UP, new VigemXbox360ButtonMapping(Xbox360Button.Up));
            buttonMappings.Add(XInputTypes.DOWN, new VigemXbox360ButtonMapping(Xbox360Button.Down));
            buttonMappings.Add(XInputTypes.LEFT, new VigemXbox360ButtonMapping(Xbox360Button.Left));
            buttonMappings.Add(XInputTypes.RIGHT, new VigemXbox360ButtonMapping(Xbox360Button.Right));

            axisMappings.Add(XInputTypes.LX, new VigemXbox360AxisMapping(Xbox360Axis.LeftThumbX));
            axisMappings.Add(XInputTypes.LY, new VigemXbox360AxisMapping(Xbox360Axis.LeftThumbY));
            axisMappings.Add(XInputTypes.RX, new VigemXbox360AxisMapping(Xbox360Axis.RightThumbX));
            axisMappings.Add(XInputTypes.RY, new VigemXbox360AxisMapping(Xbox360Axis.RightThumbY));
            sliderMappings.Add(XInputTypes.L2, new VigemXbox360SliderMapping(Xbox360Slider.LeftTrigger));
            sliderMappings.Add(XInputTypes.R2, new VigemXbox360SliderMapping(Xbox360Slider.RightTrigger));
        }
    }
}
