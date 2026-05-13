using System;

namespace TurnBasedStrategyFramework.Common.Utilities
{
    /// <summary>
    /// Represents a 2D integer vector, providing basic vector arithmetic and equality operations.
    /// </summary>
    public readonly struct Vector2IntImpl : IVectorArithmetics<Vector2IntImpl>, IEquatable<Vector2IntImpl>
    {
        public readonly int x { get; }
        public readonly int y { get; }

        public Vector2IntImpl(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public readonly Vector2IntImpl Add(Vector2IntImpl other)
        {
            return new Vector2IntImpl(x + other.x, y + other.y);
        }

        public readonly Vector2IntImpl Subtract(Vector2IntImpl value)
        {
            return new Vector2IntImpl(x - value.x, y - value.y);
        }

        public readonly float Dot(Vector2IntImpl other)
        {
            return x * other.x + y * other.y;
        }

        public readonly Vector2IntImpl Normalize()
        {
            throw new NotImplementedException();
        }

        public readonly override bool Equals(object other)
        {
            if (other is not Vector2IntImpl)
            {
                return false;
            }

            return Equals((Vector2IntImpl)other);
        }

        public readonly bool Equals(Vector2IntImpl other)
        {
            return other.x == x && other.y == y;
        }

        public readonly override int GetHashCode()
        {
            return (x * 397) ^ y;
        }

        public readonly override string ToString()
        {
            return $"({x}, {y})";
        }

        public static bool operator ==(Vector2IntImpl left, Vector2IntImpl right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2IntImpl left, Vector2IntImpl right)
        {
            return !(left == right);
        }
    }
}