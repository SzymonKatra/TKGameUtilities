using System;
using System.Runtime.InteropServices;

namespace TKGameUtilities.Graphics
{
    /// <summary>
    /// Utility struct for manipulating 32-bits RGBA colors
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IEquatable<Color>
    {
        #region Properties
        /// <summary>Red component of the color</summary>
        public byte R;
        /// <summary>Green component of the color</summary>
        public byte G;
        /// <summary>Blue component of the color</summary>
        public byte B;
        /// <summary>Alpha (transparent) component of the color</summary>
        public byte A;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct color from RGBA int
        /// </summary>
        /// <param name="rgba">RGBA color</param>
        public Color(int rgba)
        {
            R = (byte)(rgba & 0x000000FF);
            G = (byte)((rgba & 0x0000FF00) >> 8);
            B = (byte)((rgba & 0x00FF0000) >> 16);
            A = (byte)((rgba & 0xFF000000) >> 24);
            //R = (byte)((rgba & 0xFF000000) >> 24);
            //G = (byte)((rgba & 0x00FF0000) >> 16);
            //B = (byte)((rgba & 0x0000FF00) >> 8);
            //A = (byte)((rgba & 0x000000FF) >> 0);
        }
        /// <summary>
        /// Construct the color from its red, green and blue components
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        public Color(byte red, byte green, byte blue) :
            this(red, green, blue, 255)
        {
        }
        /// <summary>
        /// Construct the color from its red, green, blue and alpha components
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        /// <param name="alpha">Alpha (transparency) component</param>
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates color from RGB. Note that this method creates color from RGB, not RGBA!
        /// To create from RGBA use constructor.
        /// Alpha will be 255 in this method.
        /// </summary>
        /// <param name="rgb">RGB color</param>
        /// <returns>Color</returns>
        public static Color CreateFromRGB(int rgb)
        {
            Color c;
            c.R = (byte)(rgb & 0x000000FF);
            c.G = (byte)((rgb & 0x0000FF00) >> 8);
            c.B = (byte)((rgb & 0x00FF0000) >> 16);
            //c.R = (byte)((rgb & 0xFF000000) >> 24);
            //c.G = (byte)((rgb & 0x00FF0000) >> 16);
            //c.B = (byte)((rgb & 0x0000FF00) >> 8);
            c.A = 255;
            return c;
        }
        /// <summary>
        /// To RGBA int color
        /// </summary>
        /// <returns>RGBA int</returns>
        public int ToRGBA()
        {
            return R | (G << 8) | (B << 16) | (A << 24);
            //return (R << 24) | (G << 16) | (B << 8) | A;
        }

        /// <summary>
        /// Tells wheter this color is equals to other color
        /// </summary>
        /// <param name="other">Other color</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(Color other)
        {
            return (R == other.R && G == other.G && B == other.B && A == other.A);
        }
        /// <summary>
        /// Tells wheter this color is equals to other object
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>True if equals, otherwise false</returns>
        public override bool Equals(object other)
        {
            return ((other is  Color) ? Equals((Color)other) : false);
        }
        /// <summary>
        /// Gets hash code that represents current object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return (int)(R + G + B + A);
        }
        /// <summary>
        /// Provide a string describing the object
        /// </summary>
        /// <returns>String description of the object</returns>
        public override string ToString()
        {
            return "[Color]" +
                   " R(" + R + ")" +
                   " G(" + G + ")" +
                   " B(" + B + ")" +
                   " A(" + A + ")";
        }
        #endregion

        #region Operators
        /// <summary>
        /// Tells wheter first color is equals to second color
        /// </summary>
        /// <param name="value1">First color</param>
        /// <param name="value2">Second color</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool operator ==(Color value1, Color value2)
        {
            return (value1.R == value2.R && value1.G == value2.G && value1.B == value2.B && value1.A == value2.A);
        }
        /// <summary>
        /// Tells wheter first color don't equals to second color
        /// </summary>
        /// <param name="value1">First color</param>
        /// <param name="value2">Second color</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(Color value1, Color value2)
        {
            return (value1.R != value2.R || value1.G != value2.G || value1.B != value2.B || value1.A != value2.A);
        }

