using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using TKGameUtilities;

namespace TKGameUtilities.FixedPointMath
{
    public struct Fix64Vector2 : IEquatable<Fix64Vector2>
    {
        public static readonly Fix64Vector2 Zero = new Fix64Vector2(Fix64.Zero, Fix64.Zero);
        public static readonly Fix64Vector2 One = new Fix64Vector2((Fix64)1L, (Fix64)1L);

        public Fix64 X;
        public Fix64 Y;

        public Fix64Vector2(Fix64 x, Fix64 y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Fix64Vector2 other)
        {
            return (X == other.X && Y == other.Y);
        }
        public override bool Equals(object obj)
        {
            return Equals((Fix64Vector2)obj);
        }
        public override int GetHashCode()
        {
            return (int)(X + Y);
        }

        public static Fix64Vector2 operator -(Fix64Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }
        public static bool operator ==(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            return (value1.X == value2.X && value1.Y == value2.Y);
        }
        public static bool operator !=(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            return (value1.X != value2.X || value1.Y != value2.Y);
        }
        public static Fix64Vector2 operator +(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }
        public static Fix64Vector2 operator -(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }
        public static Fix64Vector2 operator *(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }
        public static Fix64Vector2 operator *(Fix64Vector2 value, Fix64 amount)
        {
            value.X *= amount;
            value.Y *= amount;
            return value;
        }
        public static Fix64Vector2 operator /(Fix64Vector2 value1, Fix64Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }
        public static Fix64Vector2 operator /(Fix64Vector2 value, Fix64 amount)
        {
            value.X /= amount;
            value.Y /= amount;
            return value;
        }

        public Fix64 DistanceTo(Fix64Vector2 b)
        {
            return (Fix64.Sqrt((b.X - this.X) * (b.X - this.X) + (b.Y - this.Y) * (b.Y - this.Y)));
        }
        public Fix64 AngleTo(Fix64Vector2 b)
        {
            return Fix64.ToDegress(Fix64.Atan2((this.Y - b.Y), (b.X - this.X))) % (Fix64)360;
        }
        public Fix64 AngleToRad(Fix64Vector2 b)
        {
            return Fix64.Atan2((this.Y - b.Y), (b.X - this.X)) % Fix64.PiTimes2;
        }

        public static explicit operator Vector2(Fix64Vector2 value)
        {
            return new Vector2((float)value.X, (float)value.Y);
        }
        public static explicit operator Fix64Vector2(Vector2 value)
        {
            return new Fix64Vector2((Fix64)value.X, (Fix64)value.Y);
        }

        public static explicit operator Point2(Fix64Vector2 value)
        {
            return new Point2((int)value.X, (int)value.Y);
        }
        public static explicit operator Fix64Vector2(Point2 value)
        {
            return new Fix64Vector2((Fix64)value.X, (Fix64)value.Y);
        }

        public override string ToString()
        {
            return string.Format("X: {0} Y: {1}", X, Y);
        }
    }
}
