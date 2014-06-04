using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    /// <summary>
    /// PC - PositionColor
    /// </summary>
    public struct VertexBufferPCDrawOptions
    {
        public BlendOptions Blending;
        public Matrix4 Transform;
        public PrimitiveType PrimitiveType;
        public int Start;
        public int Count;
        public IndexBuffer IndexBuffer;

        public static readonly VertexBufferPCDrawOptions Default = new VertexBufferPCDrawOptions()
        {
            Blending = BlendOptions.Default,
            Transform = Matrix4.Identity,
            PrimitiveType = PrimitiveType.Points,
            Start = 0,
            Count = 0,
            IndexBuffer = null,
        };
    }

    /// <summary>
    /// PC - PositionColor
    /// </summary>
    public class VertexBufferPC : VertexBuffer<VertexPC, ColoredPrimitiveShader, VertexBufferPCDrawOptions>
    {
        public VertexBufferPC(BufferUsageHint usageHint)
            : base(usageHint)
        {
        }

        protected unsafe override void BufferData(VertexPC[] data, BufferUsageHint usageHint, int start, int count)
        {
            fixed (VertexPC* ptr = data)
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              (IntPtr)(count * sizeof(VertexPC)),
                              (IntPtr)ptr + start * sizeof(VertexPC),
                              usageHint);
            }
        }
        protected unsafe override void BufferSubData(VertexPC[] data, int gpuOffset, int start, int count)
        {
            fixed (VertexPC* ptr = data)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)(gpuOffset * sizeof(VertexPC)),
                                 (IntPtr)(count * sizeof(VertexPC)),
                                 (IntPtr)ptr + start * sizeof(VertexPC));
            }
        }
        protected unsafe override void DrawVertices(RenderTarget target, ColoredPrimitiveShader shader, ref VertexBufferPCDrawOptions options)
        {
            target.PreDrawSetup(shader, options.Blending, options.Transform);

            GL.EnableVertexAttribArray(shader.AttributeVertexPositionLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexColorLocation);

            GL.VertexAttribPointer(shader.AttributeVertexPositionLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPC), 0);
            GL.VertexAttribPointer(shader.AttributeVertexColorLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(VertexPC), 8);

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
        }
    }
}
