﻿using System;
using OpenTK;

namespace TKGameUtilities.FixedPointMath
{
    /// <summary>
    /// Struct that definies linear acceleration
    /// </summary>
    public struct Fix64LinearAcceleration : IFix64Acceleration
    {
        /// <summary>
        /// Zero acceleration
        /// </summary>
        public static IFix64Acceleration Zero
        {
            get { return new Fix64LinearAcceleration(Fix64Vector2.Zero); }
        }

        /// <summary>
        /// Linear acceleration
        /// </summary>
        public Fix64Vector2 Acceleration;

        /// <summary>
        /// Construct linear acceleration
        /// </summary>
        /// <param name="acceleration">Linear acceleration</param>
        public Fix64LinearAcceleration(Fix64Vector2 acceleration)
        {
            Acceleration = acceleration;
        }

        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        public void ApplyAcceleration(ref Fix64Velocity velocity)
        {
            //velocity.LinearVelocity = Acceleration + velocity.LinearVelocity;
            Fix64Vector2 start = velocity.LinearVelocity;
            velocity.LinearVelocity = new Fix64Vector2(Acceleration.X + start.X, Acceleration.Y + start.Y);
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
