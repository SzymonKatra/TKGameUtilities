using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace TKGameUtilities.Graphics
{
    public class Image : ICloneable<Image>
    {
        public enum ImageFormat
        {
            Bmp,
            Emf,
            Exif,
            Gif,
            Icon,
            Jpeg,
            Png,
            Tiff,
            Wmf,
        }

        #region Constructors
        public Image(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            m_data = new byte[bitmapData.Width * bitmapData.Height * 4];
            Marshal.Copy(bitmapData.Scan0, m_data, 0, bitmapData.Width * bitmapData.Height * 4);

            //reorder bytes from BGRA to RGBA (on little endian)
            for (int i = 0; i < m_data.Length; i+= 4)
            {
                byte temp = m_data[i];
                m_data[i] = m_data[i + 2];
                m_data[i + 2] = temp;
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

        //public Color this[uint x, uint y]
        //{
        //    get
        //    {
        //        int offset = (int)((uint)m_size.X * y + x) * 4;
        //        return new Color(m_data[offset + R_OFFSET], m_data[offset + G_OFFSET], m_data[offset + B_OFFSET], m_data[offset + A_OFFSET]);
        //    }
        //    set
        //    {
        //        int offset = (int)((uint)m_size.X * y + x) * 4;
        //        m_data[offset + R_OFFSET] = value.R;
        //        m_data[offset + G_OFFSET] = value.G;
        //        m_data[offset + B_OFFSET] = value.B;
        //        m_data[offset + A_OFFSET] = value.A;
        //    }
        //}
        public Color this[int x, int y]
        {
            get
            {
                int offset = (m_size.X * y + x) * 4;
                return new Color(m_data[offset + R_OFFSET], m_data[offset + G_OFFSET], m_data[offset + B_OFFSET], m_data[offset + A_OFFSET]);
            }
            set
            {
                int offset = (m_size.X * y + x) * 4;
                m_data[offset + R_OFFSET] = value.R;
                m_data[offset + G_OFFSET] = value.G;
                m_data[offset + B_OFFSET] = value.B;
                m_data[offset + A_OFFSET] = value.A;
            }
        }
        #endregion

        #region Methods
        public void Clear(Color color)
        {
            for (int y = 0; y < m_size.Y; y++)
            {
                for (int x = 0; x < m_size.X; x++)
                {
                    this[x, y] = color;
                }
            }
        }

        public void Save(string fileName)
        {
            System.Drawing.Imaging.ImageFormat imageFormat = ObtainImageFormat(fileName);
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                Save(fileStream, imageFormat);
            }
        }
        public void Save(string fileName, TKGameUtilities.Graphics.Image.ImageFormat imageFormat)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                Save(fileStream, imageFormat);
            }
        }
        public void Save(Stream stream, TKGameUtilities.Graphics.Image.ImageFormat imageFormat)
        {
            Save(stream, ToGdiImageFormat(imageFormat));
        }
        private unsafe void Save(Stream stream, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            Bitmap bitmap = new Bitmap(m_size.X, m_size.Y);

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(m_data, 0, bitmapData.Scan0, m_data.Length);

            //reorder bytes from RGBA to BGRA (on little endian)
            for (int i = 0; i < m_data.Length; i += 4)
            {
                byte* ind0Ptr = (byte*)(bitmapData.Scan0 + i).ToPointer();
                byte* ind2Ptr = (byte*)(bitmapData.Scan0 + i + 2).ToPointer();

                byte temp = *ind0Ptr;
                *ind0Ptr = *ind2Ptr;
                *ind2Ptr = temp;
            }

            bitmap.UnlockBits(bitmapData);

            bitmap.Save(stream, imageFormat);

            bitmap.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="componentOffset">Image.R_OFFSET or Image.G_OFFSET or Image.B_OFFSET or Image.A_OFFSET</param>
        /// <returns></returns>
        public int ComputeArrayPointer(int x, int y, int componentOffset)
        {
            return (m_size.X * y + x) * 4 + componentOffset;
        }

        public Image Clone()
        {
            Image result = new Image(m_size);
            Buffer.BlockCopy(this.m_data, 0, result.Data, 0, m_data.Length);
            return result;
        }

        private System.Drawing.Imaging.ImageFormat ToGdiImageFormat(TKGameUtilities.Graphics.Image.ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Bmp: return System.Drawing.Imaging.ImageFormat.Bmp;
                case ImageFormat.Emf: return System.Drawing.Imaging.ImageFormat.Emf;
                case ImageFormat.Exif: return System.Drawing.Imaging.ImageFormat.Exif;
                case ImageFormat.Gif: return System.Drawing.Imaging.ImageFormat.Gif;
                case ImageFormat.Icon: return System.Drawing.Imaging.ImageFormat.Icon;
                case ImageFormat.Jpeg: return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ImageFormat.Png: return System.Drawing.Imaging.ImageFormat.Png;
                case ImageFormat.Tiff: return System.Drawing.Imaging.ImageFormat.Tiff;
                case ImageFormat.Wmf: return System.Drawing.Imaging.ImageFormat.Wmf;
            }

            return System.Drawing.Imaging.ImageFormat.MemoryBmp;
        }
        private System.Drawing.Imaging.ImageFormat ObtainImageFormat(string fileName)
        {
            if (fileName.EndsWith(".bmp") || fileName.EndsWith(".dib")) return System.Drawing.Imaging.ImageFormat.Bmp;
            else if (fileName.EndsWith(".emf")) return System.Drawing.Imaging.ImageFormat.Emf;
            else if (fileName.EndsWith(".exif")) return System.Drawing.Imaging.ImageFormat.Exif;
            else if (fileName.EndsWith(".gif")) return System.Drawing.Imaging.ImageFormat.Gif;
            else if (fileName.EndsWith(".ico")) return System.Drawing.Imaging.ImageFormat.Icon;
            else if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpe") || fileName.EndsWith(".jif") || fileName.EndsWith(".jfif") || fileName.EndsWith(".jfi")) return System.Drawing.Imaging.ImageFormat.Jpeg;
            else if (fileName.EndsWith(".png")) return System.Drawing.Imaging.ImageFormat.Png;
            else if (fileName.EndsWith(".tif") || fileName.EndsWith(".tiff")) return System.Drawing.Imaging.ImageFormat.Tiff;
            else if (fileName.EndsWith(".wmf")) return System.Drawing.Imaging.ImageFormat.Wmf;

            return System.Drawing.Imaging.ImageFormat.MemoryBmp;;
        }
        #endregion   
    }
}
