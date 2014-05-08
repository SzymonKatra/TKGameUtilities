using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using TKGameUtilities;

namespace TKGameUtilities.FixedPointMath
{
    public struct Fix64Rectangle
    {
        #region Constructors
        public Fix64Rectangle(Fix64Vector2 position, Fix64Vector2 size)
        {
            this.Position = position;
            this.Size = size;
        }
        #endregion

        #region Properties
        public Fix64Vector2 Position;
        public Fix64Vector2 Size;
        #endregion

        #region Methods
        /// <summary>
        /// Tells wheter this rectangle is equals to other rectangle
        /// </summary>
        /// <param name="other">Other rectangle</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(Fix64Rectangle other)
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
            return ((other is Fix64Rectangle) ? Equals((Fix64Rectangle)other) : false);
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
        /// FPRectangle corners
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
        public static bool operator ==(Fix64Rectangle value1, Fix64Rectangle value2)
        {
            return (value1.Position == value2.Position && value1.Size == value2.Size);
        }
        /// <summary>
        /// Tells wheter first rectangle don't equals to second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(Fix64Rectangle value1, Fix64Rectangle value2)
        {
            return (value1.Position != value2.Position || value1.Size != value2.Size);
        }

        public static explicit operator Rectangle(Fix64Rectangle rectangle)
        {
            return new Rectangle((Vector2)rectangle.Position, (Vector2)rectangle.Size);
        }
        public static explicit operator Fix64Rectangle(Rectangle rectangle)
        {
            return new Fix64Rectangle((Fix64Vector2)rectangle.Position, (Fix64Vector2)rectangle.Size);
        }

        public static explicit operator RectangleInt(Fix64Rectangle rectangle)
        {
            return new RectangleInt((Point2)rectangle.Position, (Point2)rectangle.Size);
        }
        public static explicit operator Fix64Rectangle(RectangleInt rectangle)
        {
            return new Fix64Rectangle((Fix64Vector2)rectangle.Position, (Fix64Vector2)rectangle.Size);
        }
        #endregion
    }
}

