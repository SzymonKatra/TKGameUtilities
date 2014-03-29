using System;
using OpenTK;
//FastSin/Cos copied from http://en.sfml-dev.org/forums/index.php?topic=10564.0 thanks to krzat

#region License

/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License
//Some methods COPIED FROM Mono.XNA to compatibility with Farseer Physics

namespace TKGameUtilities
{
    /// <summary>
    /// Useful mathematics for games and not only
    /// </summary>
    public static class GameMath
    {
        #region Constant
        private static readonly float[] _getSin, _getCos;
        private const int _lookupSize = 1024; //has to be power of 2
        private static float _maxSinCosError = -1.0f;

        /// <summary> Float E </summary>
        public const float E = (float)Math.E;
        /// <summary>Float Log10E</summary>
        public const float Log10E = 0.4342945f;
        /// <summary>Float Log2E</summary>
        public const float Log2E = 1.442695f;

        /// <summary> Float Pi </summary>
        public const float Pi = (float)Math.PI;
        /// <summary> 2 * Pi </summary>
        public const float TwoPi = 2.0f * Pi;
        /// <summary> Pi * Pi </summary>
        public const float SquarePi = Pi * Pi;
        /// <summary> Pi / 2 </summary>
        public const float PiOver2 = Pi / 2.0f;
        /// <summary> Pi / 4 </summary>
        public const float PiOver4 = Pi / 4.0f;

        /// <summary> Pi / 180 </summary>
        public const float ToRadiansFactor = Pi / 180;
        /// <summary> 180 / Pi </summary>
        public const float ToDegressFactor = 180 / Pi;
        /// <summary>
        /// Gets maximum FastSin/Cos error
        /// </summary>
        public static float MaxFastSinCosError
        {
            get
            {
                if (_maxSinCosError < 0)
                {
                    for (var i = 1; i < _lookupSize; i++)
                    {
                        _maxSinCosError = Math.Max(_maxSinCosError, Math.Abs(_getSin[i] - _getSin[i - 1]));
                    }
                    _maxSinCosError /= 2;
                }
                return _maxSinCosError;
            }
        }
        #endregion

        static GameMath()
        {
            _getSin = new float[_lookupSize];
            _getCos = new float[_lookupSize];

            for (var i = 0; i < _lookupSize; i++)
            {
                _getSin[i] = (float)Math.Sin(i * Math.PI / _lookupSize * 2);
                _getCos[i] = (float)Math.Cos(i * Math.PI / _lookupSize * 2);
            }

            //float max = 0;
            //for (var i = 1; i < _lookupSize; i++)
            //{
            //    max = Math.Max(max, Math.Abs(_getSin[i] - _getSin[i - 1]));
            //}
            //max /= 2;
            //System.Diagnostics.Debug.WriteLine("Max sin/cos error is: " + max);
        }

        #region Fast
        /// <summary>
        /// Fast innacurate sinus
        /// </summary>
        /// <param name="a">Value, in degrees</param>
        public static float FastSin(float a)
        {
            return _getSin[(int)(a * (_lookupSize / 360f) + 0.5f) & (_lookupSize - 1)];
        }
        /// <summary>
        /// Fast innacurate cosinus
        /// </summary>
        /// <param name="a">Value, in degrees</param>
        public static float FastCos(float a)
        {
            return _getCos[(int)(a * (_lookupSize / 360f) + 0.5f) & (_lookupSize - 1)];
        }
        #endregion

        #region Converts
        /// <summary>
        /// Converts degress to radians
        /// </summary>
        /// <param name="degress">Degress to convert</param>
        /// <returns>Converted radians</returns>
        public static float ToRadians(float degress)
        {
            return degress * ToRadiansFactor;
        }
        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">Radians to convert</param>
        /// <returns>Converted degrees</returns>
        public static float ToDegrees(float radians)
        {
            return radians * ToDegressFactor;
        }
        #endregion

        #region LengthDir
        /// <summary>
        /// Returns the horizontal left-component of the vector determined by the indicated length and direction
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="direction">Angle in degrees</param>
        /// <returns>Horizontal left-component of the vector</returns>
        public static float LengthDirX(float length, float direction)
        {
            return Convert.ToSingle(Math.Cos(ToRadians(direction)) * length);
        }
        /// <summary>
        /// Returns the vertical top-component of the vector determined by the indicated length and direction
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="direction">Angle in degrees</param>
        /// <returns>Vertical top-component of the vector</returns>
        public static float LengthDirY(float length, float direction)
        {
            return Convert.ToSingle(-Math.Sin(ToRadians(direction)) * length);
        }
        /// <summary>
        /// Returns the vector determined by the indicated length and direction
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="direction">Angle in degrees</param>
        /// <returns>Vector</returns>
        public static Vector2 LengthDir(float length, float direction)
        {
            return new Vector2(LengthDirX(length, direction), LengthDirY(length, direction));
        }
        #endregion

