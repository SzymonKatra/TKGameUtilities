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
            Bitmap bitmap = new Bitmap(filePath);
            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            CreateFromPtr(new Point2(bitmapData.Width, bitmapData.Height), bitmapData.Scan0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte);
            m_pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;

            bitmap.UnlockBits(bitmapData);
            ContextManager.ActivateDefaultIfNoCurrent();

            bitmap.Dispose();
        }
        public Texture(Image image)
            : this(image.Size, image.Data, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelInternalFormat.Rgba)
        {
        }
        public Texture(Point2 size, IntPtr dataPtr, OpenTK.Graphics.OpenGL.PixelFormat pixelFormat, PixelInternalFormat internalPixelFormat, PixelType pixelType)
        {
            CreateFromPtr(size, dataPtr, pixelFormat, internalPixelFormat, pixelType);
        }
        public unsafe Texture(Point2 size, byte[] data, OpenTK.Graphics.OpenGL.PixelFormat pixelFormat, PixelInternalFormat internalPixelFormat)
        {
            fixed (byte* ptr = data)
            {
                CreateFromPtr(size, new IntPtr(ptr), pixelFormat, internalPixelFormat, PixelType.UnsignedByte);
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

        private Point2 m_size;
        public Point2 Size
        {
            get { return m_size; }
        }

        private Rectangle m_textureRectangle;
        public Rectangle TextureRectangle
        {
            get { return m_textureRectangle; }
        }

        private OpenTK.Graphics.OpenGL.PixelFormat m_pixelFormat;
        public OpenTK.Graphics.OpenGL.PixelFormat PixelFormat
        {
            get { return m_pixelFormat; }
        }

        private PixelType m_pixelType;
        public PixelType PixelType
        {
            get { return m_pixelType; }
        }

        private PixelInternalFormat m_pixelInternalFormat;
        public PixelInternalFormat PixelInternalFormat
        {
            get { return m_pixelInternalFormat; }
        }

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
            m_pixelInternalFormat = internalPixelFormat;
        }
        private void ApplySize(Point2 size)
        {
            m_size = size;
            m_textureMatrix = Matrix4.Identity;
            m_textureMatrix.M11 = 1f / (float)size.X;
            m_textureMatrix.M22 = 1f / (float)size.Y;

            m_textureRectangle = new Rectangle(Vector2.Zero, (Vector2)m_size);
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
            fixed (byte* ptr = data)
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
        public unsafe void Update(RectangleInt area, byte[] data, OpenTK.Graphics.OpenGL.PixelFormat pixelFormat)
        {
            fixed (byte* ptr = data)
            {
                Update(area, new IntPtr(ptr), pixelFormat, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte);
            }
        }
        public void Update(RectangleInt area, IntPtr dataPtr, OpenTK.Graphics.OpenGL.PixelFormat pixelFormat, PixelType pixelType = PixelType.UnsignedByte)
        {
            ContextManager.ActivateDefaultIfNoCurrent();
            Bind();

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, area.Position.X, area.Position.Y, area.Size.X, area.Size.Y, pixelFormat, pixelType, dataPtr);

            Unbind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">Allocated memory of the size of texture and specified PixelType</param>
        public void GetPixels(IntPtr result)
        {
            ContextManager.ActivateDefaultIfNoCurrent();
            Bind();
            
            GL.GetTexImage(TextureTarget.Texture2D, 0, m_pixelFormat, m_pixelType, result);

            Unbind();
        }
        public void GetPixels(IntPtr result, OpenTK.Graphics.OpenGL.PixelFormat pixelFormat, PixelType pixelType = PixelType.UnsignedByte)
        {
            ContextManager.ActivateDefaultIfNoCurrent();
            Bind();

            GL.GetTexImage(TextureTarget.Texture2D, 0, pixelFormat, pixelType, result);

            Unbind();
        }
        public unsafe Image ToImage()
        {
            Image result = new Image(m_size);

            fixed (byte* ptr = result.Data)
            {
                GetPixels(new IntPtr(ptr));
            }

            return result;
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