        /// <summary>
        /// Add two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static Color operator +(Color c1, Color c2)
        {
            return new Color((byte)(c1.R + c2.R), (byte)(c1.G + c2.G), (byte)(c1.B + c2.B), (byte)(c1.A + c2.A));
        }
        /// <summary>
        /// Subtract two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static Color operator -(Color c1, Color c2)
        {
            return new Color((byte)(c1.R - c2.R), (byte)(c1.G - c2.G), (byte)(c1.B - c2.B), (byte)(c1.A - c2.A));
        }
        /// <summary>
        /// Multiply two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static Color operator *(Color c1, Color c2)
        {
            return new Color((byte)(c1.R * c2.R), (byte)(c1.G * c2.G), (byte)(c1.B * c2.B), (byte)(c1.A * c2.A));
        }
        /// <summary>
        /// Divide two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static Color operator /(Color c1, Color c2)
        {
            return new Color((byte)(c1.R / c2.R), (byte)(c1.G / c2.G), (byte)(c1.B / c2.B), (byte)(c1.A / c2.A));
        }
        /// <summary>
        /// Add to color scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static Color operator +(Color c, byte value)
        {
            return new Color((byte)(c.R + value), (byte)(c.G + value), (byte)(c.B + value), (byte)(c.A + value));
        }
        /// <summary>
        /// Subtract from color scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static Color operator -(Color c, byte value)
        {
            return new Color((byte)(c.R - value), (byte)(c.G - value), (byte)(c.B - value), (byte)(c.A - value));
        }
        /// <summary>
        /// Multiply color by scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static Color operator *(Color c, byte value)
        {
            return new Color((byte)(c.R * value), (byte)(c.G * value), (byte)(c.B * value), (byte)(c.A * value));
        }
        /// <summary>
        /// Divide color by scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static Color operator /(Color c, byte value)
        {
            return new Color((byte)(c.R / value), (byte)(c.G / value), (byte)(c.B / value), (byte)(c.A / value));
        }
        #endregion

        #region Predefinied
        /// <summary>Color with all zero components - including alpha</summary>
        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        /// <summary>Predefined black color (0, 0, 0)</summary>
        public static readonly Color Black = new Color(0, 0, 0);
        /// <summary>Predefined transparent black color (0, 0, 0, 0)</summary>
        public static readonly Color TransparentBlack = new Color(0, 0, 0, 0);
        /// <summary>Predefined white color (255, 255, 255)</summary>
        public static readonly Color White = new Color(255, 255, 255);
        /// <summary>Predefined transparent white color (255, 255, 255, 0)</summary>
        public static readonly Color TransparentWhite = new Color(255, 255, 255, 0);

