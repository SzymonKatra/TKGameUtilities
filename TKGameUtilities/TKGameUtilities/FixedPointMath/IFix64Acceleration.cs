
namespace TKGameUtilities.FixedPointMath
{
    /// <summary>
    /// Interface that definies acceleration of any object
    /// </summary>
    public interface IFix64Acceleration
    {
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        void ApplyAcceleration(ref Fix64Velocity velocity);
        /// <summary>
        /// Applies acceleration to velocity
        /// </summary>
        /// <param name="velocity">Velocity</param>
        /// <returns>Result</returns>
        Fix64Velocity ApplyAcceleration(Fix64Velocity velocity);
    }
}
