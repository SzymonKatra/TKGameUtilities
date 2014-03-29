using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Class that represents triangle.
    /// </summary>
    public class PolygonTriangle : Polygon
    {
        /// <summary>
        /// Point A.
        /// </summary>
        public Vector2 A
        {
            get { return Points[0]; }
            set { Points[0] = value; }
        }
        /// <summary>
        /// Point B.
        /// </summary>
        public Vector2 B
        {
            get { return Points[1]; }
            set { Points[1] = value; }
        }
        /// <summary>
        /// Point C.
        /// </summary>
        public Vector2 C
        {
            get { return Points[2]; }
            set { Points[2] = value; }
        }

        /// <summary>
        /// Construct empty triangle.
        /// </summary>
        public PolygonTriangle()
            : this(Vector2.Zero, Vector2.Zero, Vector2.Zero)
        {
        }
        /// <summary>
        /// Construct triangle with points.
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <param name="c">Point C</param>
        public PolygonTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            Points = new List<Vector2>(3);
            Points.Add(a);
            Points.Add(b);
            Points.Add(c);
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="polygonTriangle">Polygon triangle</param>
        protected PolygonTriangle(PolygonTriangle polygonTriangle)
            : base(polygonTriangle)
        {
        }

        /// <summary>
        /// Clone polygon triangle
        /// </summary>
        /// <returns>Cloned polygon triangle</returns>
        public override Polygon Clone()
        {
            return new PolygonTriangle(this);
        }
    }
}
