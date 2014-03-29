using System;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Circle
    /// Used for exapmle to detect collision
    /// </summary>
    public struct Circle : IEquatable<Circle>
    {
        #region Properties
        /// <summary>
        /// Circle with zero components
        /// </summary>
        public static readonly Circle Zero = new Circle(new Vector2(0, 0), 0);

        /// <summary>
        /// Radius of circle
        /// </summary>
        public float Radius;
        /// <summary>
        /// Center of circle
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// Position of circle
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(Center.X - Radius, Center.Y - Radius);
            }
            set
            {
                Center = new Vector2(value.X + Radius, value.Y + Radius);
            }
        }
        /// <summary>
        /// AABB of circle
        /// </summary>
        public Rectangle AABB
        {
            get
            {
                return new Rectangle(Position, new Vector2(Radius) * 2);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct circle from position and radius.
        /// If you want to create circle from top-left corner position use  "Circle.CreateFromCenter(position + radius, radius);" method.
        /// </summary>
        /// <param name="position">Center</param>
        /// <param name="radius">Radius of circle</param>
        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create circle from position (top-left) and radius.
        /// If you want construct circle from center and radius use constructor
        /// </summary>
        /// <param name="center">Center position of circle</param>
        /// <param name="radius">Radius of circle</param>
        /// <returns>Circle</returns>
        public static Circle CreateFromPosition(Vector2 position, float radius)
        {
            return new Circle(new Vector2(position.X + radius, position.Y + radius), radius);
        }
        /// <summary>
        /// Checks the circle overlaps with other circle
        /// </summary>
        /// <param name="other">Other circle to check</param>
        /// <returns>True if circles are overlapping</returns>
        public bool Intersects(Circle other)
        {
            return (GameMath.PointDistance(this.Center, other.Center) <= this.Radius + other.Radius);
        }
        /// <summary>
        /// Checks the circle overlaps with other rectangle
        /// </summary>
        /// <param name="other">Other rectangle to check</param>
        /// <returns>True if circle and rectangle are overlapping</returns>
        public bool Intersects(Rectangle other)
        {
            return other.Intersects(this);
        }
        /// <summary>
        /// Checks the point is inside circle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point inside the circle</returns>
        public bool IsPointInside(Vector2 point)
        {
            return (GameMath.PointDistance(this.Center, point) <= this.Radius);
        }

        /// <summary>
        /// Tells wheter this circle is equals to other circle
        /// </summary>
        /// <param name="other">Other circle</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(Circle other)
        {
            return (Radius == other.Radius && Center == other.Center);
        }
        /// <summary>
        /// Tells wheter this circle is equals to other object
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>True if equals, otherwise false</returns>
        public override bool Equals(object other)
        {
            return ((other is Circle) ? Equals((Circle)other) : false);
        }
        /// <summary>
        /// Gets hash code that represents current object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return (int)(Radius.GetHashCode() + Center.GetHashCode());
        }
        /// <summary>
        /// To string
        /// </summary>
        /// <returns>String that represents circle</returns>
        public override string ToString()
        {
            return "CENTER: " + Center.ToString() + " RADIUS: " + Radius;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Tells wheter first circle is equals to second circle
        /// </summary>
        /// <param name="value1">First circle</param>
        /// <param name="value2">Second circle</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool operator ==(Circle value1, Circle value2)
        {
            return (value1.Radius == value2.Radius && value1.Center == value2.Center);
        }
        /// <summary>
        /// Tells wheter first circle don't equals to circle rectangle
        /// </summary>
        /// <param name="value1">First circle</param>
        /// <param name="value2">Second circle</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(Circle value1, Circle value2)
        {
            return (value1.Radius != value2.Radius || value1.Center != value2.Center);
        }
        #endregion
    }
}
