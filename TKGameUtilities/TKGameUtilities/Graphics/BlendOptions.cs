using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace TKGameUtilities.Graphics
{
    public struct BlendOptions
    {
        public BlendingFactorSrc BlendSource;
        public BlendingFactorDest BlendDestination;
        public BlendEquationMode BlendEquation;

        public static readonly BlendOptions Default = new BlendOptions()
        {
            BlendSource = BlendingFactorSrc.SrcAlpha,
            BlendDestination = BlendingFactorDest.OneMinusSrcAlpha,
            BlendEquation = BlendEquationMode.FuncAdd,
        };
    }
}
