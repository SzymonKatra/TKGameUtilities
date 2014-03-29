using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities
{
    public struct RectangleInt
    {
        #region Constructors
        public RectangleInt(Point2 position, Point2 size)
        {
            this.Position = position;
            this.Size = size;
        }
        #endregion

        #region Properties
        public Point2 Position;
        public Point2 Size;
        #endregion

        #region Methods
        /// <summary>
        /// Tells wheter this rectangle is equals to other rectangle
        /// </summary>
        /// <param name="other">Other rectangle</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(RectangleInt other)
        {
            return (Position == other.Position && Size == other.Size);
        }
        /// <summary>
        /// Tells wheter this rectangle is equals to other object
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>True if equals, otherwise false</returns>
        public override bool Equals(object other)
        {
            return ((other is RectangleInt) ? Equals((RectangleInt)other) : false);
        }
        /// <summary>
        /// Gets hash code that represents current object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return (int)(Position.GetHashCode() + Size.GetHashCode());
        }
        /// <summary>
        /// Rectangle corners
        /// </summary>
        /// <returns>Corners of rectangle</returns>
        public override string ToString()
        {
            return "POSITION: " + Position.ToString() + " SIZE: " + Size.ToString();
        }

        /// <summary>
        /// Tells wheter first rectangle is equals to second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool operator ==(RectangleInt value1, RectangleInt value2)
        {
            return (value1.Position == value2.Position && value1.Size == value2.Size);
        }
        /// <summary>
        /// Tells wheter first rectangle don't equals to second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(RectangleInt value1, RectangleInt value2)
        {
            return (value1.Position != value2.Position || value1.Size != value2.Size);
        }

        public static explicit operator Rectangle(RectangleInt rectangle)
        {
            return new Rectangle((Vector2)rectangle.Position, (Vector2)rectangle.Size);
        }
        public static explicit operator RectangleInt(Rectangle rectangle)
        {
            return new RectangleInt((Point2)rectangle.Position, (Point2)rectangle.Size);
        }
        #endregion
    }
}
