using System;
using System.Linq;
using XOutput.App.Devices.Input;
using XOutput.App.Devices.Input.DirectInput;
using XOutput.App.Devices.Input.RawInput;
using XOutput.App.UI.View.InputView;
using XOutput.DependencyInjection;

namespace XOutput.App.UI
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>
    {
        private readonly InputDeviceManager inputDeviceManager;

        [ResolverMethod]
        public MainWindowViewModel(MainWindowModel model, InputDeviceManager inputDeviceManager) : base(model)
        {
            this.inputDeviceManager = inputDeviceManager;
            foreach (var inputDeviceHolder in inputDeviceManager.GetInputDevices())
            {
                foreach (var inputDevice in inputDeviceHolder.GetInputDevices().Where(d => d.InputMethod == InputDeviceMethod.DirectInput).OfType<DirectInputDevice>())
                {
                    Model.DirectInputs.Add(inputDevice);
                }
                foreach (var inputDevice in inputDeviceHolder.GetInputDevices().Where(d => d.InputMethod == InputDeviceMethod.RawInput).OfType<RawInputDevice>())
                {
                    Model.RawInputs.Add(inputDevice);
                }
            }
        }

        public void DeviceSelected(IInputDevice inputDevice)
        {
            switch (inputDevice.InputMethod)
            {
                case InputDeviceMethod.DirectInput:
                    Model.MainContent = new DirectInputDeviceView(inputDevice as DirectInputDevice);
                    break;
                case InputDeviceMethod.RawInput:
                    Model.MainContent = new RawInputDeviceView(inputDevice as RawInputDevice);
                    break;
            }
        }
    }
}