        #region Point
        /// <summary>
        /// Returns the direction from point one toward point two in degrees
        /// </summary>
        /// <param name="v1">Coordinates of point 1</param>
        /// <param name="v2">Coordinates of point 2</param>
        /// <returns>Angle from point one toward point two</returns>
        public static float PointDirection(Vector2 v1, Vector2 v2)
        {
            return AdjustAngle(ToDegrees(Convert.ToSingle(Math.Atan2(v1.Y - v2.Y, v2.X - v1.X))));
        }
        /// <summary>
        /// Returns the distance between point one and point two
        /// </summary>
        /// <param name="v1">Coordinates of point one</param>
        /// <param name="v2">Coordinates of point two</param>
        /// <returns>Distance between point one and point two</returns>
        public static float PointDistance(Vector2 v1, Vector2 v2)
        {
            return Convert.ToSingle(Math.Sqrt((v2.X - v1.X) * (v2.X - v1.X) + (v2.Y - v1.Y) * (v2.Y - v1.Y)));
        }
        #endregion

        #region DirectionUtilities
        /// <summary>
        /// Adjust direction to range 0-360
        /// </summary>
        /// <param name="angle">Angle to adjust in degrees</param>
        /// <returns>Adjusted direction</returns>
        public static float AdjustAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }
        /// <summary>
        /// Flips direction, for example 90 into 270, 45 into 225
        /// </summary>
        /// <param name="direction">Angle to flip in degrees</param>
        /// <returns>Flipped direction</returns>
        public static float FlipDirection(float direction)
        {
            direction = AdjustAngle(direction);
            //direction = (direction >= 180 ? direction - 180 : direction + 180);
            //if (direction < 180) direction += 180; else if (direction >= 180) direction -= 180;
            //return direction;
            return (direction >= 180 ? direction - 180 : direction + 180);
        }
        #endregion

        #region CountingSpeedAndDirection
        /// <summary>
        /// Counts speed from horizontal speed and vertical speed
        /// </summary>
        /// <param name="horizontalSpeed">Horizontal speed</param>
        /// <param name="verticalSpeed">Vertical speed</param>
        /// <returns>Speed</returns>
        public static float CountSpeed(float horizontalSpeed, float verticalSpeed)
        {
            return PointDistance(Vector2.Zero, new Vector2(horizontalSpeed, verticalSpeed));
        }
        /// <summary>
        /// Count speed from velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Speed</returns>
        public static float CountSpeed(Vector2 velocity)
        {
            return PointDistance(Vector2.Zero, velocity);
        }
        /// <summary>
        /// Counts direction from horizontal speed and vertical speed
        /// </summary>
        /// <param name="horizontalSpeed">Horizontal speed</param>
        /// <param name="verticalSpeed">Vertical speed</param>
        /// <returns>Angle</returns>
        public static float CountDirection(float horizontalSpeed, float verticalSpeed)
        {
            return PointDirection(Vector2.Zero, new Vector2(horizontalSpeed, verticalSpeed));
        }
        /// <summary>
        /// Count direction from velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Angle</returns>
        public static float CountDirection(Vector2 velocity)
        {
            return PointDirection(Vector2.Zero, velocity);
        }
        /// <summary>
        /// Counts horizontal speed from speed and direction
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="direction">Angle</param>
        public static float CountHorizontalSpeed(float speed, float direction)
        {
            return LengthDirX(speed, direction);
        }
        /// <summary>
        /// Counts  vertical speed from speed and direction
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="direction">Angle</param>
        public static float CountVerticalSpeed(float speed, float direction)
        {
            return LengthDirY(speed, direction);
        }
        /// <summary>
        /// Counts horizontal and vertical speed from speed and direction
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="direction">Angle</param>
        /// <param name="horizontalSpeed">Reference to horizontal speed</param>
        /// <param name="verticalSpeed">Reference to vertical speed</param>
        public static void CountHVSpeed(float speed, float direction, out float horizontalSpeed, out float verticalSpeed)
        {
            horizontalSpeed = LengthDirX(speed, direction);
            verticalSpeed = LengthDirY(speed, direction);
        }
        /// <summary>
        /// Counts velocity from speed and direction
        /// </summary>
        /// <param name="speed">Speed</param>
        /// <param name="direction">Angle</param>
        /// <returns>Velocity</returns>
        public static Vector2 CountVelocity(float speed, float direction)
        {
            return new Vector2(LengthDirX(speed, direction), LengthDirY(speed, direction));
        }
        #endregion

