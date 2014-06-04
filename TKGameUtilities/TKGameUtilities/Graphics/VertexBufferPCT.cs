using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    /// <summary>
    /// PCT - PositionColorTexCoords
    /// </summary>
    public struct VertexBufferPCTDrawOptions
    {
        public BlendOptions Blending;
        public Matrix4 Transform;
        public PrimitiveType PrimitiveType;
        public Texture Texture;
        public int Start;
        public int Count;
        public IndexBuffer IndexBuffer;

        public static readonly VertexBufferPCTDrawOptions Default = new VertexBufferPCTDrawOptions()
        {
            Blending = BlendOptions.Default,
            Transform = Matrix4.Identity,
            PrimitiveType = PrimitiveType.Points,
            Texture = null,
            Start = 0,
            Count = 0,
            IndexBuffer = null,
        };
    }

    /// <summary>
    /// PCT - PositionColorTexCoords
    /// </summary>
    public class VertexBufferPCT : VertexBuffer<VertexPCT, TexturedPrimitiveShader, VertexBufferPCTDrawOptions>
    {
        public VertexBufferPCT(BufferUsageHint usageHint)
            : base(usageHint)
        {
        }

        protected unsafe override void BufferData(VertexPCT[] data, BufferUsageHint usageHint, int start, int count)
        {
            fixed (VertexPCT* ptr = data)
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              (IntPtr)(count * sizeof(VertexPCT)),
                              (IntPtr)ptr + start * sizeof(VertexPCT),
                              usageHint);
            }
        }
        protected unsafe override void BufferSubData(VertexPCT[] data, int gpuOffset, int start, int count)
        {
            fixed (VertexPCT* ptr = data)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)(gpuOffset * sizeof(VertexPCT)),
                                 (IntPtr)(count * sizeof(VertexPCT)),
                                 (IntPtr)ptr + start * sizeof(VertexPCT));
            }
        }
        protected unsafe override void DrawVertices(RenderTarget target, TexturedPrimitiveShader shader, ref VertexBufferPCTDrawOptions options)
        {
            target.PreDrawSetup(shader, options.Blending, options.Transform);

            //if (shader.UniformTextureMatrixLocation >= 0)
            if (options.Texture != null)
            {
                options.Texture.Bind();

                Matrix4 texMatrix = options.Texture.TextureMatrix;
                GL.UniformMatrix4(shader.UniformTextureMatrixLocation, false, ref texMatrix);
            }

            GL.EnableVertexAttribArray(shader.AttributeVertexPositionLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexColorLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexTexCoordsLocation);

            GL.VertexAttribPointer(shader.AttributeVertexPositionLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPCT), 0);
            GL.VertexAttribPointer(shader.AttributeVertexColorLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(VertexPCT), 8);
            GL.VertexAttribPointer(shader.AttributeVertexTexCoordsLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPCT), 12);

            if (options.IndexBuffer != null)
            {
                options.IndexBuffer.Bind();
                GL.DrawElements(options.PrimitiveType, options.Count, options.IndexBuffer.ElementsType, options.Start);
            }
            else
            {
                IndexBuffer.Unbind();
                GL.DrawArrays(options.PrimitiveType, options.Start, options.Count);
            }

            GL.DisableVertexAttribArray(shader.AttributeVertexPositionLocation);
            GL.DisableVertexAttribArray(shader.AttributeVertexColorLocation);
            GL.DisableVertexAttribArray(shader.AttributeVertexTexCoordsLocation);
        }
    }
}
