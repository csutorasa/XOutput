using HidSharp.Reports;
using HidSharp.Reports.Input;
using System;
using System.Collections.Generic;

namespace XOutput.App.Devices.Input.RawInput
{
    public class RawInputSource : InputSource
    {
        private readonly Func<Dictionary<Usage, DataValue>, double?> getNewValue;

        public RawInputSource(IInputDevice inputDevice, string name, SourceTypes type, Usage usage, Func<Dictionary<Usage, DataValue>, double?> getNewValue) : base(inputDevice, name, type, (int)usage)
        {
            this.getNewValue = getNewValue;
        }

        public RawInputSource(IInputDevice inputDevice, string name, SourceTypes type, int usage, Func<Dictionary<Usage, DataValue>, double?> getNewValue) : base(inputDevice, name, type, usage)
        {
            this.getNewValue = getNewValue;
        }

        public static RawInputSource[] FromUsage(IInputDevice device, Usage usage)
        {
            switch (usage)
            {
                case Usage.GenericDesktopX:
                    return new RawInputSource[] { new RawInputSource(device, "X axis", SourceTypes.AxisX, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopRx:
                    return new RawInputSource[] { new RawInputSource(device, "Rx axis", SourceTypes.AxisX, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopY:
                    return new RawInputSource[] { new RawInputSource(device, "Y axis", SourceTypes.AxisY, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopRy:
                    return new RawInputSource[] { new RawInputSource(device, "Ry axis", SourceTypes.AxisY, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopZ:
                    return new RawInputSource[] { new RawInputSource(device, "Z axis", SourceTypes.AxisZ, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopRz:
                    return new RawInputSource[] { new RawInputSource(device, "Rz axis", SourceTypes.AxisZ, usage,
                    (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.Button1:
                case Usage.Button2:
                case Usage.Button3:
                case Usage.Button4:
                case Usage.Button5:
                case Usage.Button6:
                case Usage.Button7:
                case Usage.Button8:
                case Usage.Button9:
                case Usage.Button10:
                case Usage.Button11:
                case Usage.Button12:
                case Usage.Button13:
                case Usage.Button14:
                case Usage.Button15:
                case Usage.Button16:
                case Usage.Button17:
                case Usage.Button18:
                case Usage.Button19:
                case Usage.Button20:
                case Usage.Button21:
                case Usage.Button22:
                case Usage.Button23:
                case Usage.Button24:
                case Usage.Button25:
                case Usage.Button26:
                case Usage.Button27:
                case Usage.Button28:
                case Usage.Button29:
                case Usage.Button30:
                case Usage.Button31:
                    return new RawInputSource[] { new RawInputSource(device, "Button " + (usage - Usage.Button1 + 1), SourceTypes.Button, usage,
                     (changes) => GetValueFromChanges(changes, usage)) };
                case Usage.GenericDesktopHatSwitch:
                    return new RawInputSource[] {
                        new RawInputSource(device, "DPad Up", SourceTypes.Dpad, 100000,
                         (changes) => HasDirection(changes, DPadDirection.Up)),
                        new RawInputSource(device, "DPad Down", SourceTypes.Dpad, 100001,
                         (changes) => HasDirection(changes, DPadDirection.Down)),
                        new RawInputSource(device, "DPad Left", SourceTypes.Dpad, 100002,
                         (changes) => HasDirection(changes, DPadDirection.Left)),
                        new RawInputSource(device, "DPad Right", SourceTypes.Dpad, 100003,
                         (changes) => HasDirection(changes, DPadDirection.Right)),
                    };
                case Usage.GenericDesktopSlider:
                case Usage.GenericDesktopDial:
                case Usage.GenericDesktopWheel:
                    return new RawInputSource[] { new RawInputSource(device, usage.ToString(), SourceTypes.Slider, usage,
                     (changes) => GetValueFromChanges(changes, usage)) };
                default:
                    return new RawInputSource[0];
            }
        }

        private static double? GetValueFromChanges(Dictionary<Usage, DataValue> changes, Usage usage)
        {
            if (changes.ContainsKey(usage))
            {
                var dataValue = changes[usage];
                return dataValue.GetScaledValue(0, 1);
            }
            return null;
        }

        private static double? HasDirection(Dictionary<Usage, DataValue> changes, DPadDirection directionCheck)
        {
            var direction = GetDirection(changes);
            if (direction.HasValue)
            {
                return direction.Value.HasFlag(directionCheck) ? 1 : 0;
            }
            return null;
        }

        private static DPadDirection? GetDirection(Dictionary<Usage, DataValue> changes)
        {
            if (!changes.ContainsKey(Usage.GenericDesktopHatSwitch))
            {
                return null;
            }
            var dataValue = changes[Usage.GenericDesktopHatSwitch];
            switch (dataValue.GetLogicalValue() - dataValue.DataItem.LogicalMinimum)
            {
                case 0: return DPadDirection.None;
                case 1: return DPadDirection.Up;
                case 2: return DPadDirection.Up | DPadDirection.Right;
                case 3: return DPadDirection.Right;
                case 4: return DPadDirection.Down | DPadDirection.Right;
                case 5: return DPadDirection.Down;
                case 6: return DPadDirection.Down | DPadDirection.Left;
                case 7: return DPadDirection.Left;
                case 8: return DPadDirection.Up | DPadDirection.Left;
            }
            return null;
        }

        /*
        public static RawInputSource FromSlider(IInputDevice device, DeviceObjectInstance instance, int index)
        {
            string name = instance.Name;
            return new RawInputSource(device, name, SourceTypes.Slider, instance.Offset, state => GetSliderValue(state, index + 1) / (double)ushort.MaxValue);
        }

        private static int GetSliderValue(JoystickState state, int slider)
        {
            if (slider < 1)
            {
                throw new ArgumentException(nameof(slider));
            }
            return state.Sliders[slider - 1];
        }*/


        internal bool Refresh(Dictionary<Usage, DataValue> newValues)
        {
            double? newValue = getNewValue(newValues);
            if (newValue.HasValue)
            {
                return RefreshValue(newValue.Value);
            }
            return false;
        }
    }
}