        /// <summary>Predefined gray color (128, 128, 128)</summary>
        public static readonly Color Gray = new Color(128, 128, 128);
        /// <summary>Predefined red color (255, 0, 0)</summary>
        public static readonly Color Red = new Color(255, 0, 0);
        /// <summary>Predefined green color (0, 255, 0)</summary>
        public static readonly Color Green = new Color(0, 255, 0);
        /// <summary>Predefined blue color (0, 0, 255)</summary>
        public static readonly Color Blue = new Color(0, 0, 255);
        /// <summary>Predefined yellow color (255, 255, 0)</summary>
        public static readonly Color Yellow = new Color(255, 255, 0);
        /// <summary>Predefined magenta(violet) color (255, 0, 255)</summary>
        public static readonly Color Magenta = new Color(255, 0, 255);
        /// <summary>Predefined cyan color (0, 255, 255)</summary>
        public static readonly Color Cyan = new Color(0, 255, 255);
        /// <summary>Predefined orange color (255, 160, 32)</summary>
        public static readonly Color Orange = new Color(255, 160, 32);
        #endregion
    }
    
    /// <summary>
    /// Utility struct for manipulating float 32-bits float RGBA colors
    /// </summary>
    public struct ColorF : IEquatable<ColorF>
    {
        #region Properties
        /// <summary>Red component of the color</summary>
        public float R;
        /// <summary>Green component of the color</summary>
        public float G;
        /// <summary>Blue component of the color</summary>
        public float B;
        /// <summary>Alpha (transparent) component of the color</summary>
        public float A;
        #endregion

        #region Constructors
        /// <summary>
        /// Construct the color from its red, green and blue components
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        public ColorF(float red, float green, float blue) :
            this(red, green, blue, 255)
        {
        }
        /// <summary>
        /// Construct the color from its red, green, blue and alpha components
        /// </summary>
        /// <param name="red">Red component</param>
        /// <param name="green">Green component</param>
        /// <param name="blue">Blue component</param>
        /// <param name="alpha">Alpha (transparency) component</param>
        public ColorF(float red, float green, float blue, float alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tells wheter this color is equals to other color
        /// </summary>
        /// <param name="other">Other color</param>
        /// <returns>True if equals, otherwise false</returns>
        public bool Equals(ColorF other)
        {
            return (R == other.R && G == other.G && B == other.B && A == other.A);
        }
        /// <summary>
        /// Tells wheter this color is equals to other object
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>True if equals, otherwise false</returns>
        public override bool Equals(object other)
        {
            return ((other is ColorF) ? Equals((ColorF)other) : false);
        }
        /// <summary>
        /// Gets hash code that represents current object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return (int)(R + G + B + A);
        }
        /// <summary>
        /// Provide a string describing the object
        /// </summary>
        /// <returns>String description of the object</returns>
        public override string ToString()
        {
            return "[ColorF]" +
                   " R(" + R + ")" +
                   " G(" + G + ")" +
                   " B(" + B + ")" +
                   " A(" + A + ")";
        }
        #endregion

        #region Operators
        /// <summary>
        /// Tells wheter first color is equals to second color
        /// </summary>
        /// <param name="value1">First color</param>
        /// <param name="value2">Second color</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool operator ==(ColorF value1, ColorF value2)
        {
            return (value1.R == value2.R && value1.G == value2.G && value1.B == value2.B && value1.A == value2.A);
        }
        /// <summary>
        /// Tells wheter first color don't equals to second color
        /// </summary>
        /// <param name="value1">First color</param>
        /// <param name="value2">Second color</param>
        /// <returns>True if don't equals, otherwise false</returns>
        public static bool operator !=(ColorF value1, ColorF value2)
        {
            return (value1.R != value2.R || value1.G != value2.G || value1.B != value2.B || value1.A != value2.A);
        }

        /// <summary>
        /// Add two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static ColorF operator +(ColorF c1, ColorF c2)
        {
            return new ColorF(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B, c1.A + c2.A);
        }
        /// <summary>
        /// Subtract two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static ColorF operator -(ColorF c1, ColorF c2)
        {
            return new ColorF(c1.R - c2.R, c1.G - c2.G, c1.B - c2.B, c1.A - c2.A);
        }
        /// <summary>
        /// Multiply two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static ColorF operator *(ColorF c1, ColorF c2)
        {
            return new ColorF(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B, c1.A * c2.A);
        }
        /// <summary>
        /// Divide two colors
        /// </summary>
        /// <param name="c1">Color 1</param>
        /// <param name="c2">Color 2</param>
        /// <returns>New color</returns>
        public static ColorF operator /(ColorF c1, ColorF c2)
        {
            return new ColorF(c1.R / c2.R, c1.G / c2.G, c1.B / c2.B, c1.A / c2.A);
        }
        /// <summary>
        /// Add to color scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static ColorF operator +(ColorF c, float value)
        {
            return new ColorF(c.R + value, c.G + value, c.B + value, c.A + value);
        }
        /// <summary>
        /// Subtract from color scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static ColorF operator -(ColorF c, float value)
        {
            return new ColorF(c.R - value, c.G - value, c.B - value, c.A - value);
        }
        /// <summary>
        /// Multiply color by scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static ColorF operator *(ColorF c, float value)
        {
            return new ColorF(c.R * value, c.G * value, c.B * value, c.A * value);
        }
        /// <summary>
        /// Divide color by scalar value
        /// </summary>
        /// <param name="c">Color</param>
        /// <param name="value">Scalar</param>
        /// <returns>New color</returns>
        public static ColorF operator /(ColorF c, float value)
        {
            return new ColorF(c.R / value, c.G / value, c.B / value, c.A / value);
        }
        #endregion

        #region Conversions
        /// <summary>
        /// Explicit operator that converts ColorF into Color
        /// </summary>
        /// <param name="color">Input</param>
        /// <returns>Output</returns>
        public static explicit operator Color(ColorF color)
        {
            return new Color((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
        }
        /// <summary>
        /// Explicit operation that converts Color to ColorF
        /// </summary>
        /// <param name="color">Input</param>
        /// <returns>Output</returns>
        public static explicit operator ColorF(Color color)
        {
            return new ColorF(color.R, color.G, color.B, color.A);
        }
        #endregion
    }
}
