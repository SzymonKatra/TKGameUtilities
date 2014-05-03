using System;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Struct that definies linear acceleration
    /// </summary>
    public struct LinearAcceleration : IAcceleration
    {
        /// <summary>
        /// Zero acceleration
        /// </summary>
        public static IAcceleration Zero
        {
            get { return new LinearAcceleration(Vector2.Zero); }
        }

        /// <summary>
        /// Linear acceleration
        /// </summary>
        public Vector2 Acceleration;

        /// <summary>
        /// Construct linear acceleration
        /// </summary>
        /// <param name="acceleration">Linear acceleration</param>
        public LinearAcceleration(Vector2 acceleration)
        {
            Acceleration = acceleration;
        }

        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        public void ApplyAcceleration(ref Velocity velocity)
        {
            velocity.LinearVelocity = Acceleration + velocity.LinearVelocity;
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
