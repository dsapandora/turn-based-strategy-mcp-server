using System;

namespace TurnBasedStrategyFramework.Common.Utilities
{
    public readonly struct Vector3IntImpl : IVectorArithmetics<Vector3IntImpl>, IEquatable<Vector3IntImpl>
    {
        public readonly int x { get; }
        public readonly int y { get; }
        public readonly int z { get; }

        public Vector3IntImpl(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public readonly Vector3IntImpl Add(Vector3IntImpl other)
        {
            return new Vector3IntImpl(x + other.x, y + other.y, z + other.z);
        }

        public readonly Vector3IntImpl Subtract(Vector3IntImpl other)
        {
            return new Vector3IntImpl(x - other.x, y - other.y, z - other.z);
        }

        public readonly override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public readonly override bool Equals(object other)
        {
            if (other is not Vector3IntImpl)
            {
                return false;
            }

            return Equals((Vector3IntImpl)other);
        }

        public readonly bool Equals(Vector3IntImpl other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public float Dot(Vector3IntImpl other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public Vector3IntImpl Normalize()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Vector3IntImpl left, Vector3IntImpl right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3IntImpl left, Vector3IntImpl right)
        {
            return !(left == right);
        }
    }
}

