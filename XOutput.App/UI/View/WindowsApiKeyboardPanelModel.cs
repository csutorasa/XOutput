﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiKeyboardPanelModel : ModelBase
    {
        private bool enabled;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        [ResolverMethod]
        public WindowsApiKeyboardPanelModel()
        {

        }
    }
}
