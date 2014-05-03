using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public class FontBatch : BaseSpriteBatch, IDrawable<SpriteBatchDrawOptions, AlphaTexturedPrimitiveShader>
    {
        public void Add(string text, Font font, Vector2 position, Color color)
        {
            Vector2 currentPosition = position;
            for (int i = 0; i < text.Length; i++)
            {
                int charCode = Convert.ToInt32(text[i]);
                if (charCode == 32) // space
                {
                    currentPosition.X += font.GetGlyph(32).Width;
                    continue;
                }
                else if (charCode == 10) // \n
                {
                    currentPosition.X = position.X;
                    currentPosition.Y += font.CellSize.Y ;
                    continue;
                }
                else if (charCode == 9) // \t
                {
                    currentPosition.X += font.GetGlyph(32).Width * font.TabulatorFactor.X;
                    continue;
                }
                else if (charCode == 11) // \v
                {
                    currentPosition.Y += font.CellSize.Y * font.TabulatorFactor.Y;
                    continue;
                }
                Font.GlyphInfo glyph = font.GetGlyph(charCode);
                Add(glyph.Texture, glyph.Area, currentPosition, color, Vector2.One, Vector2.Zero, 0);
                currentPosition.X += glyph.Area.Size.X;
                if (i < text.Length - 1)
                {
                    currentPosition.X += font.GetKerning(charCode, Convert.ToInt32(text[i + 1]));
                }
            }
        }
        public void Add(string text, Font font, Vector2 position, Color color, Vector2 scale, Vector2 origin, float rotation = 0)
        {
            Vector2 currentPosition = position;
            for (int i = 0; i < text.Length; i++)
            {
                int charCode = Convert.ToInt32(text[i]);
                if (charCode == 32) // space
                {
                    currentPosition.X += font.GetGlyph(32).Width * scale.X;
                    continue;
                }
                else if (charCode == 10) // \n
                {
                    currentPosition.X = position.X;
                    currentPosition.Y += font.CellSize.Y * scale.Y;
                    continue;
                }
                else if (charCode == 9) // \t
                {
                    currentPosition.X += font.GetGlyph(32).Width * font.TabulatorFactor.X * scale.X;
                    continue;
                }
                else if (charCode == 11) // \v
                {
                    currentPosition.Y += font.CellSize.Y * font.TabulatorFactor.Y * scale.Y;
                    continue;
                }
                Font.GlyphInfo glyph = font.GetGlyph(charCode);
                if (rotation != 0)
                {
                    float distance = position.DistanceTo(currentPosition);
//#if FAST
                    Add(glyph.Texture, glyph.Area, position + new Vector2(GameMath.FCos(rotation) * distance, GameMath.FSin(rotation) * distance), color, scale, origin, rotation);
//#else
//                  float rot = GameMath.ToRadians(rotation);
//                  Add(glyph.Texture, glyph.Area, position + new Vector2((float)Math.Cos(rot) * distance, (float)Math.Cos(rot) * distance), color, scale, origin, rotation);
//#endif
                }
                else
                {
                    Add(glyph.Texture, glyph.Area, currentPosition, color, scale, origin, rotation);
                }
                currentPosition.X += glyph.Area.Size.X * scale.X;
                if (i < text.Length - 1)
                {
                    currentPosition.X += font.GetKerning(charCode, Convert.ToInt32(text[i + 1])) * scale.X;
                }
            }
        }

        public void Draw(RenderTarget target, AlphaTexturedPrimitiveShader shader, SpriteBatchDrawOptions options)
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
