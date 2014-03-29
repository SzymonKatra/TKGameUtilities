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
        private List<Vector2> _points = new List<Vector2>();
        private List<Vector2> _edges = new List<Vector2>();
        private Vector2 _position = Vector2.Zero;
        private Vector2 _origin = Vector2.Zero;
        private float _rotation = 0f;
        private List<PolygonTriangle> _triangles = null; 
        private bool _edgesNeedUpdate = true;
        private bool _trianglesNeedUpdate = true;
        private bool _isConvex = false;
        private bool _isConvexNeedUpdate = true;
        private bool _isOrientedClockwise = false;
        private bool _isOrientedClockwiseNeedUpdate = true;
        
        /// <summary>
        /// Edges of polygon
        /// </summary>
        protected List<Vector2> Edges
        {
            get { return _edges; }
        }
        /// <summary>
        /// Points of polygon
        /// </summary>
        protected List<Vector2> Points
        {
            get { return _points; }
            set { _points = value; }
        }
        /// <summary>
        /// Triangles in polygon
        /// </summary>
        protected internal List<PolygonTriangle> Triangles
        {
            get { return _triangles; }
            protected set { _triangles = value; }
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
                for (int i = 0; i < _points.Count; i++)
                {
                    totalX += _points[i].X;
                    totalY += _points[i].Y;
                }

                return new Vector2(totalX / (float)_points.Count, totalY / (float)_points.Count);
            }
        }
        /// <summary>
        /// Position of polygon
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                Vector2 offset = value - _position;
                _position = value;

                Move(offset);
            }
        }
        /// <summary>
        /// Origin of polygon
        /// </summary>
        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                Vector2 offset = value - _origin;
                _origin = value;

                Move(-offset);
            }
        }
        /// <summary>
        /// Rotation of polygon
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                float rotRad = GameMath.ToRadians(GameMath.AdjustAngle(value - _rotation));
                _rotation = GameMath.AdjustAngle(value);
                Vector2 rotateRelative = _position;// +_origin;

                for (int i = 0; i < _points.Count; ++i)
                {
                    _points[i].RotateRad(rotRad, rotateRelative);
                    //_points[i] = Vector2.Rotate(_points[i], rotRad, rotateRelative);
                }
                if (_triangles != null)
                {
                    for (int i = 0; i < _triangles.Count; i++)
                    {
                        for (int j = 0; j < _triangles[i]._points.Count; j++)
                        {
                            _triangles[i]._points[j].RotateRad(rotRad, rotateRelative);
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
                if (_isOrientedClockwiseNeedUpdate)
                {
                    _isOrientedClockwise = IsOrientedClockwise(_points);
                    _isOrientedClockwiseNeedUpdate = false;
                }
                return _isOrientedClockwise;
                //return IsOrientedClockwise(_points);
            }
            set
            {
                if (value)
                    OrientClockwise(_points);
                else
                    OrientCounterClockwise(_points);
            }
        }
        /// <summary>
        /// Gets wheter the polygon is convex.
        /// </summary>
        public bool Convex
        {
            get
            {
                if (_isConvexNeedUpdate) _isConvex = IsConvex(_points);
                return _isConvex;
            }
        }
        /// <summary>
        /// Count of points in polygon
        /// </summary>
        public int PointCount
        {
            get { return _points.Count; }
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
                    if (_trianglesNeedUpdate)
                    {
                        _triangles = Triangulate(_points);
                        _trianglesNeedUpdate = false;
                    }
                }
                return (_triangles == null ? -1 : _triangles.Count);
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
                return _points[index];
            }
            set
            {
                _points[index] = value;
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
            _points.Add(Vector2.Zero);
            _points.Add(new Vector2(rectangle.Size.X, 0));
            _points.Add(rectangle.Size);
            _points.Add(new Vector2(0, rectangle.Size.Y));

            Position = rectangle.Position;
        }
        /// <summary>
        /// Construct polygon from a line
        /// </summary>
        /// <param name="line">Line</param>
        public Polygon(Line line)
        {
            _points.Add(Vector2.Zero);
            _points.Add(line.End - line.Start);

            Position = line.Start;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="polygon">Polygon</param>
        protected Polygon(Polygon polygon)
        {
            this._points = new List<Vector2>(polygon._points);
            this._edges = new List<Vector2>(polygon._edges);
            this._position = polygon._position;
            this._origin = polygon._origin;
            this._rotation = polygon._rotation;
            this._triangles = null;
            if (polygon._triangles != null)
            {
                this._triangles = new List<PolygonTriangle>();
                foreach (PolygonTriangle tri in polygon._triangles)
                {
                    this._triangles.Add((PolygonTriangle)tri.Clone());
                }
            }
            this._edgesNeedUpdate = polygon._edgesNeedUpdate;
            this._trianglesNeedUpdate = polygon._trianglesNeedUpdate;
            this._isConvex = polygon._isConvex;
            this._isConvexNeedUpdate = polygon._isConvexNeedUpdate;
            this._isOrientedClockwise = polygon._isOrientedClockwise;
            this._isOrientedClockwiseNeedUpdate = polygon._isOrientedClockwiseNeedUpdate;
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
            if (_edgesNeedUpdate)
            {
                BuildEdges();
                _edgesNeedUpdate = false;
            }
            if (polygon._edgesNeedUpdate)
            {
                polygon.BuildEdges();
                polygon._edgesNeedUpdate = false;
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

            if (polygon._trianglesNeedUpdate)
            {
                polygon._triangles = Triangulate(polygon._points);
                polygon._trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle tri in polygon._triangles)
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

            if (_trianglesNeedUpdate)
            {
                _triangles = Triangulate(_points);
                _trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle tri in _triangles)
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

            if (_trianglesNeedUpdate)
            {
                _triangles = Triangulate(_points);
                _trianglesNeedUpdate = false;
            }

            if (polygon._trianglesNeedUpdate)
            {
                polygon._triangles = Triangulate(polygon._points);
                polygon._trianglesNeedUpdate = false;
            }

            Vector2 mtvSum = mtv;
            bool collision = false;

            foreach (PolygonTriangle myTri in _triangles)
            {
                foreach (PolygonTriangle otherTri in polygon._triangles)
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
            _points.Add(point);
            NeedUpdateReset();
        }
        /// <summary>
        /// Gets specified point of polygon.
        /// </summary>
        /// <param name="index">Index of point</param>
        /// <returns>Specified point</returns>
        public Vector2 GetPoint(int index)
        {
            return _points[index];
        }
        /// <summary>
        /// Set specified point of polygon.
        /// </summary>
        /// <param name="index">Index of point</param>
        /// <param name="point">Point</param>
        public void SetPoint(int index, Vector2 point)
        {
            _points[index] = point;
            NeedUpdateReset();
        }
        /// <summary>
        /// Gets specified copy of triangle of polygon
        /// </summary>
        /// <param name="index">Index of triangle</param>
        /// <returns>Copy of triangle</returns>
        public PolygonTriangle GetTriangle(int index)
        {
            PolygonTriangle orgTri = _triangles[index];
            PolygonTriangle copy = new PolygonTriangle(orgTri.A, orgTri.B, orgTri.C);
            copy._position = _position;
            copy._origin = _origin;
            copy._rotation = _rotation;
            return copy;
        }
        public Vector2 GetEdge(int index)
        {
            if (_edgesNeedUpdate)
            {
                BuildEdges();
                _edgesNeedUpdate = false;
            }
            return _edges[index];
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
                if (_trianglesNeedUpdate)
                {
                    _triangles = Triangulate(_points);
                    _trianglesNeedUpdate = false;
                }

                foreach (PolygonTriangle tri in _triangles)
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
            int max_point = polygon._points.Count - 1;
            float total_angle = GameMath.GetAngle(polygon._points[max_point].X, polygon._points[max_point].Y, point.X, point.Y, polygon._points[0].X, polygon._points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GameMath.GetAngle(polygon._points[i].X, polygon._points[i].Y, point.X, point.Y, polygon._points[i + 1].X, polygon._points[i + 1].Y);
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
            _isConvexNeedUpdate = true;
            _trianglesNeedUpdate = true;
            _edgesNeedUpdate = true;
            _isOrientedClockwiseNeedUpdate = true;
        }
        /// <summary>
        /// Build polygon edges
        /// </summary>
        protected void BuildEdges()
        {
            Vector2 p1;
            Vector2 p2;
            _edges.Clear();
            for (int i = 0; i < _points.Count; i++)
            {
                p1 = _points[i];
                if (i + 1 >= _points.Count)
                {
                    p2 = _points[0];
                }
                else
                {
                    p2 = _points[i + 1];
                }
                _edges.Add(p2 - p1);
            }
        }
        private void Move(Vector2 offset)
        {
            //move points
            for (int i = 0; i < _points.Count; i++)
            {
                _points[i] += offset;
            }

            //move triangles if polygon is tringulated
            if (_triangles != null)
            {
                for (int i = 0; i < _triangles.Count; i++)
                {
                    for (int j = 0; j < _triangles[i].Points.Count; j++)
                    {
                        _triangles[i].Points[j] += offset;
                    }
                }
            }
        }

        public void PerformTriangulate()
        {
            if (_trianglesNeedUpdate)
            {
                _triangles = Triangulate(_points);
                _trianglesNeedUpdate = false;
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

                float cross_product = GameMath.CrossProductLength(points[A].X, points[A].Y, points[B].X, points[B].Y, points[C].X, points[C].Y);
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
            if (GameMath.GetAngle(points[a].X, points[a].Y, points[b].X, points[b].Y, points[c].X, points[c].Y) > 0)
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
            line._isConvex = true;
            line._isConvexNeedUpdate = false;
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
            return _points.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
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

            for (int i = 0; i < _points.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + _points[i].ToString() + "}";
            }

            return result;
        }
        #endregion
    }
}
