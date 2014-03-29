using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class IndexBuffer : IDisposable
    {
        #region Constructors
        public IndexBuffer(BufferUsageHint usageHint, DrawElementsType elementsType)
        {
            m_usageHint = usageHint;
            m_elementsType = elementsType;

            GL.GenBuffers(1, out m_GLIBOID);
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private int m_GLIBOID;
        public int GLIDBOID
        {
            get { return m_GLIBOID; }
        }

        private BufferUsageHint m_usageHint;
        public BufferUsageHint UsageHint
        {
            get { return m_usageHint; }
        }

        private int m_count;
        public int Count
        {
            get { return m_count; }
        }

        private DrawElementsType m_elementsType;
        public DrawElementsType ElementsType
        {
            get { return m_elementsType; }
        }
        #endregion

        #region Methods
        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_GLIBOID);
        }
        public static void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        #region Byte
        public void UpdateData(byte[] data)
        {
            UpdateData(data, 0, data.Length);
        }
        public unsafe void UpdateData(byte[] data, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedByte) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed(byte* ptr = data)
            {
                BufferData((IntPtr)ptr, start, count, sizeof(byte));
            }

            m_count = count - start;
        }
        public unsafe void UpdateSubData(byte[] data, int gpuOffset, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedByte) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed(byte* ptr = data)
            {
                BufferSubData((IntPtr)ptr, gpuOffset, start, count, sizeof(byte));
            }
        }
        #endregion
        #region UInt16
        public void UpdateData(ushort[] data)
        {
            UpdateData(data, 0, data.Length);
        }
        public unsafe void UpdateData(ushort[] data, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedShort) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed (ushort* ptr = data)
            {
                BufferData((IntPtr)ptr, start, count, sizeof(ushort));
            }

            m_count = count - start;
        }
        public unsafe void UpdateSubData(ushort[] data, int gpuOffset, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedShort) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed (ushort* ptr = data)
            {
                BufferSubData((IntPtr)ptr, gpuOffset, start, count, sizeof(ushort));
            }
        }
        #endregion
        #region UInt32
        public void UpdateData(uint[] data)
        {
            UpdateData(data, 0, data.Length);
        }
        public unsafe void UpdateData(uint[] data, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedInt) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed (uint* ptr = data)
            {
                BufferData((IntPtr)ptr, start, count, sizeof(uint));
            }

            m_count = count - start;
        }
        public unsafe void UpdateSubData(uint[] data, int gpuOffset, int start, int count)
        {
            if (m_elementsType != DrawElementsType.UnsignedInt) throw new InvalidOperationException("Bad element type. Element type for this instance is: " + m_elementsType.ToString());

            Bind();

            fixed (uint* ptr = data)
            {
                BufferSubData((IntPtr)ptr, gpuOffset, start, count, sizeof(uint));
            }
        }
        #endregion

        private unsafe void BufferData(IntPtr ptr, int start, int count, int sizeOfElement)
        {
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                         (IntPtr)((count - start) * sizeOfElement),
                         (IntPtr)ptr + start * sizeOfElement,
                         m_usageHint);
        }
        private unsafe void BufferSubData(IntPtr ptr, int gpuOffset, int start, int count, int sizeOfElement)
        {
            GL.BufferSubData(BufferTarget.ElementArrayBuffer,
                            (IntPtr)(gpuOffset * sizeOfElement),
                            (IntPtr)((count - start) * sizeOfElement),
                            (IntPtr)ptr + start * sizeOfElement);
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
                GL.DeleteBuffer(m_GLIBOID);

                m_disposed = true;
            }
        }
        ~IndexBuffer()
        {
            Dispose(false);
        }
        #endregion
    }
}
