using System;
using OpenTK;

namespace TKGameUtilities
{
    /// <summary>
    /// Defines line
    /// </summary>
    public struct Line
    {
        #region Properties
        #region Constant
        /// <summary>
        /// Zero line (0, 0); (0, 0)
        /// </summary>
        public static readonly Line Zero = new Line(Vector2.Zero, Vector2.Zero);
        /// <summary>
        /// One line (1, 1); (1, 1)
        /// </summary>
        public static readonly Line One = new Line(Vector2.One, Vector2.One);
        #endregion
        public Vector2 Start;
        public Vector2 End;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct line from two points
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        #endregion
    }
}
