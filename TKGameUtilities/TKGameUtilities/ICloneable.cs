using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKGameUtilities
{
    /// <summary>
    /// Interface that defines object that can be deep cloned
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Deep clone
        /// </summary>
        /// <returns>Cloned object</returns>
        T Clone();
    }
}
