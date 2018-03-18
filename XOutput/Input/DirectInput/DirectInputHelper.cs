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
                case DirectInputTypes.Axis7:
                case DirectInputTypes.Axis8:
                case DirectInputTypes.Axis9:
                case DirectInputTypes.Axis10:
                case DirectInputTypes.Axis11:
                case DirectInputTypes.Axis12:
                case DirectInputTypes.Axis13:
                case DirectInputTypes.Axis14:
                case DirectInputTypes.Axis15:
                case DirectInputTypes.Axis16:
                case DirectInputTypes.Axis17:
                case DirectInputTypes.Axis18:
                case DirectInputTypes.Axis19:
                case DirectInputTypes.Axis20:
                case DirectInputTypes.Axis21:
                case DirectInputTypes.Axis22:
                case DirectInputTypes.Axis23:
                case DirectInputTypes.Axis24:
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
                case DirectInputTypes.Button65:
                case DirectInputTypes.Button66:
                case DirectInputTypes.Button67:
                case DirectInputTypes.Button68:
                case DirectInputTypes.Button69:
                case DirectInputTypes.Button70:
                case DirectInputTypes.Button71:
                case DirectInputTypes.Button72:
                case DirectInputTypes.Button73:
                case DirectInputTypes.Button74:
                case DirectInputTypes.Button75:
                case DirectInputTypes.Button76:
                case DirectInputTypes.Button77:
                case DirectInputTypes.Button78:
                case DirectInputTypes.Button79:
                case DirectInputTypes.Button80:
                case DirectInputTypes.Button81:
                case DirectInputTypes.Button82:
                case DirectInputTypes.Button83:
                case DirectInputTypes.Button84:
                case DirectInputTypes.Button85:
                case DirectInputTypes.Button86:
                case DirectInputTypes.Button87:
                case DirectInputTypes.Button88:
                case DirectInputTypes.Button89:
                case DirectInputTypes.Button90:
                case DirectInputTypes.Button91:
                case DirectInputTypes.Button92:
                case DirectInputTypes.Button93:
                case DirectInputTypes.Button94:
                case DirectInputTypes.Button95:
                case DirectInputTypes.Button96:
                case DirectInputTypes.Button97:
                case DirectInputTypes.Button98:
                case DirectInputTypes.Button99:
                case DirectInputTypes.Button100:
                case DirectInputTypes.Button101:
                case DirectInputTypes.Button102:
                case DirectInputTypes.Button103:
                case DirectInputTypes.Button104:
                case DirectInputTypes.Button105:
                case DirectInputTypes.Button106:
                case DirectInputTypes.Button107:
                case DirectInputTypes.Button108:
                case DirectInputTypes.Button109:
                case DirectInputTypes.Button110:
                case DirectInputTypes.Button111:
                case DirectInputTypes.Button112:
                case DirectInputTypes.Button113:
                case DirectInputTypes.Button114:
                case DirectInputTypes.Button115:
                case DirectInputTypes.Button116:
                case DirectInputTypes.Button117:
                case DirectInputTypes.Button118:
                case DirectInputTypes.Button119:
                case DirectInputTypes.Button120:
                case DirectInputTypes.Button121:
                case DirectInputTypes.Button122:
                case DirectInputTypes.Button123:
                case DirectInputTypes.Button124:
                case DirectInputTypes.Button125:
                case DirectInputTypes.Button126:
                case DirectInputTypes.Button127:
                case DirectInputTypes.Button128:
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
