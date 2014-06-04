using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public struct VertexBufferDrawOptions
    {
        public BlendOptions Blending;
        public Matrix4 Transform;
        public Texture Texture;
        public PrimitiveType PrimitiveType;
        public int Start;
        public int Count;
        public IndexBuffer IndexBuffer;

        public static readonly VertexBufferDrawOptions Default = new VertexBufferDrawOptions()
        {
            Blending = BlendOptions.Default,
            Transform = Matrix4.Identity,
            Texture = null,
            PrimitiveType = PrimitiveType.Points,
            Start = 0,
            Count = 0,
            IndexBuffer = null,
        };
    }

    /// <summary>
    /// Vertex buffer
    /// </summary>
    /// <typeparam name="T">Vertex structure type</typeparam>
    /// <typeparam name="S">Shader class type</typeparam>
    public abstract class VertexBuffer<T, S> : IDrawable<VertexBufferDrawOptions, S>, IDisposable
    {
        #region Constructors
        protected VertexBuffer(BufferUsageHint usageHint)
        {
            ContextManager.ActivateDefaultIfNoCurrent();

            GL.GenBuffers(1, out m_GLVBOID);

            m_usageHint = usageHint;
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private int m_GLVBOID;
        public int GLVBOID
        {
            get { return m_GLVBOID; }
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
        #endregion

        #region Methods
        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_GLVBOID);
        }
        public static void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateData(T[] data)
        {
            UpdateData(data, 0, data.Length);
        }
        public void UpdateData(T[] data, int start, int count)
        {
            Bind();

            BufferData(data, m_usageHint, start, count);

            m_count = count - start;
        }
        public void UpdateSubData(T[] data, int gpuOffset, int start, int count)
        {
            Bind();

            BufferSubData(data, gpuOffset, start, count);
        }

        public void Draw(RenderTarget target, S shader, VertexBufferDrawOptions options)
        {
            target.Activate();

            Bind();

            DrawVertices(target, shader, ref options);
        }

        protected abstract void BufferData(T[] data, BufferUsageHint usageHint, int start, int count);
        protected abstract void BufferSubData(T[] data, int gpuOffset, int start, int count);
        protected abstract void DrawVertices(RenderTarget target, S shader, ref VertexBufferDrawOptions options);

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
                GL.DeleteBuffer(m_GLVBOID);

                m_disposed = true;
            }
        }
        ~VertexBuffer()
        {
            Dispose(false);
        }
        #endregion
    }
}
