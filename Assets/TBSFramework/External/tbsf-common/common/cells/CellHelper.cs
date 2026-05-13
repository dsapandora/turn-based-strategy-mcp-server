using System;

namespace TurnBasedStrategyFramework.Common.Cells
{
    /// <summary>
    /// Provides utility methods for comparing and hashing ICell instances based on their GridCoordinates.
    /// Centralizes equality logic to ensure consistent behavior across different ICell implementations.
    /// </summary>
    public static class CellHelper
    {
        /// <summary>
        /// Determines whether two <see cref="ICell"/> instances are equal by comparing their <see cref="ICell.GridCoordinates"/>.
        /// Handles null references safely and returns true if both references are the same.
        /// </summary>
        /// <param name="a">The first cell to compare.</param>
        /// <param name="b">The second cell to compare.</param>
        /// <returns>True if both cells are equal or both null; otherwise, false.</returns>
        public static bool Equals(ICell a, ICell b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.GridCoordinates.Equals(b.GridCoordinates);
        }

        /// <summary>
        /// Determines whether an <see cref="ICell"/> instance is equal to an arbitrary object.
        /// Returns false if the object is not an <see cref="ICell"/> or if GridCoordinates differ.
        /// </summary>
        /// <param name="a">The cell instance.</param>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if <paramref name="obj"/> is an <see cref="ICell"/> equal to <paramref name="a"/>; otherwise false.</returns>
        public static bool Equals(ICell a, object obj)
        {
            if (ReferenceEquals(a, obj)) return true;
            if (obj is not ICell other) return false;
            return Equals(a, other);
        }

        /// <summary>
        /// Returns a hash code for the given <see cref="ICell"/> based on its <see cref="ICell.GridCoordinates"/>.
        /// Throws <see cref="ArgumentNullException"/> if the cell is null.
        /// </summary>
        /// <param name="cell">The cell instance to get the hash code for.</param>
        /// <returns>An integer hash code representing the cell's coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cell"/> is null.</exception>
        public static int GetHashCode(ICell cell)
        {
            if (cell is null) throw new ArgumentNullException(nameof(cell));
            return cell.GridCoordinates.GetHashCode();
        }
    }
}