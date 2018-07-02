using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace XOutput.UI.Component
{
    public class Icon : Label
    {
        public Icon()
        {
            FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./resources/Fonts/FontAwesome.otf#Font Awesome 5 Free Solid");
        }
    }
}
