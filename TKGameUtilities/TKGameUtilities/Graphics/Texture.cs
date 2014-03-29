using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace TKGameUtilities.Graphics
{
    public class Texture : IDisposable
    {
        #region Constructors
        public Texture(string filePath)
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            Bitmap bitmap = new Bitmap(filePath);

            GL.GenTextures(1, out m_GLTextureID);
            GL.BindTexture(TextureTarget.Texture2D, m_GLTextureID);

            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            ApplySize(new Point2(bitmap.Size.Width, bitmap.Size.Height));

            m_pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
            m_pixelType = PixelType.UnsignedByte;

            bitmap.Dispose();
        }
        public Texture(Point2 size, IntPtr dataPtr, OpenTK.Graphics.OpenGL.PixelFormat inputPixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType)
        {
            CreateFromPtr(size,dataPtr,inputPixelFormat,internalPixelFormat,pixelType);
        }
        public unsafe Texture(Point2 size, byte[] data, OpenTK.Graphics.OpenGL.PixelFormat inputPixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType)
        {
            fixed (byte* ptr = data)
            {
                CreateFromPtr(size, new IntPtr(ptr), inputPixelFormat, internalPixelFormat, pixelType);
            }
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private int m_GLTextureID;
        public int GLTextureID
        {
            get { return m_GLTextureID; }
        }

        private Matrix4 m_textureMatrix;
        public Matrix4 TextureMatrix
        {
            get { return m_textureMatrix; }
        }

        private Point2 m_textureSize;
        public Point2 Size
        {
            get { return m_textureSize; }
        }

        private Rectangle m_textureRectangle;
        public Rectangle TextureRectangle
        {
            get { return m_textureRectangle; }
        }

        private OpenTK.Graphics.OpenGL.PixelFormat m_pixelFormat;
        private PixelType m_pixelType;

        public static int MaxTextureSize
        {
            get
            {
                ContextManager.ActivateDefaultIfNoCurrent();

                return GL.GetInteger(GetPName.MaxTextureSize);
            }
        }
        #endregion

        #region Methods
        private void CreateFromPtr(Point2 size, IntPtr dataPtr, OpenTK.Graphics.OpenGL.PixelFormat inputPixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType)
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            GL.GenTextures(1, out m_GLTextureID);
            GL.BindTexture(TextureTarget.Texture2D, m_GLTextureID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, internalPixelFormat, size.X, size.Y, 0, inputPixelFormat, pixelType, dataPtr);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            ApplySize(size);

            m_pixelFormat = inputPixelFormat;
            m_pixelType = pixelType;
        }
        private void ApplySize(Point2 size)
        {
            m_textureSize = size;
            m_textureMatrix = Matrix4.Identity;
            m_textureMatrix.M11 = 1f / (float)size.X;
            m_textureMatrix.M22 = 1f / (float)size.Y;

            m_textureRectangle = new Rectangle(Vector2.Zero, (Vector2)m_textureSize);
        }
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_GLTextureID);
        }
        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public unsafe void Update(RectangleInt area, byte[] data)
        {
            fixed(byte* ptr = data)
            {
                Update(area, new IntPtr(ptr));
            }
        }
        public void Update(RectangleInt area, IntPtr dataPtr)
        {
            ContextManager.ActivateDefaultIfNoCurrent();
            Bind();

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, area.Position.X, area.Position.Y, area.Size.X, area.Size.Y, m_pixelFormat, m_pixelType, dataPtr);

            Unbind();
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
                }

                ContextManager.ActivateDefaultIfNoCurrent();
                GL.DeleteTexture(m_GLTextureID);

                m_disposed = true;
            }
        }
        ~Texture()
        {
            Dispose(false);
        }
        #endregion
    }
}
