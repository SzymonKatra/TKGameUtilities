using System;

namespace TKGameUtilities
{
    /// <summary>
    /// Struct that definies angular acceleration
    /// </summary>
    public struct AngularAcceleration : IAcceleration
    {
        /// <summary>
        /// Zero acceleration
        /// </summary>
        public static IAcceleration Zero
        {
            get { return new AngularAcceleration(0f, 0f); }
        }

        /// <summary>
        /// Speed increment
        /// </summary>
        public float Speed;
        /// <summary>
        /// Angle, in degrees
        /// </summary>
        public float Angle;

        /// <summary>
        /// Construct angular acceleration
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="angle">Angle, in degrees</param>
        public AngularAcceleration(float speed, float angle)
        {
            Speed = speed;
            Angle = angle;
        }
        
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        public void ApplyAcceleration(ref Velocity velocity)
        {
            velocity.Speed += Speed;
            velocity.Angle += Angle;
        }
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Result</returns>
        public Velocity ApplyAcceleration(Velocity velocity)
        {
            ApplyAcceleration(ref velocity);
            return velocity;
        }
    }
}
