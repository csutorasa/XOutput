using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XOutput.Input.DirectInput;

namespace XOutput.Input
{
    public static class InputHelper
    {
        public static bool IsButton(this Enum input)
        {
            if (input is Key)
            {
                return true;
            }
            else if (input is DirectInputTypes)
            {
                switch ((DirectInputTypes)input)
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
            throw new ArgumentException();
        }

        public static bool IsAxis(this Enum input)
        {
            if (input is Key)
            {
                return true;
            }
            else if (input is DirectInputTypes)
            {
                switch ((DirectInputTypes)input)
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
            throw new ArgumentException();
        }
    }
}
