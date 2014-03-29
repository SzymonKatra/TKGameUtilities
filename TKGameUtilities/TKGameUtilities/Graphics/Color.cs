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
        /// <summary>Predefined white color (255, 255, 255)</summary>
        public static readonly Color White = new Color(255, 255, 255);

        /// <summary>Predefined gray color (128, 128, 128)</summary>
        public static readonly Color Gray = new Color(128, 128, 128);
        /// <summary>Predefined dark gray color (64, 64, 64)</summary>
        public static readonly Color DarkGray = new Color(64, 64, 64);
        /// <summary>Predefined light gray color (192, 192, 192)</summary>
        public static readonly Color LightGray = new Color(192, 192, 192);

        /// <summary>Predefined red color (255, 0, 0)</summary>
        public static readonly Color Red = new Color(255, 0, 0);
        /// <summary>Predefined dark red color (128, 0, 0)</summary>
        public static readonly Color DarkRed = new Color(128, 0, 0);
        /// <summary>Predefined light red color (255, 128, 128)</summary>
        public static readonly Color LightRed = new Color(255, 128, 128);

        /// <summary>Predefined green color (0, 255, 0)</summary>
        public static readonly Color Green = new Color(0, 255, 0);
        /// <summary>Predefined dark green color (0, 192, 0)</summary>
        public static readonly Color DarkGreen = new Color(0, 192, 0);
        /// <summary>Predefined light green color (128, 255, 128)</summary>
        public static readonly Color LightGreen = new Color(128, 255, 128);

        /// <summary>Predefined blue color (0, 0, 255)</summary>
        public static readonly Color Blue = new Color(0, 0, 255);
        /// <summary>Predefined dark blue color (0, 0, 192)</summary>
        public static readonly Color DarkBlue = new Color(0, 0, 192);
        /// <summary>Predefined light blue color (128, 128, 255)</summary>
        public static readonly Color LightBlue = new Color(128, 128, 255);

        /// <summary>Predefined yellow color (255, 255, 0)</summary>
        public static readonly Color Yellow = new Color(255, 255, 0);
        /// <summary>Predefined dark yellow color (192, 192, 0)</summary>
        public static readonly Color DarkYellow = new Color(192, 192, 0);
        /// <summary>Predefined light yellow color (255, 255, 128)</summary>
        public static readonly Color LightYellow = new Color(255, 255, 128);

        /// <summary>Predefined magenta(violet) color (255, 0, 255)</summary>
        public static readonly Color Magenta = new Color(255, 0, 255);
        /// <summary>Predefined dark magenta(violet) color (192, 0, 192)</summary>
        public static readonly Color DarkMagenta = new Color(192, 0, 192);
        /// <summary>Predefined light magenta(violet) color (255, 128, 255)</summary>
        public static readonly Color LightMagenta = new Color(255, 128, 255);

        /// <summary>Predefined cyan color (0, 255, 255)</summary>
        public static readonly Color Cyan = new Color(0, 255, 255);
        /// <summary>Predefined dark cyan color (0, 192, 192)</summary>
        public static readonly Color DarkCyan = new Color(0, 192, 192);
        /// <summary>Predefined light cyan color (128, 255, 255)</summary>
        public static readonly Color LightCyan = new Color(128, 255, 255);

        /// <summary>Predefined orange color (255, 160, 32)</summary>
        public static readonly Color Orange = new Color(255, 160, 32);
        /// <summary>Predefined dark orange color (160, 96, 0)</summary>
        public static readonly Color DarkOrange = new Color(160, 96, 0);
        /// <summary>Predefined light orange color (255, 192, 128)</summary>
        public static readonly Color LightOrange = new Color(255, 192, 128);
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
