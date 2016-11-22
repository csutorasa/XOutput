using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.DirectInput
{
    public enum DirectInputTypes
    {
        // Axes
        Axis1,
        Axis2,
        Axis3,
        Axis4,
        Axis5,
        Axis6,

        // Buttons
        Button1,
        Button2,
        Button3,
        Button4,
        Button5,
        Button6,
        Button7,
        Button8,
        Button9,
        Button10,
        Button11,
        Button12,
        Button13,
        Button14,
        Button15,
        Button16,
        Button17,
        Button18,
        Button19,
        Button20
    }
    public static class DirectInputHelper
    {
        public static IEnumerable<DirectInputTypes> GetAll()
        {
            return (DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes));
        }
        public static bool IsButton(this DirectInputTypes input)
        {
            switch (input)
            {
                case DirectInputTypes.Button1:
                case DirectInputTypes.Button2:
                case DirectInputTypes.Button3:
                case DirectInputTypes.Button4:
                case DirectInputTypes.Button5:
                case DirectInputTypes.Button6:
                case DirectInputTypes.Button7:
                case DirectInputTypes.Button8:
                case DirectInputTypes.Button9:
                case DirectInputTypes.Button10:
                case DirectInputTypes.Button11:
                case DirectInputTypes.Button12:
                case DirectInputTypes.Button13:
                case DirectInputTypes.Button14:
                case DirectInputTypes.Button15:
                case DirectInputTypes.Button16:
                case DirectInputTypes.Button17:
                case DirectInputTypes.Button18:
                case DirectInputTypes.Button19:
                case DirectInputTypes.Button20:
                    return true;
                default:
                    return false;
            }
        }
        public static IEnumerable<DirectInputTypes> GetButtons()
        {
            DirectInputTypes[] outputTypes = (DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes));
            return outputTypes.Where(type => type.IsButton());
        }
        public static IEnumerable<DirectInputTypes> GetButtons(DirectDevice device)
        {
            DirectInputTypes[] outputTypes = (DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes));
            return outputTypes.Where(type => type.IsButton()).Take(device.ButtonCount);
        }

        public static bool IsAxis(this DirectInputTypes input)
        {
            switch (input)
            {
                case DirectInputTypes.Axis1:
                case DirectInputTypes.Axis2:
                case DirectInputTypes.Axis3:
                case DirectInputTypes.Axis4:
                case DirectInputTypes.Axis5:
                case DirectInputTypes.Axis6:
                    return true;
                default:
                    return false;
            }
        }
        public static IEnumerable<DirectInputTypes> GetAxes()
        {
            DirectInputTypes[] outputTypes = (DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes));
            return outputTypes.Where(type => type.IsAxis());
        }
    }
}
