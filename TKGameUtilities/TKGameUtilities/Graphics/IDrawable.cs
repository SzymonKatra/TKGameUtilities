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
    /// <typeparam name="T">Additional options</typeparam>
    /// <typeparam name="S">Shader</typeparam>
    public interface IDrawable<T, S>
    {
        void Draw(RenderTarget target, S shader, T options);
    }
}
