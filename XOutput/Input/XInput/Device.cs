using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;

namespace XOutput.Input.XInput
{
    /// <summary>
    /// Device that contains data for a XInput device
    /// </summary>
    public sealed class XDevice
    {
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// </summary>
        public event Action InputChanged;

        private readonly Dictionary<XInputTypes, double> values = new Dictionary<XInputTypes, double>();
        private readonly IInputDevice source;
        private readonly Mapper.InputMapperBase mapper;
        private DPadDirection dPad = DPadDirection.None;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XDevice(IInputDevice source, Mapper.InputMapperBase mapper)
        {
            this.source = source;
            this.mapper = mapper;
            source.InputChanged += () => RefreshInput();
        }

        /// <summary>
        /// Gets the current state of the DPad.
        /// </summary>
        /// <returns></returns>
        public DPadDirection GetDPad()
        {
            return dPad;
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(XInputTypes inputType)
        {
            if (values.ContainsKey(inputType))
            {
                return values[inputType];
            }
            return 0;
        }

        /// <summary>
        /// Refreshes the current state.
        /// </summary>
        /// <returns></returns>
        public bool RefreshInput()
        {
            foreach(var type in (XInputTypes[])Enum.GetValues(typeof(XInputTypes)))
            {
                var mapping = mapper.GetMapping(type);
                if (mapping != null)
                {
                    double value = 0;
                    if (mapping.InputType != null)
                        value = source.Get(mapping.InputType);
                    values[type] = mapping.GetValue(value);
                }
                else
                {

                }
            }
            dPad = source.DPad;
            InputChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Gets binary data to report to scp device.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBinary()
        {
            byte[] report = new byte[20];
            report[0] = 0; // Input report
            report[1] = 20; // Message length

            // Buttons
            if (GetDPad().HasFlag(DPadDirection.Up)) report[2] |= 1 << 0;
            if (GetDPad().HasFlag(DPadDirection.Down)) report[2] |= 1 << 1;
            if (GetDPad().HasFlag(DPadDirection.Left)) report[2] |= 1 << 2;
            if (GetDPad().HasFlag(DPadDirection.Right)) report[2] |= 1 << 3;
            if (GetBool(XInputTypes.Start)) report[2] |= 1 << 4;
            if (GetBool(XInputTypes.Back)) report[2] |= 1 << 5;
            if (GetBool(XInputTypes.L3)) report[2] |= 1 << 6;
            if (GetBool(XInputTypes.R3)) report[2] |= 1 << 7;

            if (GetBool(XInputTypes.L1)) report[3] |= 1 << 0;
            if (GetBool(XInputTypes.R1)) report[3] |= 1 << 1;
            if (GetBool(XInputTypes.Home)) report[3] |= 1 << 2;

            if (GetBool(XInputTypes.A)) report[3] |= 1 << 4;
            if (GetBool(XInputTypes.B)) report[3] |= 1 << 5;
            if (GetBool(XInputTypes.X)) report[3] |= 1 << 6;
            if (GetBool(XInputTypes.Y)) report[3] |= 1 << 7;

            // Axes
            byte l2 = (byte)(Get(XInputTypes.L2) * byte.MaxValue);
            report[4] = l2;
            byte r2 = (byte)(Get(XInputTypes.R2) * byte.MaxValue);
            report[5] = r2;

            ushort lx = (ushort)((Get(XInputTypes.LX) - 0.5) * ushort.MaxValue);
            report[6] = (byte)(lx & 0xFF);
            report[7] = (byte)((lx >> 8) & 0xFF);
            ushort ly = (ushort)((Get(XInputTypes.LY) - 0.5) * ushort.MaxValue);
            report[8] = (byte)(ly & 0xFF);
            report[9] = (byte)((ly >> 8) & 0xFF);

            ushort rx = (ushort)((Get(XInputTypes.RX) - 0.5) * ushort.MaxValue);
            report[10] = (byte)(rx & 0xFF);
            report[11] = (byte)((rx >> 8) & 0xFF);
            ushort ry = (ushort)((Get(XInputTypes.RY) - 0.5) * ushort.MaxValue);
            report[12] = (byte)(ry & 0xFF);
            report[13] = (byte)((ry >> 8) & 0xFF);

            return report;
        }
        public bool GetBool(XInputTypes inputType)
        {
            return Get(inputType) > 0.5;
        }
    }
}
