using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.DirectInput
{
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
                case DirectInputTypes.Button33:
                case DirectInputTypes.Button34:
                case DirectInputTypes.Button35:
                case DirectInputTypes.Button36:
                case DirectInputTypes.Button37:
                case DirectInputTypes.Button38:
                case DirectInputTypes.Button39:
                case DirectInputTypes.Button40:
                case DirectInputTypes.Button41:
                case DirectInputTypes.Button42:
                case DirectInputTypes.Button43:
                case DirectInputTypes.Button44:
                case DirectInputTypes.Button45:
                case DirectInputTypes.Button46:
                case DirectInputTypes.Button47:
                case DirectInputTypes.Button48:
                case DirectInputTypes.Button49:
                case DirectInputTypes.Button50:
                case DirectInputTypes.Button51:
                case DirectInputTypes.Button52:
                case DirectInputTypes.Button53:
                case DirectInputTypes.Button54:
                case DirectInputTypes.Button55:
                case DirectInputTypes.Button56:
                case DirectInputTypes.Button57:
                case DirectInputTypes.Button58:
                case DirectInputTypes.Button59:
                case DirectInputTypes.Button60:
                case DirectInputTypes.Button61:
                case DirectInputTypes.Button62:
                case DirectInputTypes.Button63:
                case DirectInputTypes.Button64:
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
