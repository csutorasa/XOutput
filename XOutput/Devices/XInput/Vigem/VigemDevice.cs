using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Nefarius.ViGEm.Client.Exceptions;

namespace XOutput.Devices.XInput.Vigem
{
    /// <summary>
    /// ViGEm XOutput implementation.
    /// </summary>
    public sealed class VigemDevice : IXOutputInterface
    {
        private readonly ViGEmClient client;
        private readonly Dictionary<int, Xbox360Controller> controllers = new Dictionary<int, Xbox360Controller>();
        private readonly Dictionary<XInputTypes, VigemXbox360ButtonMapping> buttonMappings = new Dictionary<XInputTypes, VigemXbox360ButtonMapping>();
        private readonly Dictionary<XInputTypes, VigemXbox360AxisMapping> axisMappings = new Dictionary<XInputTypes, VigemXbox360AxisMapping>();

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
            var controller = new Xbox360Controller(client);
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
                var report = new Xbox360Report();
                foreach (var value in values)
                {
                    if (value.Key.IsAxis())
                    {
                        var mapping = axisMappings[value.Key];
                        report.SetAxis(mapping.Type, mapping.GetValue(value.Value));
                    }
                    else
                    {
                        var mapping = buttonMappings[value.Key];
                        report.SetButtonState(mapping.Type, mapping.GetValue(value.Value));
                    }
                }
                controllers[controllerCount].SendReport(report);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (var controller in controllers.Values)
            {
                controller.Dispose();
            }
            client.Dispose();
        }

        public Xbox360Controller GetController(int controllerCount)
        {
            return controllers[controllerCount];
        }

        private void InitMapping()
        {
            buttonMappings.Add(XInputTypes.A, new VigemXbox360ButtonMapping(Xbox360Buttons.A));
            buttonMappings.Add(XInputTypes.B, new VigemXbox360ButtonMapping(Xbox360Buttons.B));
            buttonMappings.Add(XInputTypes.X, new VigemXbox360ButtonMapping(Xbox360Buttons.X));
            buttonMappings.Add(XInputTypes.Y, new VigemXbox360ButtonMapping(Xbox360Buttons.Y));
            buttonMappings.Add(XInputTypes.L1, new VigemXbox360ButtonMapping(Xbox360Buttons.LeftShoulder));
            buttonMappings.Add(XInputTypes.R1, new VigemXbox360ButtonMapping(Xbox360Buttons.RightShoulder));
            buttonMappings.Add(XInputTypes.Back, new VigemXbox360ButtonMapping(Xbox360Buttons.Back));
            buttonMappings.Add(XInputTypes.Start, new VigemXbox360ButtonMapping(Xbox360Buttons.Start));
            buttonMappings.Add(XInputTypes.Home, new VigemXbox360ButtonMapping(Xbox360Buttons.Guide));
            buttonMappings.Add(XInputTypes.R3, new VigemXbox360ButtonMapping(Xbox360Buttons.RightThumb));
            buttonMappings.Add(XInputTypes.L3, new VigemXbox360ButtonMapping(Xbox360Buttons.LeftThumb));

            buttonMappings.Add(XInputTypes.UP, new VigemXbox360ButtonMapping(Xbox360Buttons.Up));
            buttonMappings.Add(XInputTypes.DOWN, new VigemXbox360ButtonMapping(Xbox360Buttons.Down));
            buttonMappings.Add(XInputTypes.LEFT, new VigemXbox360ButtonMapping(Xbox360Buttons.Left));
            buttonMappings.Add(XInputTypes.RIGHT, new VigemXbox360ButtonMapping(Xbox360Buttons.Right));

            axisMappings.Add(XInputTypes.LX, new VigemXbox360AxisMapping(Xbox360Axes.LeftThumbX));
            axisMappings.Add(XInputTypes.LY, new VigemXbox360AxisMapping(Xbox360Axes.LeftThumbY));
            axisMappings.Add(XInputTypes.RX, new VigemXbox360AxisMapping(Xbox360Axes.RightThumbX));
            axisMappings.Add(XInputTypes.RY, new VigemXbox360AxisMapping(Xbox360Axes.RightThumbY));
            axisMappings.Add(XInputTypes.L2, new VigemXbox360AxisMapping(Xbox360Axes.LeftTrigger));
            axisMappings.Add(XInputTypes.R2, new VigemXbox360AxisMapping(Xbox360Axes.RightTrigger));
        }
    }
}
