using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace TKGameUtilities.Graphics
{
    public class Font : IDisposable
    {
        [Flags]
        public enum FontStyle
        {
            Regular = 0,
            Bold = 1,
            Italic = 1 << 1,
            Underline = 1 << 2,
            Strikeout = 1 << 3,
        }
        public struct GlyphInfo
        {
            public Rectangle Area;
            public Texture Texture;
            public int Width;
        }

        #region Constructors
        public Font(string fileName, float size, TKGameUtilities.Graphics.Font.FontStyle style = TKGameUtilities.Graphics.Font.FontStyle.Regular, int fromUnicode = 33, int toUnicode = 255, bool antialiasing = true)
        {
            m_fromUnicode = fromUnicode;
            m_toUnicode = toUnicode;
            m_antialiasing = antialiasing;
            m_tabulatorFactor = new Point2(4, 4);
            m_glyphs = new GlyphInfo[m_toUnicode - m_fromUnicode + 1];

            m_fontCollection = new PrivateFontCollection();
            m_fontCollection.AddFontFile(fileName);
            m_gdiFont = new System.Drawing.Font(m_fontCollection.Families[0], size, ToGdiStyle(style));

            m_tempBitmap = new Bitmap(1, 1);
            m_tempGraphics = System.Drawing.Graphics.FromImage(m_tempBitmap);

            ObtainMetrics();
            //ObtainKerning();
            RenderGlyphs();

            m_tempBitmap.Dispose();
            m_tempGraphics.Dispose();
        }
        #endregion

        #region Properties
        private bool m_disposed = false;
        private System.Drawing.Font m_gdiFont;
        private PrivateFontCollection m_fontCollection;
        private Texture[] m_textures;

        private int m_fromUnicode;
        private int m_toUnicode;
        private GlyphInfo[] m_glyphs;
        private ABC[] m_glyphSizes; // used for kerning
        private TEXTMETRICW m_textMetrics;
        private bool m_antialiasing;
        public bool Antialiasing
        {
            get { return m_antialiasing; }
        }
        public int LoadedGlyphsCount
        {
            get { return m_glyphs.Length; }
        }

        private Bitmap m_tempBitmap;
        private System.Drawing.Graphics m_tempGraphics;

        private Point2 m_cellSize;
        public Point2 CellSize
        {
            get { return m_cellSize; }
        }

        private Point2 m_tabulatorFactor;
        public Point2 TabulatorFactor
        {
            get { return m_tabulatorFactor; }
            set { m_tabulatorFactor = value; }
        }

        public float Size
        {
            get { return m_gdiFont.Size; }
        }
        public FontStyle Style
        {
            get { return ToOurStyle(m_gdiFont.Style); }
        }
        public string Name
        {
            get { return m_gdiFont.FontFamily.Name; }
        }
        #endregion

        #region Methods
        private static TKGameUtilities.Graphics.Font.FontStyle ToOurStyle(System.Drawing.FontStyle gdiStyle)
        {
            if (gdiStyle == System.Drawing.FontStyle.Regular) return FontStyle.Regular;

            FontStyle result = 0;

            result |= ((gdiStyle & System.Drawing.FontStyle.Bold) > 0 ? FontStyle.Bold : 0);
            result |= ((gdiStyle & System.Drawing.FontStyle.Italic) > 0 ? FontStyle.Italic : 0);
            result |= ((gdiStyle & System.Drawing.FontStyle.Strikeout) > 0 ? FontStyle.Strikeout : 0);
            result |= ((gdiStyle & System.Drawing.FontStyle.Underline) > 0 ? FontStyle.Underline : 0);

            return result;
        }
        private static System.Drawing.FontStyle ToGdiStyle(TKGameUtilities.Graphics.Font.FontStyle style)
        {
            if (style == FontStyle.Regular) return System.Drawing.FontStyle.Regular;

            System.Drawing.FontStyle result = 0;

            result |= ((style & FontStyle.Bold) > 0 ? System.Drawing.FontStyle.Bold : 0);
            result |= ((style & FontStyle.Italic) > 0 ? System.Drawing.FontStyle.Italic : 0);
            result |= ((style & FontStyle.Strikeout) > 0 ? System.Drawing.FontStyle.Strikeout : 0);
            result |= ((style & FontStyle.Underline) > 0 ? System.Drawing.FontStyle.Underline : 0);

            return result;
        }
        private int ToInternalGlyphIndex(int unicode)
        {
            return unicode - m_fromUnicode;
        }
        private int ToPublicGlyphIndex(int index)
        {
            return index + m_fromUnicode;
        }

        #region Loading
        private unsafe void ObtainMetrics()
        {
            m_glyphSizes=new ABC[m_toUnicode-m_fromUnicode + 1];

            IntPtr graphicsHdc = m_tempGraphics.GetHdc();
            IntPtr hGdiFont = m_gdiFont.ToHfont();
            try
            {
                IntPtr lastHFont = SelectObject(graphicsHdc, hGdiFont);

                GetTextMetricsW(graphicsHdc, out m_textMetrics);
                
                fixed (ABC* ptr = m_glyphSizes)
                {
                    GetCharABCWidthsW(graphicsHdc, (uint)m_fromUnicode, (uint)m_toUnicode, ptr);
                }

                SelectObject(graphicsHdc, lastHFont);
            }
            finally
            {
                DeleteObject(hGdiFont);
                m_tempGraphics.ReleaseHdc(graphicsHdc);
            }

            m_cellSize = new Point2(m_textMetrics.tmMaxCharWidth, m_textMetrics.tmHeight);

            m_glyphs = new GlyphInfo[m_toUnicode - m_fromUnicode + 1];

            for (int i = 0; i < m_glyphs.Length; i++)
            {
                ABC abcSize = m_glyphSizes[i];
                m_glyphs[i].Width = abcSize.abcA + (int)abcSize.abcB + abcSize.abcC;
            }

            //for (int i = m_fromUnicode; i <= m_toUnicode; i++)
            //{
            //    int width = (int)Math.Ceiling(m_tempGraphics.MeasureString(Convert.ToChar(i).ToString(), m_gdiFont).Width);
            //    if (width > m_cellSize.X) m_cellSize.X = width;

            //    m_glyphs[ToInternalGlyphIndex(i)].Width = width;
            //}
        }

        private void RenderGlyphs()
        {
            int textureMaxSize = Texture.MaxTextureSize;

            int maxHorizontalCells = textureMaxSize / m_cellSize.X;
            int maxVerticalCells = textureMaxSize / m_cellSize.Y;
            int maxCells = maxHorizontalCells * maxVerticalCells;

            int fullTextureParts = LoadedGlyphsCount / maxCells;
            int remainingCells = LoadedGlyphsCount - fullTextureParts * maxCells;
            int sqrtRemainingCells = (int)Math.Sqrt(MathHelper.NextPowerOfTwo(remainingCells));
            int horizontalRemainingCells = sqrtRemainingCells;
            int verticalRemainingCells = sqrtRemainingCells;

            m_textures = new Texture[fullTextureParts + (remainingCells > 0 ? 1 : 0)];
            for (int i = 0; i < fullTextureParts; i++)
            {
                m_textures[i] = new Texture(new Point2(m_cellSize.X * maxHorizontalCells, m_cellSize.Y * maxVerticalCells),
                                            null,
                                            OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                                            OpenTK.Graphics.OpenGL.PixelInternalFormat.Alpha);
            }
            if (remainingCells > 0)
            {
                if (m_cellSize.X * sqrtRemainingCells > textureMaxSize || m_cellSize.Y * sqrtRemainingCells > textureMaxSize)
                {
                    verticalRemainingCells = remainingCells / maxHorizontalCells;
                    if (remainingCells % maxHorizontalCells != 0) ++verticalRemainingCells;
                    horizontalRemainingCells = (verticalRemainingCells > 1 ? maxHorizontalCells : remainingCells % maxHorizontalCells);
                }
                m_textures[m_textures.Length - 1] = new Texture(new Point2(m_cellSize.X * horizontalRemainingCells, m_cellSize.Y * verticalRemainingCells),
                                                                null,
                                                                OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                                                                OpenTK.Graphics.OpenGL.PixelInternalFormat.Alpha);
            }

            Bitmap renderBitmap = new Bitmap(m_cellSize.X, m_cellSize.Y);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(renderBitmap);
            graphics.TextRenderingHint = (m_antialiasing ? TextRenderingHint.AntiAlias : TextRenderingHint.SingleBitPerPixel);

            int currentChar = m_fromUnicode;
            for (int i = 0; i < fullTextureParts; i++)
            {
                currentChar = FillCells(m_textures[i], currentChar, maxHorizontalCells, maxVerticalCells, renderBitmap, graphics);
            }
            if (remainingCells > 0)
            {
                currentChar = FillCells(m_textures[m_textures.Length - 1], currentChar, horizontalRemainingCells, verticalRemainingCells, renderBitmap, graphics);
            }

            graphics.Dispose();
            renderBitmap.Dispose();
        }
        private int FillCells(Texture texture, int startChar, int horizontalCellsCount, int verticalCellsCount, Bitmap renderBitmap, System.Drawing.Graphics graphics)
        {
            System.Drawing.Rectangle renderBitmapRectangle = new System.Drawing.Rectangle(0, 0, m_cellSize.X, m_cellSize.Y);
            PointF zeroPoint = new PointF(0, 0);
            Brush brush = Brushes.White;

            int currentChar = startChar;
            for (int cellY = 0; cellY < verticalCellsCount; cellY++)
            {
                for (int cellX = 0; cellX < horizontalCellsCount; cellX++)
                {
                    if (currentChar > m_toUnicode) return currentChar;

                    graphics.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
                    graphics.DrawString(Convert.ToChar(currentChar).ToString(), m_gdiFont, brush, zeroPoint);
                    graphics.Flush();

                    var bmpData = renderBitmap.LockBits(renderBitmapRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    texture.Update(new RectangleInt(new Point2(cellX * m_cellSize.X, cellY * m_cellSize.Y), new Point2(m_cellSize.X, m_cellSize.Y)), bmpData.Scan0);

                    GlyphInfo glyphInfo = m_glyphs[ToInternalGlyphIndex(currentChar)];
                    int firstPixel = SearchGlyphXFirstPixel(bmpData);
                    glyphInfo.Area = new Rectangle(new Vector2(cellX * m_cellSize.X + firstPixel, cellY * m_cellSize.Y),
                                                   new Vector2(SearchGlyphXLastPixel(bmpData) - firstPixel + 1, m_cellSize.Y));
                    glyphInfo.Texture = texture;
                    m_glyphs[ToInternalGlyphIndex(currentChar)] = glyphInfo;

                    renderBitmap.UnlockBits(bmpData);

                    ++currentChar;
                }
            }

            return currentChar;
        }
        private int SearchGlyphXFirstPixel(System.Drawing.Imaging.BitmapData bmpData)
        {
            bool stopSearching = false;
            int glyphX = 0;
            for (int x = 0; x < m_cellSize.X; x++)
            {
                for (int y = 0; y < m_cellSize.Y; y++)
                {
                    IntPtr ptr = new IntPtr((long)bmpData.Scan0 + (y * bmpData.Width + x) * 4);
                    if (Marshal.ReadByte(ptr) > 0) //there is color (alpha > 0)
                    {
                        glyphX = x;
                        stopSearching = true;
                        break;
                    }
                }
                if (stopSearching) break;
            }

            return glyphX;
        }
        private int SearchGlyphXLastPixel(System.Drawing.Imaging.BitmapData bmpData)
        {
            bool stopSearching = false;
            int glyphWidth = 0;
            for (int x = m_cellSize.X - 1; x >= 0; x--)
            {
                for (int y = 0; y < m_cellSize.Y; y++)
                {
                    IntPtr ptr = new IntPtr((long)bmpData.Scan0 + (y * bmpData.Width + x) * 4);
                    if (Marshal.ReadByte(ptr) > 0) //there is color (alpha > 0)
                    {
                        glyphWidth = x;
                        stopSearching = true;
                        break;
                    }
                }
                if (stopSearching) break;
            }

            return glyphWidth;
        }
        #endregion

        //private unsafe void ObtainKerning()
        //{
        //    IntPtr hDC = m_tempGraphics.GetHdc();
        //    m_glyphSizes = new ABC[m_toUnicode - m_fromUnicode + 1];
        //    fixed(ABC* ptr = m_glyphSizes)
        //    {
        //        IntPtr hFontPreviouse = SelectObject(hDC, m_gdiFont.ToHfont());
        //        GetCharABCWidthsW(hDC, (uint)m_fromUnicode, (uint)m_toUnicode, ptr);
        //        SelectObject(hDC, hFontPreviouse);
        //    }
        //}

        public GlyphInfo GetGlyph(int unicode)
        {
            return m_glyphs[ToInternalGlyphIndex(unicode)];
        }
        public GlyphInfo GetGlyph(char unicode)
        {
            return m_glyphs[ToInternalGlyphIndex(Convert.ToInt32(unicode))];
        }
        public int GetKerning(char firstUnicode, char secondUnicode)
        {
            return GetKerning(Convert.ToInt32(firstUnicode), Convert.ToInt32(secondUnicode));
        }
        public int GetKerning(int firstUnicode, int secondUnicode)
        {
            ABC first = m_glyphSizes[ToInternalGlyphIndex(firstUnicode)];
            ABC second = m_glyphSizes[ToInternalGlyphIndex(secondUnicode)];

            return first.abcC + second.abcA; // http://msdn.microsoft.com/en-us/library/dd183418(VS.85).aspx
        }

        public Vector2 MeasureSize(string text)
        {
            Vector2 currentPosition = Vector2.Zero;
            for (int i = 0; i < text.Length; i++)
            {
                int charCode = Convert.ToInt32(text[i]);
                if (charCode == 32) // space
                {
                    currentPosition.X += GetGlyph(32).Width;
                    continue;
                }
                else if (charCode == 10) // \n
                {
                    currentPosition.X = 0f;
                    currentPosition.Y += CellSize.Y;
                    continue;
                }
                else if (charCode == 9) // \t
                {
                    currentPosition.X += GetGlyph(32).Width * TabulatorFactor.X;
                    continue;
                }
                else if (charCode == 11) // \v
                {
                    currentPosition.Y += CellSize.Y * TabulatorFactor.Y;
                    continue;
                }
                Font.GlyphInfo glyph = GetGlyph(charCode);
                currentPosition.X += glyph.Area.Size.X;
                if (i < text.Length - 1)
                {
                    currentPosition.X += GetKerning(charCode, Convert.ToInt32(text[i + 1]));
                }
            }

            return currentPosition;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_tempBitmap.Dispose();
                    m_tempBitmap = null;
                    m_gdiFont.Dispose();
                    m_gdiFont = null;
                    m_fontCollection.Dispose();
                    m_fontCollection = null;
                    for (int i = 0; i < m_textures.Length; i++)
                    {
                        m_textures[i].Dispose();
                        m_textures[i] = null;
                    }
                }

                m_disposed = true;
            }
        }
        ~Font()
        {
            Dispose(false);
        }
        #endregion

        #region Imports
        //http://www.pinvoke.net/

        [StructLayout(LayoutKind.Sequential)]
        private struct ABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;

            public override string ToString()
            {
                return string.Format("A={0}, B={1}, C={2}", abcA, abcB, abcC);
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TEXTMETRICW
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public ushort tmFirstChar;
            public ushort tmLastChar;
            public ushort tmDefaultChar;
            public ushort tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [DllImport("gdi32.dll", EntryPoint = "GetCharABCWidthsW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern unsafe bool GetCharABCWidthsW(IntPtr hdc, uint uFirstChar, uint uLastChar, ABC* lpabc);
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetTextMetricsW(IntPtr hdc, out TEXTMETRICW lptm);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hObject);
        #endregion
    }
}
