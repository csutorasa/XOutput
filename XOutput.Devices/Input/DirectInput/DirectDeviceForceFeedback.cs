using NLog;
using SharpDX;
using SharpDX.DirectInput;
using System;

namespace XOutput.Devices.Input.DirectInput
{
    public class DirectDeviceForceFeedback : IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private double value;
        public double Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    effect = DoForceFeedback(effect, axes, directions, value);
                    this.value = value;
                }
            }
        }

        private readonly Joystick joystick;
        private readonly EffectInfo force;

        private readonly int[] axes;
        private readonly int[] directions;
        private Effect effect;
        private readonly int gain;
        private readonly int samplePeriod;
        private bool disposed;


        public DirectDeviceForceFeedback(Joystick joystick, EffectInfo force, DeviceObjectInstance actuator)
        {
            this.force = force;
            this.joystick = joystick;
            gain = joystick.Properties.ForceFeedbackGain;
            samplePeriod = joystick.Capabilities.ForceFeedbackSamplePeriod;
            axes = new int[] { (int)actuator.ObjectId };
            directions = new int[] { 0 };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                effect?.Dispose();
            }
            disposed = true;
        }

        private Effect DoForceFeedback(Effect oldEffect, int[] axes, int[] directions, double value)
        {
            var effectParams = new EffectParameters
            {
                Flags = EffectFlags.Cartesian | EffectFlags.ObjectIds,
                StartDelay = 0,
                SamplePeriod = samplePeriod,
                Duration = int.MaxValue,
                TriggerButton = -1,
                TriggerRepeatInterval = int.MaxValue,
                Gain = gain
            };
            effectParams.SetAxes(axes, directions);
            var cf = new ConstantForce
            {
                Magnitude = CalculateMagnitude(value)
            };
            effectParams.Parameters = cf;
            try
            {
                var newEffect = new Effect(joystick, force.Guid, effectParams);
                oldEffect?.Dispose();
                newEffect.Start();
                return newEffect;
            }
            catch (SharpDXException e)
            {
                logger.Warn(e, $"Failed to create and start effect for {ToString()}");
                return null;
            }
        }

        private int CalculateMagnitude(double value)
        {
            return (int)(gain * value);
        }
    }
}
