using System;
using System.Runtime.InteropServices;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Axis aligned
    /// Used for example to collision detection
    /// </summary>
    public struct Rectangle :  IEquatable<Rectangle>
    {
        #region Properties
        #region Constant
        /// <summary>
        /// Rectangle with all zero components
        /// </summary>
        public static readonly Rectangle Zero = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// Rectangle with all one components
        /// </summary>
        public static readonly Rectangle One = new Rectangle(1, 1, 1, 1);
        #endregion
        /// <summary> Position of rectangle </summary>
        public Vector2 Position;
        /// <summary> Center of rectangle </summary>
        public Vector2 Center
        {
            get
            {
                return (Size / 2) + Position;
            }
            set
            {
                Position = value - (Size / 2);
            }
        }
        /// <summary> Size of rectangle </summary>
        public Vector2 Size;
        /// <summary> Left of rectangle </summary>
        public float Left
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position.X = value;
            }
        }
        /// <summary> Top of rectangle </summary>
        public float Top
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position.Y = value;
            }
        }
        /// <summary> Right of rectangle </summary>
        public float Right
        {
            get
            {
                return Position.X + Size.X;
            }
            set
            {
                Size.X = value - Position.X;
            }
        }
        /// <summary> Bottom of rectangle </summary>
        public float Bottom
        {
            get
            {
                return Position.Y + Size.Y;
            }
            set
            {
                Size.Y = value - Position.Y;
            }
        }

        #region Corners
        /// <summary> Top-left corner of rectangle </summary>
        public Vector2 TopLeftCorner
        {
            get
            {
                return Position;
            }
        }
        /// <summary> Top-right corner of rectangle </summary>
        public Vector2 TopRightCorner
        {
            get
            {
                return new Vector2(Position.X + Size.X, Position.Y);
            }
        }
        /// <summary> Bottom-left corner of rectangle </summary>
        public Vector2 BottomLeftCorner
        {
            get
            {
                return new Vector2(Position.X, Position.Y + Size.Y);
            }
        }
        /// <summary> Bottom-right corner of rectangle </summary>
        public Vector2 BottomRightCorner
        {
            get
            {
                return new Vector2(Position.X + Size.X, Position.Y + Size.Y);
            }
        }
        /// <summary>
        /// All corners of rectangle in order: 
        /// top-left, top-right, bottom-right, bottom-left.
        /// </summary>
        public Vector2[] AllCorners
        {
            get
            {
                Vector2[] ret = new Vector2[4];
                ret[0] = TopLeftCorner;
                ret[1] = TopRightCorner;
                ret[2] = BottomRightCorner;
                ret[3] = BottomLeftCorner;
                return ret;
            }
        }
        /// <summary>
        /// All corners of rectangle in order:
        /// top-left, bottom-left, bottom-right, top-right
        /// </summary>
        public Vector2[] InverseAllCorners
        {
            get
            {
                Vector2[] ret = new Vector2[4];
                ret[0] = TopLeftCorner;
                ret[1] = BottomLeftCorner;
                ret[2] = BottomRightCorner;
                ret[3] = TopRightCorner;
                return ret;
            }
        }
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Construct rectangle from specified values
        /// </summary>
        /// <param name="left">Left of rectangle</param>
        /// <param name="top">Top of rectangle</param>
        /// <param name="right">Right of rectangle</param>
        /// <param name="bottom">Bottom of rectangle</param>
        public Rectangle(float left, float top, float right, float bottom)
            : this(new Vector2(left, top), new Vector2(right - left, bottom - top))
        {
        }
        /// <summary>
        /// Construct rectangle from specified values
        /// </summary>
        /// <param name="position">Position of rectangle</param>
        /// <param name="size">Size of rectangle</param>
        public Rectangle(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks the rectangle overlaps with other rectangle
        /// </summary>
        /// <param name="other">Other rectangle to check</param>
        /// <returns>True if rectangles are overlapping</returns>
        public bool Intersects(Rectangle other)
        {
            return (Math.Max(this.Position.X, other.Position.X) < Math.Min(this.Right, other.Right)) && (Math.Max(this.Position.Y, other.Position.Y) < Math.Min(this.Bottom, other.Bottom));
        }
        /// <summary>
        /// Checks the rectangle overlaps with other circle
        /// </summary>
        /// <param name="other">Other circle to check</param>
        /// <returns>True if rectangle and circle are overlapping</returns>
        public bool Intersects(Circle other)
        {
            //source: http://forums.tigsource.com/index.php?topic=26092.0 - Glaiel-Gamer
            Vector2 pt = other.Center;
            if (pt.X > this.Right) pt.X = this.Right;
            if (pt.X < this.Left) pt.X = this.Left;
            if (pt.Y > this.Bottom) pt.Y = this.Bottom;
            if (pt.Y < this.Top) pt.Y = this.Top;
            return (GameMath.PointDistance(pt, other.Center) < other.Radius);
        }
        /// <summary>
        /// Checks the point is inside rectangle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point inside the rectangle</returns>
        public bool Contains(Vector2 point)
        {
            return (point.X >= this.Left) && (point.X <= this.Right) && (point.Y >= this.Top) && (point.Y <= this.Bottom);
        }
        /// <summary>
        /// Creates a Rectangle defining the area where first rectangle intersects with second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>Result</returns>
        public static Rectangle Intersect(Rectangle value1, Rectangle value2)
        {
            Rectangle rectangle;
            Intersect(ref value1, ref value2, out rectangle);
            return rectangle;
        }
        /// <summary>
        /// Creates a Rectangle defining the area where first rectangle intersects with second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <param name="result">Result</param>
        public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            if (value1.Intersects(value2))
            {
                float right_side = Math.Min(value1.Right, value2.Right);
                float left_side = Math.Max(value1.Left, value2.Left);
                float top_side = Math.Max(value1.Top, value2.Top);
                float bottom_side = Math.Min(value1.Bottom, value2.Bottom);
                result = new Rectangle(left_side, top_side, right_side - left_side, bottom_side - top_side);
            }
            else
            {
                result = new Rectangle(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Tells wheter this rectangle is equals to other rectangle
        /// </summary>
        /// <param name="other">Other rectangle</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(Rectangle other)
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
            return ((other is Rectangle) ? Equals((Rectangle)other) : false);
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
        #endregion

        #region Operators
        /// <summary>
        /// Tells wheter first rectangle is equals to second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool operator ==(Rectangle value1, Rectangle value2)
        {
            return (value1.Position == value2.Position && value1.Size == value2.Size);
        }
        /// <summary>
        /// Tells wheter first rectangle don't equals to second rectangle
        /// </summary>
        /// <param name="value1">First rectangle</param>
        /// <param name="value2">Second rectangle</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(Rectangle value1, Rectangle value2)
        {
            return (value1.Position != value2.Position || value1.Size != value2.Size);
        }
        #endregion
    }
}
