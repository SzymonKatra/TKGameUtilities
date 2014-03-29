using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class SpriteBatch : BaseSpriteBatch, IDrawable<SpriteBatchDrawOptions, TexturedPrimitiveShader>
    {
        public void Draw(RenderTarget target, TexturedPrimitiveShader shader, SpriteBatchDrawOptions options)
        {
            int current = 0;
            VertexBufferDrawOptions vbOptions = new VertexBufferDrawOptions()
            {
                Blending = options.Blending,
                Transform = options.Transform,
                PrimitiveType = PrimitiveType.Quads,
            };
            for (int i = 0; i < QueueCount; i++)
            {
                BatchItem item = DrawQueue[i];

                VertexBuffer.UpdateData(Vertices, current, item.Count);

                vbOptions.Texture = item.Texture;
                vbOptions.Start = 0;
                vbOptions.Count = item.Count;

                VertexBuffer.Draw(target, shader, vbOptions);

                current += item.Count;
            }
        }
    }
}
