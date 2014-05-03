
namespace TKGameUtilities
{
    /// <summary>
    /// Interface that definies acceleration of any object
    /// </summary>
    public interface IAcceleration
    {
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        void ApplyAcceleration(ref Velocity velocity);
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Result</returns>
        Velocity ApplyAcceleration(Velocity velocity);
    }
}
