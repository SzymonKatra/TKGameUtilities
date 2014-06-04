using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public struct PrimitiveBatchDrawOptions
    {
        public BlendOptions Blending;
        public Matrix4 Transform;

        public static readonly PrimitiveBatchDrawOptions Default = new PrimitiveBatchDrawOptions()
        {
            Blending = BlendOptions.Default,
            Transform = Matrix4.Identity
        };
    }

    public class IndexedPrimitiveBatch : IDrawable<ColoredPrimitiveShader, PrimitiveBatchDrawOptions>, IDisposable
    {
        #region Constructors
        public IndexedPrimitiveBatch()
        {
            m_vertexBuffer = new VertexBufferPC(BufferUsageHint.DynamicDraw);
            m_indexBuffer = new IndexBuffer(BufferUsageHint.DynamicDraw, DrawElementsType.UnsignedInt);
            m_vertices = new VertexPC[100 * 3];
            m_indices = new uint[100 * 3];
            m_verticesCount = 0;
            m_indicesCount = 0;
            m_active = false;
        }
        #endregion

        #region Properties
        private bool m_disposed = false;

        private VertexBufferPC m_vertexBuffer;
        private IndexBuffer m_indexBuffer;
        private VertexPC[] m_vertices;
        private uint[] m_indices;

        private int m_verticesCount;
        public int VerticesCount
        {
            get { return m_verticesCount; }
        }

        private int m_indicesCount;
        public int IndicesCount
        {
            get { return m_indicesCount; }
        }

        private bool m_active;
        public bool Active
        {
            get { return m_active; }
        }
        #endregion

        #region Methods
        public void Begin()
        {
            if (m_active) throw new InvalidOperationException("Already active");

            m_verticesCount = 0;
            m_indicesCount = 0;

            m_active = true;
        }
        public void End()
        {
            if (!m_active) throw new InvalidOperationException("Call Begin() first");

            m_active = false;
        }

        public void AddLine(Vector2 start, Vector2 end, Color startColor, Color endColor)
        {
            if (m_verticesCount + 2 >= m_vertices.Length) Array.Resize(ref m_vertices, Math.Max(m_vertices.Length + 2, m_vertices.Length * 2));
            if (m_indicesCount + 2 >= m_indices.Length) Array.Resize(ref m_indices, Math.Max(m_vertices.Length + 2, m_indices.Length * 2));

            m_vertices[m_verticesCount].Position = start;
            m_vertices[m_verticesCount].Color = startColor;
            m_indices[m_indicesCount] = (uint)m_verticesCount;

            ++m_verticesCount;
            ++m_indicesCount;

            m_vertices[m_verticesCount].Position = end;
            m_vertices[m_verticesCount].Color = endColor;
            m_indices[m_indicesCount] = (uint)m_verticesCount;

            ++m_verticesCount;
            ++m_indicesCount;
        }
        public unsafe void AddLines(IList<Vector2> lines, Color color)
        {
            if(lines.Count % 2 != 0) throw new ArgumentException("Count of vertices must be divisible by 2");

            if (m_verticesCount + lines.Count >= m_vertices.Length) Array.Resize(ref m_vertices, Math.Max(m_vertices.Length + lines.Count, m_vertices.Length * 2));
            if (m_indicesCount + lines.Count >= m_indices.Length) Array.Resize(ref m_indices, Math.Max(m_indices.Length + lines.Count, m_indices.Length * 2));

            fixed(VertexPC* verticesStartPtr = m_vertices)
            {
                fixed(uint* indicesStartPtr = m_indices)
                {
                    VertexPC* vPtr = verticesStartPtr + m_verticesCount;
                    uint* iPtr = indicesStartPtr + m_indicesCount;

                    foreach(Vector2 vec in lines)
                    {
                        vPtr->Position = vec;
                        vPtr->Color = color;

                        ++vPtr;
                        ++iPtr;
                    }
                }
            }

            m_verticesCount += lines.Count;
            m_indicesCount += lines.Count;
        }

        public void Draw(RenderTarget target, ColoredPrimitiveShader shader, PrimitiveBatchDrawOptions options)
        {
            VertexBufferPCDrawOptions vboOptions = new VertexBufferPCDrawOptions()
            {
                Blending = options.Blending,
                Transform = options.Transform,
                IndexBuffer = m_indexBuffer,
                PrimitiveType = PrimitiveType.Lines,
                Start = 0,
                Count = m_verticesCount
            };

            m_indexBuffer.UpdateData(m_indices, 0, m_indicesCount);
            m_vertexBuffer.UpdateData(m_vertices, 0, m_verticesCount);
            
            m_vertexBuffer.Draw(target, shader, vboOptions);
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
                    m_vertexBuffer.Dispose();
                    m_vertexBuffer = null;
                    m_indexBuffer.Dispose();
                    m_indexBuffer = null;
                }

                m_disposed = true;
            }
        }
        ~IndexedPrimitiveBatch()
        {
            Dispose(false);
        }
        #endregion    
    }
}
