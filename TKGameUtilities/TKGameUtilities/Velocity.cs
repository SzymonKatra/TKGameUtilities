using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities
{
    public struct Velocity : IEquatable<Velocity>
    {
        #region Properties
        /// <summary>Speed = 0. Angle = 0</summary>
        public static readonly Velocity Zero = new Velocity(0, 0);

        /// <summary>Speed = 1. Angle = 0</summary>
        public static readonly Velocity OneRight = new Velocity(1f, 0f);
        /// <summary>Speed = 1. Angle = 90</summary>
        public static readonly Velocity OneUp = new Velocity(1f, 90f);
        /// <summary>Speed = 1. Angle = 180</summary>
        public static readonly Velocity OneLeft = new Velocity(1f, 180f);
        /// <summary>Speed = 1. Angle = 270</summary>
        public static readonly Velocity OneDown = new Velocity(1f, 270f);

        public float Speed;
        public float Angle;

        /// <summary>Speed and direction translated to Vector2 velocity</summary>
        public Vector2 LinearVelocity
        {
            get
            {
                return new Vector2(Speed * GameMath.FCos(Angle), -(Speed * GameMath.FSin(Angle)));
            }
            set
            {
                Speed = Vector2.Zero.DistanceTo(value);
                Angle = Vector2.Zero.AngleTo(value);
            }
        }
        #endregion

        #region Constructors
        public Velocity(float speed, float angle)
        {
            Speed = speed;
            Angle = angle;
        }
        public Velocity(Vector2 linear)
        {
            Speed = Vector2.Zero.DistanceTo(linear);
            Angle = Vector2.Zero.AngleTo(linear);
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            return (obj is Velocity) ? this == ((Velocity)obj) : false;
        }
        public bool Equals(Velocity other)
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
        public static bool operator ==(Velocity value1, Velocity value2)
        {
            return (value1.Speed == value2.Speed && value1.Angle == value2.Angle);
        }
        public static bool operator !=(Velocity value1, Velocity value2)
        {
            return (value1.Speed != value2.Speed || value1.Angle != value2.Angle);
        }
        #endregion
    }
}
