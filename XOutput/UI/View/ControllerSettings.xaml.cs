using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.Component;
using XOutput.UI.View;

namespace XOutput.UI.View
{
    /// <summary>
    /// Interaction logic for ControllerSettings.xaml
    /// </summary>
    public partial class ControllerSettings : Window
    {
        private readonly GameController controller;

        private readonly DispatcherTimer timer = new DispatcherTimer();

        private readonly ControllerSettingsViewModel viewModel = new ControllerSettingsViewModel();

        public ControllerSettings(GameController controller)
        {
            this.controller = controller;
            viewModel.Title = controller.DisplayName;
            createDirectInputControls();
            createMappingControls();
            createXInputControls();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            update();

            timer.Interval = TimeSpan.FromMilliseconds(25);
            timer.Tick += (object sender1, EventArgs e1) => { update(); };
            timer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void createDirectInputControls()
        {
            foreach (var buttonInput in DirectInputHelper.GetButtons(controller.DirectInput))
            {
                var inputButtonView = new ButtonView(buttonInput);
                viewModel.DirectInputButtonViews.Add(inputButtonView);
            }
            if (controller.DirectInput.HasAxes)
            {
                foreach (var axisInput in DirectInputHelper.GetAxes())
                {
                    var inputAxisView = new AxisView(axisInput);
                    viewModel.DirectInputAxisViews.Add(inputAxisView);
                }
            }
        }
        private void updateDirectInputControls()
        {
            foreach (var axisView in viewModel.DirectInputAxisViews)
            {
                axisView.Value = (int)(controller.DirectInput.Get((DirectInputTypes)axisView.Type) * 1000);
            }
            foreach (var buttonView in viewModel.DirectInputButtonViews)
            {
                buttonView.Value = controller.DirectInput.GetBool((DirectInputTypes)buttonView.Type);
            }
            if (controller.DirectInput.HasDPad)
            {
                viewModel.DirectDPadText = controller.DirectInput.GetDPad().ToString();
            }
            else
            {
                viewModel.DirectDPadText = "This device has no DPad";
            }
        }


        private void createMappingControls()
        {
            foreach (var xInputType in XInputHelper.GetButtons())
            {
                var mappingView = new MappingView(xInputType, controller.Mapper.GetMapping(xInputType));
                viewModel.MapperButtonViews.Add(mappingView);
            }
            foreach (var xInputType in XInputHelper.GetAxes())
            {
                var mappingView = new MappingView(xInputType, controller.Mapper.GetMapping(xInputType));
                viewModel.MapperAxisViews.Add(mappingView);
            }
            if (controller.DirectInput.HasDPad)
            {
                viewModel.MapperDPadText = "DPad is automatically mapped";
            }
            else
            {
                viewModel.MapperDPadText = "This device has no DPad";
            }
        }

        private void createXInputControls()
        {
            foreach (var buttonInput in XInputHelper.GetButtons())
            {
                var inputButtonView = new ButtonView(buttonInput);
                viewModel.XInputButtonViews.Add(inputButtonView);
            }
            foreach (var axisInput in XInputHelper.GetAxes())
            {
                var inputAxisView = new AxisView(axisInput);
                viewModel.XInputAxisViews.Add(inputAxisView);
            }
        }
        private void updateXInputControls()
        {
            foreach (var axisView in viewModel.XInputAxisViews)
            {
                axisView.Value = (int)(controller.XInput.Get((XInputTypes)axisView.Type) * 1000);
            }
            foreach (var buttonView in viewModel.XInputButtonViews)
            {
                buttonView.Value = controller.XInput.GetBool((XInputTypes)buttonView.Type);
            }
            viewModel.XDPadText = controller.XInput.GetDPad().ToString();
        }

        private void update()
        {
            if(!controller.DirectInput.RefreshInput())
            {
                Close();
                return;
            }

            updateDirectInputControls();

            updateXInputControls();
        }
    }
}
