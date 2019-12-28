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
        [ResolverMethod(Scope.Prototype)]
        public static MainWindowModel GetMainWindowModel()
        {
            return new MainWindowModel();
        }

        [ResolverMethod(Scope.Prototype)]
        public static MainWindowViewModel GetMainWindowViewModel(MainWindowModel model, Dispatcher dispatcher, HidGuardianManager hidGuardianManager)
        {
            return new MainWindowViewModel(model, dispatcher, hidGuardianManager);
        }

        [ResolverMethod(Scope.Prototype)]
        public static MainWindow GetMainWindow(MainWindowViewModel viewModel, ArgumentParser argumentParser)
        {
            return new MainWindow(viewModel, argumentParser);
        }


        [ResolverMethod(Scope.Prototype)]
        public static SettingsModel GetSettingsModel(RegistryModifier registryModifier, Settings settings)
        {
            return new SettingsModel(registryModifier, settings);
        }

        [ResolverMethod(Scope.Prototype)]
        public static SettingsViewModel GetSettingsViewModel(SettingsModel model)
        {
            return new SettingsViewModel(model);
        }

        [ResolverMethod(Scope.Prototype)]
        public static SettingsWindow GetSettingsWindow(SettingsViewModel viewModel)
        {
            return new SettingsWindow(viewModel);
        }


        [ResolverMethod(Scope.Prototype)]
        public static DiagnosticsViewModel GetDiagnosticsViewModel(DiagnosticsModel model)
        {
            return new DiagnosticsViewModel(model);
        }

        [ResolverMethod(Scope.Prototype)]
        public static DiagnosticsWindow GetDiagnosticsWindow(DiagnosticsViewModel viewModel)
        {
            return new DiagnosticsWindow(viewModel);
        }
    }
}
