using System;

    /// <summary>
    ///     A three component float vector.
    /// </summary>
    public struct C3Vector : IEquatable<C3Vector>
    {
        public readonly float X, Y, Z;

        public C3Vector(float p1, float p2, float p3)
        {
            X = p1;
            Y = p2;
            Z = p3;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals((C3Vector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(C3Vector left, C3Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C3Vector left, C3Vector right)
        {
            return !left.Equals(right);
        }

    public static implicit operator C3Vector(M2Array<C3Vector> v)
    {
        throw new NotImplementedException();
    }

    public bool Equals(C3Vector other)
        {
            return X.Equals(other.X)
                   && Y.Equals(other.Y)
                   && Z.Equals(other.Z);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
