using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class VertexBufferPositionColor : VertexBuffer<VertexPositionColor, ColoredPrimitiveShader>
    {
        public VertexBufferPositionColor(BufferUsageHint usageHint)
            : base(usageHint)
        {
        }

        protected unsafe override void BufferData(VertexPositionColor[] data, BufferUsageHint usageHint, int start, int count)
        {
            fixed (VertexPositionColor* ptr = data)
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              (IntPtr)(count * sizeof(VertexPositionColor)),
                              (IntPtr)ptr + start * sizeof(VertexPositionColor),
                              usageHint);
            }
        }
        protected unsafe override void BufferSubData(VertexPositionColor[] data, int gpuOffset, int start, int count)
        {
            fixed (VertexPositionColor* ptr = data)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)(gpuOffset * sizeof(VertexPositionColor)),
                                 (IntPtr)(count * sizeof(VertexPositionColor)),
                                 (IntPtr)ptr + start * sizeof(VertexPositionColor));
            }
        }
        protected unsafe override void DrawVertices(RenderTarget target, ColoredPrimitiveShader shader, ref VertexBufferDrawOptions options)
        {
            target.PreDrawSetup(shader, options.Blending, options.Transform, null, -1);

            GL.EnableVertexAttribArray(shader.AttributeVertexPositionLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexColorLocation);

            GL.VertexAttribPointer(shader.AttributeVertexPositionLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPositionColor), 0);
            GL.VertexAttribPointer(shader.AttributeVertexColorLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(VertexPositionColor), 8);

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
