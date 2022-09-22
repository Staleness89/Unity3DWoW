using System;
using System.Collections.Generic;
using System.Diagnostics;


    /// <summary>
    ///     A two component float vector.
    /// </summary>
    public struct C2Vector : IEquatable<C2Vector>
    {
        public readonly float X, Y;

        public C2Vector(float p1, float p2)
        {
            X = p1;
            Y = p2;
        }

        public C2Vector(IReadOnlyList<float> p)
        {
            Debug.Assert(p.Count >= 2, "float[] is too small to create a C2Vector");
            X = p[0];
            Y = p[1];
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals((C2Vector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(C2Vector left, C2Vector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(C2Vector left, C2Vector right)
        {
            return !left.Equals(right);
        }

        public bool Equals(C2Vector other)
        {
            return X.Equals(other.X)
                   && Y.Equals(other.Y);
        }


        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
