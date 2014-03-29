﻿#define FAST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace TKGameUtilities.Graphics
{
    public struct SpriteBatchDrawOptions
    {
        public BlendOptions Blending;
        public Matrix4 Transform;

        public static readonly SpriteBatchDrawOptions Default = new SpriteBatchDrawOptions()
        {
            Blending = BlendOptions.Default,
            Transform = Matrix4.Identity
        };
    }

    public class BaseSpriteBatch
    {
        protected struct BatchItem
        {
            public Texture Texture;
            public int Count;
        }

        #region Constructors
        public BaseSpriteBatch()
        {
            m_vertexBuffer = new VertexBufferPositionColorTexCoords(BufferUsageHint.DynamicDraw);
            m_vertices = new VertexPositionColorTexCoords[100 * 4];
            m_verticesCount = 0;
            m_previousVerticesCount = 0;
            m_active = false;
            m_drawQueue = new BatchItem[20];
            m_queueCount = 0;
            m_activeItem = new BatchItem();
            m_activeItem.Texture = null;
            m_activeItem.Count = 0;
        }
        #endregion

        #region Properties
        private VertexBufferPositionColorTexCoords m_vertexBuffer;
        protected VertexBufferPositionColorTexCoords VertexBuffer
        {
            get { return m_vertexBuffer; }
        }

        private VertexPositionColorTexCoords[] m_vertices;
        protected VertexPositionColorTexCoords[] Vertices
        {
            get { return m_vertices; }
        }

        private int m_verticesCount;
        public int VertiesCount
        {
            get { return m_verticesCount; }
        }
        public int SpriteCount
        {
            get { return m_verticesCount / 4; }
        }

        private int m_previousVerticesCount;

        private bool m_active;
        public bool Active
        {
            get { return m_active; }
        }

        private BatchItem[] m_drawQueue;
        protected BatchItem[] DrawQueue
        {
            get { return m_drawQueue; }
        }

        private int m_queueCount;
        protected int QueueCount
        {
            get { return m_queueCount; }
        }

        private BatchItem m_activeItem;
        #endregion

        #region Methods
        public void Begin()
        {
            if (m_active) throw new InvalidOperationException("Already active");

            m_verticesCount = 0;
            m_previousVerticesCount = 0;
            m_queueCount = 0;

            m_active = true;
        }
        public void End()
        {
            if (!m_active) throw new InvalidOperationException("Call Begin() first");
            EnqueueCurrent();
            m_active = false;
        }
        public unsafe void Add(Texture texture, Rectangle textureRectangle, Vector2 position, Color color, Vector2 scale, Vector2 origin, float rotation = 0)
        {
            if (m_verticesCount + 4 >= m_vertices.Length) Array.Resize(ref m_vertices, m_verticesCount * 2);

            ApplyTexture(texture);

#if FAST
            float cos = GameMath.FastCos(rotation);
            float sin = GameMath.FastSin(rotation);
#else
            float sin = 0, cos = 1;
            if (rotation != 0)
            {
                rotation = GameMath.ToRadians(rotation);
                sin = (float)Math.Sin(rotation);
                cos = (float)Math.Cos(rotation);
            }
#endif

            float px = -origin.X * scale.X;
            float py = -origin.Y * scale.Y;
            scale.X *= textureRectangle.Size.X;
            scale.Y *= textureRectangle.Size.Y;

            fixed(VertexPositionColorTexCoords* sptr = m_vertices)
            {
                //x' = (x - x2) * cos(rot) - (y - y2) * sin(rot) + x2
                //y' = (x - x2) * sin(rot) + (y - y2) * cos(rot) + y2

                VertexPositionColorTexCoords* ptr = sptr + m_verticesCount;

                ptr->Position.X = px * cos - py * sin + position.X;
                ptr->Position.Y = px * sin + py * cos + position.Y;
                ptr->TexCoords.X = textureRectangle.Left;
                ptr->TexCoords.Y = textureRectangle.Top;
                ptr->Color = color;

                ++ptr;
                px += scale.X;

                ptr->Position.X = px * cos - py * sin + position.X;
                ptr->Position.Y = px * sin + py * cos + position.Y;
                ptr->TexCoords.X = textureRectangle.Right;
                ptr->TexCoords.Y = textureRectangle.Top;
                ptr->Color = color;

                ++ptr;
                py += scale.Y;

                ptr->Position.X = px * cos - py * sin + position.X;
                ptr->Position.Y = px * sin + py * cos + position.Y;
                ptr->TexCoords.X = textureRectangle.Right;
                ptr->TexCoords.Y = textureRectangle.Bottom;
                ptr->Color = color;

                ++ptr;
                px -= scale.X;

                ptr->Position.X = px * cos - py * sin + position.X;
                ptr->Position.Y = px * sin + py * cos + position.Y;
                ptr->TexCoords.X = textureRectangle.Left;
                ptr->TexCoords.Y = textureRectangle.Bottom;
                ptr->Color = color;
            }

            m_verticesCount += 4;
        }

        private void ApplyTexture(Texture texture)
        {
            if (!m_active) throw new InvalidOperationException("Call Begin() first");

            if (m_activeItem.Texture != texture)
            {
                EnqueueCurrent();
                m_activeItem.Texture = texture;
            }
        }
        private void EnqueueCurrent()
        {
            int currentQueueVerticesCount = m_verticesCount - m_previousVerticesCount;
            m_previousVerticesCount = m_verticesCount;

            if (currentQueueVerticesCount > 0)
            {
                if(m_queueCount >= m_drawQueue.Length)
                {
                    Array.Resize(ref m_drawQueue, m_queueCount * 2);
                }

                BatchItem item = m_activeItem;
                item.Count = currentQueueVerticesCount;
                m_drawQueue[m_queueCount] = item;
                //m_drawQueue[m_queueCount] = new BatchItem() { Texture = m_activeTexture, Count = currentQueueVerticesCount };

                ++m_queueCount;
            }  
        }
        #endregion
    }
}
