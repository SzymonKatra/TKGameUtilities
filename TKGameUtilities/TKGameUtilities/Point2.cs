using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKGameUtilities
{
    public struct Point2 : IEquatable<Point2>
    {
        public int X;
        public int Y;

        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point2 other)
        {
            return (X == other.X && Y == other.Y);
        }
        public override bool Equals(object obj)
        {
            return Equals((Point2)obj);
        }
        public override int GetHashCode()
        {
            return X + Y;
        }

        public static Point2 operator -(Point2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }
        public static bool operator ==(Point2 value1, Point2 value2)
        {
            return (value1.X == value2.X && value1.Y == value2.Y);
        }
        public static bool operator !=(Point2 value1, Point2 value2)
        {
            return (value1.X != value2.X || value1.Y != value2.Y);
        }
        public static Point2 operator +(Point2 value1, Point2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }
        public static Point2 operator -(Point2 value1, Point2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }
        public static Point2 operator *(Point2 value1, Point2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }
        public static Point2 operator *(Point2 value, int amount)
        {
            value.X *= amount;
            value.Y *= amount;
            return value;
        }
        public static Point2 operator /(Point2 value1, Point2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }
        public static Point2 operator /(Point2 value, int amount)
        {
            value.X /= amount;
            value.Y /= amount;
            return value;
        }

        public static explicit operator Vector2(Point2 value)
        {
            return new Vector2(value.X, value.Y);
        }
        public static explicit operator Point2(Vector2 value)
        {
            return new Point2((int)value.X, (int)value.Y);
        }
    }
}
