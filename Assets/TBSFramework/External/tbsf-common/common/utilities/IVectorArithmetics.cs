namespace TurnBasedStrategyFramework.Common.Utilities
{
    /// <summary>
    /// Defines basic arithmetic operations (addition and subtraction) for vectors.
    /// </summary>
    /// <typeparam name="T">The type of vector implementing this interface.</typeparam>
    public interface IVectorArithmetics<T>
    {
        /// <summary>
        /// Adds the specified vector to the current vector.
        /// </summary>
        /// <param name="other">The vector to add.</param>
        /// <returns>A new vector that is the result of the addition.</returns>
        T Add(T other);

        /// <summary>
        /// Subtracts the specified vector from the current vector.
        /// </summary>
        /// <param name="other">The vector to subtract.</param>
        /// <returns>A new vector that is the result of the subtraction.</returns>
        T Subtract(T other);

        float Dot(T other);

        T Normalize();
    }
}
