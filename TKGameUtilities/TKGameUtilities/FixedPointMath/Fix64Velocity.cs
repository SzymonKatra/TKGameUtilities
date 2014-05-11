using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities.FixedPointMath
{
    public struct Fix64Velocity : IEquatable<Fix64Velocity>
    {
        #region Properties
        /// <summary>Speed = 0. Angle = 0</summary>
        public static readonly Fix64Velocity Zero = new Fix64Velocity(Fix64.Zero, Fix64.Zero);

        /// <summary>Speed = 1. Angle = 0</summary>
        public static readonly Fix64Velocity OneRight = new Fix64Velocity(Fix64.One, (Fix64)0L);
        /// <summary>Speed = 1. Angle = 90</summary>
        public static readonly Fix64Velocity OneUp = new Fix64Velocity(Fix64.One, (Fix64)90L);
        /// <summary>Speed = 1. Angle = 180</summary>
        public static readonly Fix64Velocity OneLeft = new Fix64Velocity(Fix64.One, (Fix64)180L);
        /// <summary>Speed = 1. Angle = 270</summary>
        public static readonly Fix64Velocity OneDown = new Fix64Velocity(Fix64.One, (Fix64)270L);

        public Fix64 Speed;
        public Fix64 Angle;

        /// <summary>Speed and direction translated to Vector2 velocity</summary>
        public Fix64Vector2 LinearVelocity
        {
            get
            {
                Fix64 rad = Fix64.ToRadians(Angle);
                return new Fix64Vector2(Speed * Fix64.Cos(rad), -(Speed * Fix64.Sin(rad)));
            }
            set
            {
                Speed = Fix64Vector2.Zero.DistanceTo(value);
                Angle = Fix64Vector2.Zero.AngleTo(value);
            }
        }
        #endregion

        #region Constructors
        public Fix64Velocity(Fix64 speed, Fix64 angle)
        {
            Speed = speed;
            Angle = angle;
        }
        public Fix64Velocity(Fix64Vector2 linear)
        {
            Speed = Fix64Vector2.Zero.DistanceTo(linear);
            Angle = Fix64Vector2.Zero.AngleTo(linear);
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            return (obj is Fix64Velocity) ? this == ((Fix64Velocity)obj) : false;
        }
        public bool Equals(Fix64Velocity other)
        {
            return this == other;
        }
        public override int GetHashCode()
        {
            return (int)(Speed + Angle);
        }
        public override string ToString()
        {
            return "[Velocity] (Speed) " + Speed + " (Direction) " + Angle + " (HVVelocity) " + LinearVelocity.ToString();
        }
        #endregion

        #region Operator
        public static bool operator ==(Fix64Velocity value1, Fix64Velocity value2)
        {
            return (value1.Speed == value2.Speed && value1.Angle == value2.Angle);
        }
        public static bool operator !=(Fix64Velocity value1, Fix64Velocity value2)
        {
            return (value1.Speed != value2.Speed || value1.Angle != value2.Angle);
        }
        #endregion
    }
}
