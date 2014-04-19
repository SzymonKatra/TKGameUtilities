//THANKS TO Laurent Cozic from http://www.codeproject.com/
//http://www.codeproject.com/Articles/15573/2D-Polygon-Collision-Detection
//AND: http://blog.csharphelper.com/2010/01/04/triangulate-a-polygon-in-c.aspx
//AND: http://xion.org.pl/2008/04/27/podzial-wielokatow-na-trojkaty/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Class that represents polygon
    /// </summary>
    public class Polygon : IEnumerable<Vector2>, ICloneable<Polygon>
    {
        #region Properties
        private List<Vector2> m_points = new List<Vector2>();
        private List<Vector2> m_edges = new List<Vector2>();
        private Vector2 m_position = Vector2.Zero;
        private Vector2 m_origin = Vector2.Zero;
        private float m_rotation = 0f;
        private List<PolygonTriangle> m_triangles = null; 
        private bool m_edgesNeedUpdate = true;
        private bool m_trianglesNeedUpdate = true;
        private bool m_isConvex = false;
        private bool m_isConvexNeedUpdate = true;
        private bool m_isOrientedClockwise = false;
        private bool m_isOrientedClockwiseNeedUpdate = true;
        
        /// <summary>
        /// Edges of polygon
        /// </summary>
        protected List<Vector2> Edges
        {
            get { return m_edges; }
        }
        /// <summary>
        /// Points of polygon
        /// </summary>
        protected List<Vector2> Points
        {
            get { return m_points; }
            set { m_points = value; }
        }
        /// <summary>
        /// Triangles in polygon
        /// </summary>
        protected internal List<PolygonTriangle> Triangles
        {
            get { return m_triangles; }
            protected set { m_triangles = value; }
        }
        /// <summary>
        /// Center of polygon
        /// </summary>
        public Vector2 Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < m_points.Count; i++)
                {
                    totalX += m_points[i].X;
                    totalY += m_points[i].Y;
                }

                return new Vector2(totalX / (float)m_points.Count, totalY / (float)m_points.Count);
            }
        }
        /// <summary>
        /// Position of polygon
        /// </summary>
        public Vector2 Position
        {
            get { return m_position; }
            set
            {
                Vector2 offset = value - m_position;
                m_position = value;

                Move(offset);
            }
        }
        /// <summary>
        /// Origin of polygon
        /// </summary>
        public Vector2 Origin
        {
            get { return m_origin; }
            set
            {
                Vector2 offset = value - m_origin;
                m_origin = value;

                Move(-offset);
            }
        }
        /// <summary>
        /// Rotation of polygon
        /// </summary>
        public float Rotation
        {
            get { return m_rotation; }
            set
            {
                float rotRad = GameMath.ToRadians(GameMath.ReduceAngle(value - m_rotation));
                m_rotation = GameMath.ReduceAngle(value);
                Vector2 rotateRelative = m_position;// +_origin;

                for (int i = 0; i < m_points.Count; ++i)
                {
                    //_points[i].RotateRad(rotRad, rotateRelative);
                    m_points[i] = GameMath.RotateRad(m_points[i], rotRad, rotateRelative);
                    //_points[i] = Vector2.Rotate(_points[i], rotRad, rotateRelative);
                }
                if (m_triangles != null)
                {
                    for (int i = 0; i < m_triangles.Count; i++)
                    {
                        for (int j = 0; j < m_triangles[i].m_points.Count; j++)
                        {
                            //_triangles[i]._points[j].RotateRad(rotRad, rotateRelative);
                            m_triangles[i].m_points[j] = GameMath.RotateRad(m_triangles[i].m_points[j], rotRad, rotateRelative);
                            //_triangles[i]._points[j] = Vector2.Rotate(_triangles[i]._points[j], rotRad, rotateRelative);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets wheter the polygon is oriented clockwise.
        /// </summary>
        public bool OrientedClockwise
        {
            get
            {
                if (m_isOrientedClockwiseNeedUpdate)
                {
                    m_isOrientedClockwise = IsOrientedClockwise(m_points);
                    m_isOrientedClockwiseNeedUpdate = false;
                }
                return m_isOrientedClockwise;
                //return IsOrientedClockwise(_points);
            }
            set
            {
                if (value)
                    OrientClockwise(m_points);
                else
                    OrientCounterClockwise(m_points);
            }
        }
        /// <summary>
        /// Gets wheter the polygon is convex.
        /// </summary>
        public bool Convex
        {
            get
            {
                if (m_isConvexNeedUpdate) m_isConvex = IsConvex(m_points);
                return m_isConvex;
            }
        }
        /// <summary>
        /// Count of points in polygon
        /// </summary>
        public int PointCount
        {
            get { return m_points.Count; }
        }
        /// <summary>
        /// If polygon is concave it returns number of triangles in polygon which he have from triangulation
        /// </summary>
        public int TrianglesCount
        {
            get
            {
                if (!Convex)
                {
                    if (m_trianglesNeedUpdate)
                    {
                        m_triangles = Triangulate(m_points);
                        m_trianglesNeedUpdate = false;
                    }
                }
                return (m_triangles == null ? -1 : m_triangles.Count);
            }
        }
        /// <summary>
        /// Gets or sets specified point of polygon.
        /// </summary>
        /// <param name="index">Index of point</param>
        /// <returns>Specified point</returns>
        public Vector2 this[int index]
        {
            get
            {
                return m_points[index];
            }
            set
            {
                m_points[index] = value;
                NeedUpdateReset();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Construct empty polygon
        /// </summary>
        public Polygon()
        {
        }
        /// <summary>
        /// Construct polygon from a rectangle
        /// </summary>
        /// <param name="rectangle">Rectangle</param>
        public Polygon(Rectangle rectangle)
        {
            m_points.Add(Vector2.Zero);
            m_points.Add(new Vector2(rectangle.Size.X, 0));
            m_points.Add(rectangle.Size);
            m_points.Add(new Vector2(0, rectangle.Size.Y));

            Position = rectangle.Position;
        }
        /// <summary>
        /// Construct polygon from a line
        /// </summary>
        /// <param name="line">Line</param>
        public Polygon(Line line)
        {
            m_points.Add(Vector2.Zero);
            m_points.Add(line.End - line.Start);

            Position = line.Start;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="polygon">Polygon</param>
        protected Polygon(Polygon polygon)
        {
            this.m_points = new List<Vector2>(polygon.m_points);
            this.m_edges = new List<Vector2>(polygon.m_edges);
            this.m_position = polygon.m_position;
            this.m_origin = polygon.m_origin;
            this.m_rotation = polygon.m_rotation;
            this.m_triangles = null;
            if (polygon.m_triangles != null)
            {
                this.m_triangles = new List<PolygonTriangle>();
                foreach (PolygonTriangle tri in polygon.m_triangles)
                {
                    this.m_triangles.Add((PolygonTriangle)tri.Clone());
                }
            }
            this.m_edgesNeedUpdate = polygon.m_edgesNeedUpdate;
            this.m_trianglesNeedUpdate = polygon.m_trianglesNeedUpdate;
            this.m_isConvex = polygon.m_isConvex;
            this.m_isConvexNeedUpdate = polygon.m_isConvexNeedUpdate;
            this.m_isOrientedClockwise = polygon.m_isOrientedClockwise;
            this.m_isOrientedClockwiseNeedUpdate = polygon.m_isOrientedClockwiseNeedUpdate;
        }
        #endregion

        #region Methods
        #region Collisions
        /// <summary>
        /// Check if this polygon collides with other.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <returns>True if collides, otherwise false</returns>
        public bool Collision(Polygon polygon)
        {
            Vector2 mtv;
            return Collision(polygon, out mtv);
        }
        /// <summary>
        /// Check if this polygon collides with other.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <param name="mtv">Minimum translation vector</param>
        /// <returns>True if collides, otherwise false</returns>
        public bool Collision(Polygon polygon, out Vector2 mtv)
        {
            mtv = Vector2.Zero;
            if (Convex)
            {
                if (polygon.Convex)
                {
                    return ConvexCollision(polygon, out mtv); // we are convex and other is convex
                }
                else
                {
                    return OtherTrianglesCollision(polygon, out mtv); // we are convex but other is concave
                }
            }
            else
            {
                if (polygon.Convex)
                {
                    return MyTrianglesCollision(polygon, out mtv); // we are concave but other is convex
                }
                else
                {
                    return AllTrianglesCollision(polygon, out mtv); // we are concave and other is concave
                }
            }
            
        }
        /// <summary>
        /// Check collision. Passed polygon must be convex.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <param name="mtv">Minimum translation vector</param>
        /// <returns>True if collides, otherwise false</returns>
        protected virtual bool ConvexCollision(Polygon polygon, out Vector2 mtv)
        {
            if (m_edgesNeedUpdate)
            {
                BuildEdges();
                m_edgesNeedUpdate = false;
            }
            if (polygon.m_edgesNeedUpdate)
            {
                polygon.BuildEdges();
                polygon.m_edgesNeedUpdate = false;
            }

            int edgeCountA = this.Edges.Count;
            int edgeCountB = polygon.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;
            mtv = Vector2.Zero;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = this.Edges[edgeIndex];
                }
                else
                {
                    edge = polygon.Edges[edgeIndex - edgeCountA];
                }

                //INTERSECT

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, this, ref minA, ref maxA);
                ProjectPolygon(axis, polygon, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) return false;


                ////WILL INTERSECT

                //// Project the velocity on the current axis
                //float velocityProjection = Vector2.Dot(axis, velocity);

                //// Pop the projection of polygon A during the movement
                //if (velocityProjection < 0)
                //{
                //    minA += velocityProjection;
                //}
                //else
                //{
                //    maxA += velocityProjection;
                //}

                //// Do the same test as above for the new projection
                //float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                //if (intervalDistance > 0) result.WillIntersect = false;

                //// If the polygons are not intersecting and won't intersect, exit the loop
                //if (!result.Intersect && !result.WillIntersect) break;

                //// Check if the current interval distance is the minimum one. If so store
                //// the interval distance and the current distance.
                //// This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = this.Center - polygon.Center;
                    if (Vector2.Dot(d, translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            mtv = translationAxis * minIntervalDistance;

            return true;
        }
        /// <summary>
        /// Check collision. Us polygon will be checked as convex polygon, other will be checked by triangles.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <param name="mtv">Minimum translation vector</param>
        /// <returns>True if collides, otherwise false</returns>
        protected bool OtherTrianglesCollision(Polygon polygon, out Vector2 mtv)
        {
            mtv = Vector2.Zero;

            //if (_edgesNeedUpdate)
            //{
            //    BuildEdges();
            //    _edgesNeedUpdate = false;
            //}

            if (polygon.m_trianglesNeedUpdate)
            {
                polygon.m_triangles = Triangulate(polygon.m_points);
                polygon.m_trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle tri in polygon.m_triangles)
            {
                if (this.ConvexCollision(tri, out mtv))
                {
                    collision = true;
                    mtvSum += mtv;
                }
            }

            mtv = mtvSum;
            return collision;
        }
        /// <summary>
        /// Check collision. Us polygon will be checked by triangles, other will be checked as convex.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <param name="mtv">Minimum translation vector</param>
        /// <returns>True if collides, otherwise false</returns>
        protected bool MyTrianglesCollision(Polygon polygon, out Vector2 mtv)
        {
            mtv = Vector2.Zero;

            //if (polygon._edgesNeedUpdate)
            //{
            //    polygon.BuildEdges();
            //    polygon._edgesNeedUpdate = false;
            //}

            if (m_trianglesNeedUpdate)
            {
                m_triangles = Triangulate(m_points);
                m_trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle tri in m_triangles)
            {
                if (tri.ConvexCollision(polygon, out mtv))
                {
                    collision = true;
                    mtvSum += mtv;
                }
            }

            mtv = mtvSum;
            return collision;
        }
        /// <summary>
        /// Check collision. Us and other polygon will be checked by triangles.
        /// </summary>
        /// <param name="polygon">Other polygon</param>
        /// <param name="mtv">Minimum translation vector</param>
        /// <returns>True if collides, otherwise false</returns>
        protected bool AllTrianglesCollision(Polygon polygon, out Vector2 mtv)
        {
            mtv = Vector2.Zero;

            if (m_trianglesNeedUpdate)
            {
                m_triangles = Triangulate(m_points);
                m_trianglesNeedUpdate = false;
            }

            if (polygon.m_trianglesNeedUpdate)
            {
                polygon.m_triangles = Triangulate(polygon.m_points);
                polygon.m_trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle myTri in m_triangles)
            {
                foreach (PolygonTriangle otherTri in polygon.m_triangles)
                {
                    if (myTri.ConvexCollision(otherTri, out mtv))
                    {
                        collision = true;
                        mtvSum += mtv;
                    }
                }
            }

            mtv = mtvSum;
            return collision;
        }

        /// <summary>
        /// Calculate the distance between [minA, maxA] and [minB, maxB]
        /// The distance will be negative if the intervals overlap
        /// </summary>
        /// <param name="minA">Min A</param>
        /// <param name="maxA">Max A</param>
        /// <param name="minB">Min B</param>
        /// <param name="maxB">Max B</param>
        /// <returns>Result</returns>
        protected float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }
        /// <summary>
        /// Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        /// </summary>
        /// <param name="axis">Axis</param>
        /// <param name="polygon">Polygon</param>
        /// <param name="min">Minimum interval</param>
        /// <param name="max">Maximum interval</param>
        protected void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = Vector2.Dot(axis, polygon.Points[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = Vector2.Dot(polygon.Points[i], axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }
        #endregion

        #region PropertiesAndOtherChecks
        /// <summary>
        /// Add point to the polygon.
        /// </summary>
        /// <param name="point">Point</param>
        public void AddPoint(Vector2 point)
        {
            m_points.Add(point);
            NeedUpdateReset();
        }
        /// <summary>
        /// Gets specified point of polygon.
        /// </summary>
        /// <param name="index">Index of point</param>
        /// <returns>Specified point</returns>
        public Vector2 GetPoint(int index)
        {
            return m_points[index];
        }
        /// <summary>
        /// Set specified point of polygon.
        /// </summary>
        /// <param name="index">Index of point</param>
        /// <param name="point">Point</param>
        public void SetPoint(int index, Vector2 point)
        {
            m_points[index] = point;
            NeedUpdateReset();
        }
        /// <summary>
        /// Gets specified copy of triangle of polygon
        /// </summary>
        /// <param name="index">Index of triangle</param>
        /// <returns>Copy of triangle</returns>
        public PolygonTriangle GetTriangle(int index)
        {
            PolygonTriangle orgTri = m_triangles[index];
            PolygonTriangle copy = new PolygonTriangle(orgTri.A, orgTri.B, orgTri.C);
            copy.m_position = m_position;
            copy.m_origin = m_origin;
            copy.m_rotation = m_rotation;
            return copy;
        }
        public Vector2 GetEdge(int index)
        {
            if (m_edgesNeedUpdate)
            {
                BuildEdges();
                m_edgesNeedUpdate = false;
            }
            return m_edges[index];
        }

        /// <summary>
        /// Check polygon is contains passed point
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>True if is in polygon, otherwise false.</returns>
        public bool Contains(Vector2 point)
        {
            if (Convex)
            {
                return ConvexContains(this, point);
            }
            else
            {
                if (m_trianglesNeedUpdate)
                {
                    m_triangles = Triangulate(m_points);
                    m_trianglesNeedUpdate = false;
                }

                foreach (PolygonTriangle tri in m_triangles)
                {
                    if (ConvexContains(tri, point)) return true;
                }

                return false;
            }

            //// Get the angle between the point and the
            //// first and last vertices.
            //int max_point = _points.Count - 1;
            //float total_angle = GameMath.GetAngle(_points[max_point].X, _points[max_point].Y, point.X, point.Y, _points[0].X, _points[0].Y);

            //// Add the angles from the point
            //// to each other pair of vertices.
            //for (int i = 0; i < max_point; i++)
            //{
            //    total_angle += GameMath.GetAngle(_points[i].X, _points[i].Y, point.X, point.Y, _points[i + 1].X, _points[i + 1].Y);
            //}

            //// The total angle should be 2 * PI or -2 * PI if
            //// the point is in the polygon and close to zero
            //// if the point is outside the polygon.
            //return (Math.Abs(total_angle) > 0.000001);
        }
        private static bool ConvexContains(Polygon polygon, Vector2 point)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = polygon.m_points.Count - 1;
            float total_angle = GetAngle(polygon.m_points[max_point].X, polygon.m_points[max_point].Y, point.X, point.Y, polygon.m_points[0].X, polygon.m_points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(polygon.m_points[i].X, polygon.m_points[i].Y, point.X, point.Y, polygon.m_points[i + 1].X, polygon.m_points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }
        #endregion

        #region Other
        /// <summary>
        /// Call this if you changed point in polygon
        /// </summary>
        protected void NeedUpdateReset()
        {
            m_isConvexNeedUpdate = true;
            m_trianglesNeedUpdate = true;
            m_edgesNeedUpdate = true;
            m_isOrientedClockwiseNeedUpdate = true;
        }
        /// <summary>
        /// Build polygon edges
        /// </summary>
        protected void BuildEdges()
        {
            Vector2 p1;
            Vector2 p2;
            m_edges.Clear();
            for (int i = 0; i < m_points.Count; i++)
            {
                p1 = m_points[i];
                if (i + 1 >= m_points.Count)
                {
                    p2 = m_points[0];
                }
                else
                {
                    p2 = m_points[i + 1];
                }
                m_edges.Add(p2 - p1);
            }
        }
        private void Move(Vector2 offset)
        {
            //move points
            for (int i = 0; i < m_points.Count; i++)
            {
                m_points[i] += offset;
            }

            //move triangles if polygon is tringulated
            if (m_triangles != null)
            {
                for (int i = 0; i < m_triangles.Count; i++)
                {
                    for (int j = 0; j < m_triangles[i].Points.Count; j++)
                    {
                        m_triangles[i].Points[j] += offset;
                    }
                }
            }
        }

        public void PerformTriangulate()
        {
            if (m_trianglesNeedUpdate)
            {
                m_triangles = Triangulate(m_points);
                m_trianglesNeedUpdate = false;
            }
        }
        
        /// <summary>
        /// Is polygon convex?
        /// </summary>
        /// <param name="points">Polygon points</param>
        /// <returns>True if convex, otherwise false</returns>
        public static bool IsConvex(List<Vector2> points)
        {
            if (points.Count == 3) return true; //triangles are always convex

            // For each set of three adjacent points A, B, C,
            // find the dot product AB · BC. If the sign of
            // all the dot products is the same, the angles
            // are all positive or negative (depending on the
            // order in which we visit them) so the polygon
            // is convex.
            bool got_negative = false;
            bool got_positive = false;
            int nuPoints = points.Count;
            int B, C;
            for (int A = 0; A < nuPoints; A++)
            {
                B = (A + 1) % nuPoints;
                C = (B + 1) % nuPoints;

                float cross_product = CrossProductLength(points[A].X, points[A].Y, points[B].X, points[B].Y, points[C].X, points[C].Y);
                if (cross_product < 0)
                {
                    got_negative = true;
                }
                else if (cross_product > 0)
                {
                    got_positive = true;
                }
                if (got_negative && got_positive) return false;
            }

            // If we got this far, the polygon is convex.
            return true;
        }
        #endregion

        #region Triangulation
        /// <summary>
        /// Triangulates polygon.
        /// </summary>
        /// <param name="points">Polygon vertexes</param>
        /// <param name="copyList">Copy polygon points list?</param>
        /// <returns>Triangulated polygon</returns>
        public static List<PolygonTriangle> Triangulate(List<Vector2> points, bool copyList = true)
        {
            //THANKS TO: http://blog.csharphelper.com/2010/01/04/triangulate-a-polygon-in-c.aspx
            //AND: http://xion.org.pl/2008/04/27/podzial-wielokatow-na-trojkaty/


            List<PolygonTriangle> triangles = new List<PolygonTriangle>();
            //if (copyList)
            //{
            //    List<Vector2> clonedPoints = new List<Vector2>();
            //    clonedPoints.AddRange(points);
            //    points = clonedPoints;
            //}
            if (copyList) points = new List<Vector2>(points);

            OrientClockwise(points);

            while (points.Count > 3)
            {
                int a = 0;
                int b = 0;
                int c = 0;

                //find ear
                for (a = 0; a < points.Count; a++)
                {
                    b = (a + 1) % points.Count;
                    c = (b + 1) % points.Count;

                    if (IsEar(points, a, b, c)) break;
                }
                //if (a >= points.Count) throw new InvalidOperationException("An error occurred while triangulating polygon!");

                if (a < points.Count) triangles.Add(new PolygonTriangle(points[a], points[b], points[c]));
                points.RemoveAt(b);
            }

            if (points.Count == 3) triangles.Add(new PolygonTriangle(points[0], points[1], points[2]));

            return triangles;
        }
        private static bool IsEar(List<Vector2> points, int a, int b, int c)
        {
            // See if the angle ABC is concave.
            if (GetAngle(points[a].X, points[a].Y, points[b].X, points[b].Y, points[c].X, points[c].Y) > 0)
            {
                // This is a concave corner so the triangle
                // cannot be an ear.
                return false;
            }

            // Make the triangle A, B, C.
            PolygonTriangle triangle = new PolygonTriangle(points[a], points[b], points[c]);

            // Check the other points to see 
            // if they lie in triangle A, B, C.
            for (int i = 0; i < points.Count; i++)
            {
                if ((i != a) && (i != b) && (i != c))
                {
                    if (triangle.Contains(points[i]))
                    {
                        // This point is in the triangle 
                        // do this is not an ear.
                        return false;
                    }
                }
            }

            // This is an ear.
            return true;
        }    
        #endregion

        #region Orientation
        /// <summary>
        /// Is polygon oriented clockwise?
        /// </summary>
        /// <param name="points">Polygon</param>
        /// <returns>True if clockwise, otherwise false</returns>
        public static bool IsOrientedClockwise(List<Vector2> points)
        {
            // Add the first point to the end.
            points.Add(points[0]);

            // Get the areas.
            float area = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                area += (points[i + 1].X - points[i].X) * (points[i + 1].Y + points[i].Y) / 2;
            }

            points.RemoveAt(points.Count - 1);

            return (area < 0);
        }
        /// <summary>
        /// Orient polygon clockwise
        /// </summary>
        /// <param name="points">Polygon</param>
        public static void OrientClockwise(List<Vector2> points)
        {
            if (!IsOrientedClockwise(points)) points.Reverse();
        }
        /// <summary>
        /// Orien polygon counter clockwise
        /// </summary>
        /// <param name="points">Polygon</param>
        public static void OrientCounterClockwise(List<Vector2> points)
        {
            if (IsOrientedClockwise(points)) points.Reverse();
        }
        #endregion     

        #region Raycasting
        public RayCastResult RayCast(Vector2 startPoint, float length, float direction, float precision = 0.5f)
        {
            return RayCast(this, startPoint, length, direction, precision);
        }
        public static RayCastResult RayCast(Polygon polygon, Vector2 startPoint, float length, float direction, float precision = 0.5f)
        {
            RayCastResult result = new RayCastResult();
            result.Collides = true;
            Vector2 endPoint = GameMath.LengthDir(length, direction) + startPoint;
            result.CollisionPoint = endPoint;

            Polygon line = new Polygon();
            line.m_isConvex = true;
            line.m_isConvexNeedUpdate = false;
            line.AddPoint(startPoint);
            line.AddPoint(endPoint);

            if (!polygon.Collision(line) || polygon.Contains(startPoint))
            {
                result.Collides = false;
                return result;
            }

            float min = 0f;
            float max = length;
            float q = length;

            while (max - min > precision)
            {
                q = (min + max) / 2;
                line[1] = GameMath.LengthDir(q, direction) + startPoint;
                if (polygon.Collision(line))
                {
                    max = q;
                }
                else
                {
                    min = q + precision;
                }
            }

            result.CollisionPoint = line[1];
            result.RayLength = q;

            return result;
        }
        public static RayCastResult RayCast(IEnumerable<Polygon> polygons, Vector2 startPoint, float length, float direction, float precision = 0.5f)
        {
            //bool flag = false;
            //float minLen = float.MaxValue;
            RayCastResult result = new RayCastResult();
            result.RayLength = length;
            result.Collides = false;
            result.CollisionPoint = GameMath.LengthDir(length, direction) + startPoint;
            foreach (Polygon poly in polygons)
            {
                RayCastResult res = RayCast(poly, startPoint, length, direction, precision);
                if(res.Collides)
                {
                    if (res.RayLength < result.RayLength)
                    {
                        result.RayLength = res.RayLength;
                        result.CollisionPoint = res.CollisionPoint;
                    }
                    result.Collides = true;
                }
            }

            return result;
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates over collection of points in polygon.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<Vector2> GetEnumerator()
        {
            return m_points.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_points.GetEnumerator();
        }
        #endregion

        #region PolygonUtilities
        /// <summary>
        /// Return the cross product AB x BC.
        /// The cross product is a vector perpendicular to AB
        /// and BC having length |AB| * |BC| * Sin(theta) and
        /// with direction given by the right-hand rule.
        /// For two vectors in the X-Y plane, the result is a
        /// vector with X and Y components 0 so the Z component
        /// gives the vector's length and direction.
        /// </summary>
        /// <param name="Ax"></param>
        /// <param name="Ay"></param>
        /// <param name="Bx"></param>
        /// <param name="By"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        internal static float CrossProductLength(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }
        /// <summary>
        /// Return the dot product AB · BC.
        /// Note that AB · BC = |AB| * |BC| * Cos(theta).
        /// </summary>
        /// <param name="Ax"></param>
        /// <param name="Ay"></param>
        /// <param name="Bx"></param>
        /// <param name="By"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        internal static float DotProduct(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }
        /// <summary>
        /// Return the angle ABC.
        /// Return a value between PI and -PI.
        /// Note that the value is the opposite of what you might
        /// expect because Y coordinates increase downward.
        /// </summary>
        /// <param name="Ax"></param>
        /// <param name="Ay"></param>
        /// <param name="Bx"></param>
        /// <param name="By"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        internal static float GetAngle(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float)Math.Atan2(cross_product, dot_product);
        }
        #endregion

        /// <summary>
        /// Clone polygon
        /// </summary>
        /// <returns>Cloned polygon</returns>
        public virtual Polygon Clone()
        {
            return new Polygon(this);
        }
        /// <summary>
        /// String that represents polygon
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < m_points.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + m_points[i].ToString() + "}";
            }

            return result;
        }
        #endregion
    }
}
