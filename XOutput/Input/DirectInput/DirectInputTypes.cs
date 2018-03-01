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
        Button20,
        Button21,
        Button22,
        Button23,
        Button24,
        Button25,
        Button26,
        Button27,
        Button28,
        Button29,
        Button30,
        Button31,
        Button32,

        // Sliders
        Slider1,
        Slider2,
    }

    public class DirectInputHelper : AbstractInputHelper<DirectInputTypes>
    {
        public static readonly DirectInputHelper instance = new DirectInputHelper();
        public static DirectInputHelper Instance => instance;

        public override bool IsAxis(DirectInputTypes type)
        {
            switch (type)
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

        public override bool IsButton(DirectInputTypes type)
        {
            switch (type)
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
                case DirectInputTypes.Button21:
                case DirectInputTypes.Button22:
                case DirectInputTypes.Button23:
                case DirectInputTypes.Button24:
                case DirectInputTypes.Button25:
                case DirectInputTypes.Button26:
                case DirectInputTypes.Button27:
                case DirectInputTypes.Button28:
                case DirectInputTypes.Button29:
                case DirectInputTypes.Button30:
                case DirectInputTypes.Button31:
                case DirectInputTypes.Button32:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsDPad(DirectInputTypes type)
        {
            return false;
        }

        public override bool IsSlider(DirectInputTypes type)
        {
            switch (type)
            {
                case DirectInputTypes.Slider1:
                case DirectInputTypes.Slider2:
                    return true;
                default:
                    return false;
            }
        }
    }
}
