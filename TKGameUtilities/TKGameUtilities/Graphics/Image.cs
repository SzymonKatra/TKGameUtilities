using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TKGameUtilities.Graphics
{
    public class Image
    {
        #region Constructors
        public Image(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            m_data = new byte[bitmapData.Width * bitmapData.Height * 4];
            Marshal.Copy(bitmapData.Scan0, m_data, 0, bitmapData.Width * bitmapData.Height * 4);

            //reorder bytes from BGRA to RGBA
            for (int i = 0; i < m_data.Length; i+= 4)
            {
                byte r = m_data[i];
                m_data[i] = m_data[i + 2];
                m_data[i + 2] = r;
            }

            m_size = new Point2(bitmapData.Width, bitmapData.Height);

            bitmap.UnlockBits(bitmapData);

            bitmap.Dispose();
        }
        public Image(Point2 size)
        {
            m_size = size;
            m_data = new byte[size.X * size.Y * 4];
        }
        #endregion

        #region Properties
        public const int R_OFFSET = 0;
        public const int G_OFFSET = 1;
        public const int B_OFFSET = 2;
        public const int A_OFFSET = 3;

        private byte[] m_data;
        public byte[] Data
        {
            get { return m_data; }
        }

        private Point2 m_size;
        public Point2 Size
        {
            get { return m_size; }
        }

        public Color this[uint x, uint y]
        {
            get
            {
                int offset = (int)((uint)m_size.X * y + x) * 4;
                return new Color(m_data[offset + R_OFFSET], m_data[offset + G_OFFSET], m_data[offset + B_OFFSET], m_data[offset + A_OFFSET]);
            }
            set
            {
                int offset = (int)((uint)m_size.X * y + x);
                m_data[offset + R_OFFSET] = value.R;
                m_data[offset + G_OFFSET] = value.G;
                m_data[offset + B_OFFSET] = value.B;
                m_data[offset + A_OFFSET] = value.A;
            }
        }
        #endregion
    }
}
