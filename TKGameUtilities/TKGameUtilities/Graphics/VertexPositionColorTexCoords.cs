using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTexCoords
    {
        public Vector2 Position;
        public Color Color;
        public Vector2 TexCoords;
    }
}
