using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using XOutput.Input.Mapper;
using XOutput.Input.XInput;

namespace XOutput.UI.View
{
    /// <summary>
    /// Interaction logic for AutoConfigureWindow.xaml
    /// </summary>
    public partial class AutoConfigureWindow : Window, INotifyPropertyChanged
    {
        public AutoConfigureModel Model { get { return model; } }

        private static readonly double ResetInterval = 1000;
        private readonly AutoConfigureModel model = new AutoConfigureModel();
        private readonly Dictionary<Enum, double> lastReadValues = new Dictionary<Enum, double>();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly GameController controller;
        private XInputTypes[] xInputTypes;
        private Enum[] inputTypes;
        private int currentIndex = 0;
        private bool reading = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public AutoConfigureWindow(GameController controller)
        {
            this.controller = controller;
            DataContext = this;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += (object sender1, EventArgs e1) => startReading();
            timer.Interval = TimeSpan.FromMilliseconds(ResetInterval);
            xInputTypes = XInputHelper.Values.ToArray();
            inputTypes = controller.InputDevice.GetButtons().Concat(controller.InputDevice.GetAxes()).ToArray();
            foreach (var type in inputTypes)
            {
                lastReadValues.Add(type, controller.InputDevice.Get(type));
            }
            controller.InputDevice.InputChanged += readValues;
            startReading();
        }
        
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MapperData md = controller.Mapper.GetMapping(xInputTypes[currentIndex]);
            if (Model.MaxType != null)
            {
                md.InputType = Model.MaxType;
                md.MinValue = 0;
                md.MaxValue = 1;
            }
            else
            {
                md.InputType = inputTypes[0];
                md.MinValue = 0.5;
                md.MaxValue = 0.5;
            }
            next(false);
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            controller.InputDevice.InputChanged -= readValues;
            timer.Stop();
        }

        /// <summary>
        /// Changes to the next XInput value to be read.
        /// </summary>
        /// <param name="timed">If the program should wait before reading values.</param>
        private void next(bool timed)
        {
            currentIndex++;
            Model.MaxType = null;
            if (currentIndex < xInputTypes.Length)
            {
                if (timed)
                {
                    reading = false;
                    Model.LabelText = $"{controller.Mapper.GetMapping(xInputTypes[currentIndex - 1]).InputType} was assigned to XInput.{xInputTypes[currentIndex - 1]}";
                    if (timer.IsEnabled)
                        timer.Stop();
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    startReading();
                }
            }
            else
            {
                Dispatcher.Invoke(Close);
            }
        }

        /// <summary>
        /// Starts reading values for detection.
        /// </summary>
        private void startReading()
        {
            timer.Stop();
            Model.LabelText = $"Waiting for input for XInput.{xInputTypes[currentIndex]}";
            readReferenceValues();
        }

        /// <summary>
        /// Reads reference values for comparing.
        /// </summary>
        private void readReferenceValues()
        {
            foreach (var type in inputTypes)
            {
                lastReadValues[type] = controller.InputDevice.Get(type);
            }
            reading = true;
        }

        /// <summary>
        /// Reads the current values, and if the values have changed enough saves them.
        /// </summary>
        private void readValues()
        {
            if (reading)
            {
                Enum maxType = null;
                double maxDiff = 0;
                foreach (var type in inputTypes)
                {
                    double oldValue = lastReadValues[type];
                    double newValue = controller.InputDevice.Get(type);
                    double diff = Math.Abs(newValue - oldValue);
                    if (diff > maxDiff)
                    {
                        maxType = type;
                        maxDiff = diff;
                    }
                }
                if (maxDiff > 0.3)
                {
                    if (Model.IsAuto)
                    {
                        MapperData md = controller.Mapper.GetMapping(xInputTypes[currentIndex]);
                        md.InputType = maxType;
                        md.MinValue = 0;
                        md.MaxValue = 1;
                        next(true);
                    }
                    else
                    {
                        Model.MaxType = maxType;
                    }
                }
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
