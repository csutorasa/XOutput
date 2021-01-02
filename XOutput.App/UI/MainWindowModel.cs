using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI
{
    public class MainWindowModel : ModelBase
    {
        private FrameworkElement mainContent;
        public FrameworkElement MainContent
        {
            get => mainContent;
            set { 
                if (value != mainContent)
                {
                    mainContent = value;
                    OnPropertyChanged(nameof(MainContent));
                }
            }
        }

        [ResolverMethod]
        public MainWindowModel()
        {

        }
    }
}
