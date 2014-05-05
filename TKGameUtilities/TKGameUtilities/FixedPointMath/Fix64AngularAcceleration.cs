using System;

namespace TKGameUtilities.FixedPointMath
{
    /// <summary>
    /// Struct that definies angular acceleration
    /// </summary>
    public struct Fix64AngularAcceleration : IFix64Acceleration
    {
        /// <summary>
        /// Zero acceleration
        /// </summary>
        public static IFix64Acceleration Zero
        {
            get { return new Fix64AngularAcceleration(Fix64.Zero, Fix64.Zero); }
        }

        /// <summary>
        /// Speed increment
        /// </summary>
        public Fix64 Speed;
        /// <summary>
        /// Angle, in degrees
        /// </summary>
        public Fix64 Angle;

        /// <summary>
        /// Construct angular acceleration
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="angle">Angle, in degrees</param>
        public Fix64AngularAcceleration(Fix64 speed, Fix64 angle)
        {
            Speed = speed;
            Angle = angle;
        }
        
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        public void ApplyAcceleration(ref Fix64Velocity velocity)
        {
            velocity.Speed += Speed;
            velocity.Angle += Angle;
        }
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Result</returns>
        public Fix64Velocity ApplyAcceleration(Fix64Velocity velocity)
        {
            ApplyAcceleration(ref velocity);
            return velocity;
        }
    }
}
