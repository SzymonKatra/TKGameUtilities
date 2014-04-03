using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class VertexBufferPositionColorTexCoords : VertexBuffer<VertexPositionColorTexCoords, TexturedPrimitiveShader>
    {
        public VertexBufferPositionColorTexCoords(BufferUsageHint usageHint)
            : base(usageHint)
        {
        }

        protected unsafe override void BufferData(VertexPositionColorTexCoords[] data, BufferUsageHint usageHint, int start, int count)
        {
            fixed (VertexPositionColorTexCoords* ptr = data)
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                              (IntPtr)(count * sizeof(VertexPositionColorTexCoords)),
                              (IntPtr)ptr + start * sizeof(VertexPositionColorTexCoords),
                              usageHint);
            }
        }
        protected unsafe override void BufferSubData(VertexPositionColorTexCoords[] data, int gpuOffset, int start, int count)
        {
            fixed (VertexPositionColorTexCoords* ptr = data)
            {
                GL.BufferSubData(BufferTarget.ArrayBuffer,
                                 (IntPtr)(gpuOffset * sizeof(VertexPositionColorTexCoords)),
                                 (IntPtr)(count * sizeof(VertexPositionColorTexCoords)),
                                 (IntPtr)ptr + start * sizeof(VertexPositionColorTexCoords));
            }
        }
        protected unsafe override void DrawVertices(RenderTarget target, TexturedPrimitiveShader shader, ref VertexBufferDrawOptions options)
        {
            target.PreDrawSetup(shader, options.Blending, options.Transform, options.Texture, shader.UniformTextureMatrixLocation);

            GL.EnableVertexAttribArray(shader.AttributeVertexPositionLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexColorLocation);
            GL.EnableVertexAttribArray(shader.AttributeVertexTexCoordsLocation);

            GL.VertexAttribPointer(shader.AttributeVertexPositionLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPositionColorTexCoords), 0);
            GL.VertexAttribPointer(shader.AttributeVertexColorLocation, 4, VertexAttribPointerType.UnsignedByte, true, sizeof(VertexPositionColorTexCoords), 8);
            GL.VertexAttribPointer(shader.AttributeVertexTexCoordsLocation, 2, VertexAttribPointerType.Float, false, sizeof(VertexPositionColorTexCoords), 12);

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