        #region RandomUtilities
        /// <summary>
        /// Randoms float number
        /// </summary>
        /// <param name="randomizer">Random object</param>
        /// <param name="min">Inclusive minimum range</param>
        /// <param name="max">Inclusive maximum range</param>
        /// <returns>Random float</returns>
        public static float RandomFloat(Random randomizer, float min, float max)
        {
            return min + (Convert.ToSingle(randomizer.NextDouble()) * (max - min));
        }
        /// <summary>
        /// Randoms float number
        /// </summary>
        /// <param name="randomizer">Random object</param>
        /// <param name="min">Inclusive minimum range</param>
        /// <param name="max">Inclusive maximum range</param>
        /// <returns>Random float</returns>
        public static float NextFloat(this Random randomizer, float min, float max)
        {
            return RandomFloat(randomizer, min, max);
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

        #region MathHelperMethods
        /// <summary>
        /// Barycentric
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <param name="value3">Third value</param>
        /// <param name="amount1">First amount</param>
        /// <param name="amount2">Second amount</param>
        /// <returns>Result</returns>
        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }
        /// <summary>
        /// Catmull rom
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <param name="value3">Third value</param>
        /// <param name="value4">Fourth value</param>
        /// <param name="amount">Amount</param>
        /// <returns>Result</returns>
        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (float)(0.5 * (2.0 * value2 +
                                 (value3 - value1) * amount +
                                 (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                 (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }
        /// <summary>
        /// Clamp
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>Result</returns>
        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
        ///// <summary>
        ///// Distance
        ///// </summary>
        ///// <param name="value1">First value</param>
        ///// <param name="value2">Second value</param>
        ///// <returns>Result</returns>
        //public static float Distance(float value1, float value2)
        //{
        //    return Math.Abs(value1 - value2);
        //}
        /// <summary>
        /// Hermite
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="tangent1">First tangent</param>
        /// <param name="value2">Second value</param>
        /// <param name="tangent2">Second tangent</param>
        /// <param name="amount">Amount</param>
        /// <returns>Result</returns>
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return (float)result;
        }
        /// <summary>
        /// Lerp
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <param name="amount">Amount</param>
        /// <returns>Result</returns>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
        ///// <summary>
        ///// Math.Max(value1, value2)
        ///// </summary>
        ///// <param name="value1">First value</param>
        ///// <param name="value2">Second value</param>
        ///// <returns>The bigger value</returns>
        //public static float Max(float value1, float value2)
        //{
        //    return Math.Max(value1, value2);
        //}
        ///// <summary>
        ///// Math.Min(value1, value2)
        ///// </summary>
        ///// <param name="value1">First value</param>
        ///// <param name="value2">Second value</param>
        ///// <returns>The smaller value</returns>
        //public static float Min(float value1, float value2)
        //{
        //    return Math.Min(value1, value2);
        //}
        /// <summary>
        /// Smooth step
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <param name="amount">Amount</param>
        /// <returns>Result</returns>
        public static float SmoothStep(float value1, float value2, float amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            float result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }
        ///// <summary>
        ///// Reduces a given angle to a value between π and -π.
        ///// </summary>
        ///// <param name="angle">Angle to reduce, in radians</param>
        ///// <returns>Result</returns>
        //public static float ReduceAngleRadians(float angle)
        //{
        //    angle = (float)Math.IEEERemainder((double)angle, 6.2831854820251465); //2xPi precission is double
        //    if (angle <= -3.141593f)
        //    {
        //        angle += 6.283185f;
        //        return angle;
        //    }
        //    if (angle > 3.141593f)
        //    {
        //        angle -= 6.283185f;
        //    }
        //    return angle;
        //}
        #endregion

        #region Vector2Ext
        public static void Rotate(this Vector2 vector, float rotation, Vector2 relative)
        {
            //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
            //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

            float sin = (float)Math.Sin(ToRadians(rotation));
            float cos = (float)Math.Cos(ToRadians(rotation));
            float px = vector.X - relative.X;
            float py = vector.Y - relative.Y;

            vector.X = px * cos - py * sin + relative.X;
            vector.Y = px * sin + py * cos + relative.Y;
        }
        public static Vector2 Rotate(ref Vector2 vector, ref float rotation, ref Vector2 relative)
        {
            //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
            //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

            Vector2 result = new Vector2();

            float sin = (float)Math.Sin(ToRadians(rotation));
            float cos = (float)Math.Cos(ToRadians(rotation));
            float px = vector.X - relative.X;
            float py = vector.Y - relative.Y;

            result.X = px * cos - py * sin + relative.X;
            result.Y = px * sin + py * cos + relative.Y;

            return result;
        }

        public static void RotateRad(this Vector2 vector, float rotationRad, Vector2 relative)
        {
            //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
            //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

            float sin = (float)Math.Sin(rotationRad);
            float cos = (float)Math.Cos(rotationRad);
            float px = vector.X - relative.X;
            float py = vector.Y - relative.Y;

            vector.X = px * cos - py * sin + relative.X;
            vector.Y = px * sin + py * cos + relative.Y;
        }
        public static Vector2 RotateRad(ref Vector2 vector, ref float rotationRad, ref Vector2 relative)
        {
            //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
            //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

            Vector2 result = new Vector2();

            float sin = (float)Math.Sin(rotationRad);
            float cos = (float)Math.Cos(rotationRad);
            float px = vector.X - relative.X;
            float py = vector.Y - relative.Y;

            result.X = px * cos - py * sin + relative.X;
            result.Y = px * sin + py * cos + relative.Y;

            return result;
        }
        #endregion
    }
}
