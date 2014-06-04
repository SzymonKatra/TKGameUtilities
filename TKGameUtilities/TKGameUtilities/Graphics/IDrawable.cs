using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKGameUtilities.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S">Shader</typeparam>
    /// <typeparam name="O">Additional options</typeparam>
    public interface IDrawable<S, O>
    {
        void Draw(RenderTarget target, S shader, O options);
    }
}
