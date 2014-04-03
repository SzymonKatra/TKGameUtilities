using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace TKGameUtilities.Graphics
{
    public class PixelBuffer : IDisposable
    {
        #region Constructors
        public PixelBuffer(bool pixelUnpackBuffer = true, BufferUsageHint usageHint = BufferUsageHint.DynamicDraw)
        {
            GL.GenBuffers(1, out m_GLPBOID);
            m_usageHint = usageHint;

            m_bufferTarget = (pixelUnpackBuffer ? BufferTarget.PixelUnpackBuffer : BufferTarget.PixelPackBuffer);
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private int m_GLPBOID;

        public int GLPBOID
        {
            get { return m_GLPBOID; }
        }

        private BufferUsageHint m_usageHint;
        public BufferUsageHint UsageHint
        {
            get { return m_usageHint; }
        }

        private BufferTarget m_bufferTarget;
        public BufferTarget BufferTarget
        {
            get { return m_bufferTarget; }
        }
        public bool IsPixelUnpackBuffer
        {
            get { return (m_bufferTarget == OpenTK.Graphics.OpenGL.BufferTarget.PixelUnpackBuffer); }
        }

        private int m_size;
        public int Size
        {
            get { return m_size; }
        }
        #endregion

        #region Methods
        public void Bind()
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            GL.BindBuffer(m_bufferTarget, m_GLPBOID);
        }
        public static void Unbind(bool pixelUnpackBuffer = true)
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            GL.BindBuffer((pixelUnpackBuffer ? BufferTarget.PixelUnpackBuffer : BufferTarget.PixelPackBuffer), 0);
        }

        public void UpdateData(IntPtr data, int size)
        {
            Bind();

            BufferData(data, m_usageHint, size);

            m_size = size;
        }
        public void UpdateSubData(IntPtr data, int gpuOffset, int size)
        {
            Bind();

            BufferSubData(data, gpuOffset, size);
        }

        public void UpdateData(byte[] data)
        {
            UpdateData(data, 0, data.Length);
        }
        public unsafe void UpdateData(byte[] data, int start, int count)
        {
            fixed (byte* ptr = data)
            {
                UpdateData(new IntPtr(ptr + start), count);
            }
        }
        public void UpdateSubData(byte[] data, int gpuOffset)
        {
            UpdateSubData(data, gpuOffset, 0, data.Length);
        }
        public unsafe void UpdateSubData(byte[] data, int gpuOffset, int start, int count)
        {
            fixed (byte* ptr = data)
            {
                UpdateSubData(new IntPtr(ptr + start), gpuOffset, count);
            }
        }

        protected void BufferData(IntPtr data, BufferUsageHint usageHint, int size)
        {
            GL.BufferData(m_bufferTarget,
                          new IntPtr(size),
                          data,
                          m_usageHint);
        }
        protected void BufferSubData(IntPtr data, int gpuOffset, int size)
        {
            GL.BufferSubData(m_bufferTarget,
                             new IntPtr(gpuOffset),
                             new IntPtr(size),
                             data);
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



                m_disposed = true;
            }
        }
        ~PixelBuffer()
        {
            Dispose(false);
        }
        #endregion
    }
}
