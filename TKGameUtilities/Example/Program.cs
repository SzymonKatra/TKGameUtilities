using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using TKGameUtilities;
using TKGameUtilities.Graphics;

namespace Example
{
    class Program
    {
        private static bool m_spaceToggle;

        static void Main(string[] args)
        {
            Window window = new Window(new Point2(800, 600), "TKGameUtilities example", false, GraphicsMode.Default);
            window.KeyboardDefault.KeyDown += KeyboardDefault_KeyDown;
            window.Show();

            ColoredPrimitiveShader coloredPrimitiveShader = new ColoredPrimitiveShader();
            AlphaTexturedPrimitiveShader alphaTexturedPrimitiveShader = new AlphaTexturedPrimitiveShader();
            TexturedPrimitiveShader texturedPrimitiveShader = new TexturedPrimitiveShader();

            IndexedPrimitiveBatch primitiveBatch = new IndexedPrimitiveBatch();
            
            Font font = new Font(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\arial.ttf", 16);
            FontBatch fontBatch = new FontBatch();

            Texture texture = new Texture("sprite.png");
            SpriteBatch spriteBatch = new SpriteBatch();

            while (window.Visible)
            {
                window.ProcessEvents();

                window.Clear();

                primitiveBatch.Begin();
                primitiveBatch.AddLine(new Vector2(100, 100), new Vector2(300, 300), Color.Cyan, Color.Orange);
                primitiveBatch.End();
                primitiveBatch.Draw(window, coloredPrimitiveShader, PrimitiveBatchDrawOptions.Default);

                fontBatch.Begin();
                fontBatch.Add("This is example text using TTF fonts", font, new Vector2(400, 300), Color.Magenta, new Vector2(1.2f, 1.5f), Vector2.Zero, 30f);
                if (m_spaceToggle) fontBatch.Add("Space key was pressed", font, new Vector2(100, 50), Color.White);
                fontBatch.End();
                fontBatch.Draw(window, alphaTexturedPrimitiveShader, SpriteBatchDrawOptions.Default);

                spriteBatch.Begin();
                spriteBatch.Add(texture, texture.TextureRectangle, new Vector2(200, 300), Color.White, Vector2.One, texture.TextureRectangle.Size / 2, 200f);
                spriteBatch.End();
                spriteBatch.Draw(window, texturedPrimitiveShader, SpriteBatchDrawOptions.Default);

                window.Display();
            }

            coloredPrimitiveShader.Dispose();
            alphaTexturedPrimitiveShader.Dispose();
            primitiveBatch.Dispose();
            font.Dispose();
            fontBatch.Dispose();

            window.Close();
            window.Dispose();
        }

        private static void KeyboardDefault_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Space) m_spaceToggle = !m_spaceToggle;
        }
    }
}
