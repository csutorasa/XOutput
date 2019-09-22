using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput.UI
{
    public static class UIConfiguration
    {
        [ResolverMethod(false)]
        public static MainWindowModel GetMainWindowModel()
        {
            return new MainWindowModel();
        }

        [ResolverMethod(false)]
        public static MainWindowViewModel GetMainWindowViewModel(MainWindowModel model, Dispatcher dispatcher, HidGuardianManager hidGuardianManager)
        {
            return new MainWindowViewModel(model, dispatcher, hidGuardianManager);
        }

        [ResolverMethod(false)]
        public static MainWindow GetMainWindow(MainWindowViewModel viewModel, ArgumentParser argumentParser)
        {
            return new MainWindow(viewModel, argumentParser);
        }


        [ResolverMethod(false)]
        public static SettingsModel GetSettingsModel(RegistryModifier registryModifier, Settings settings)
        {
            return new SettingsModel(registryModifier, settings);
        }

        [ResolverMethod(false)]
        public static SettingsViewModel GetSettingsViewModel(SettingsModel model)
        {
            return new SettingsViewModel(model);
        }

        [ResolverMethod(false)]
        public static SettingsWindow GetSettingsWindow(SettingsViewModel viewModel)
        {
            return new SettingsWindow(viewModel);
        }


        [ResolverMethod(false)]
        public static DiagnosticsViewModel GetDiagnosticsViewModel(DiagnosticsModel model)
        {
            return new DiagnosticsViewModel(model);
        }

        [ResolverMethod(false)]
        public static DiagnosticsWindow GetDiagnosticsWindow(DiagnosticsViewModel viewModel)
        {
            return new DiagnosticsWindow(viewModel);
        }
    }
}
