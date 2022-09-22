
    /// <summary>
    ///     A four shorts (compressed) quaternion.
    /// </summary>
    public struct M2CompQuat
    {
        public readonly short X, Y, Z, W;

        public M2CompQuat(short p1, short p2, short p3, short p4)
        {
            X = p1;
            Y = p2;
            Z = p3;
            W = p4;
        }

        public static explicit operator C4Quaternion(M2CompQuat comp)
        {
            return new C4Quaternion(ShortToFloat(comp.X), ShortToFloat(comp.Y), ShortToFloat(comp.Z),
                ShortToFloat(comp.W));
        }

        /// <summary>
        ///     Decompress a short in a float.
        /// </summary>
        /// <param name="value">The short to convert.</param>
        /// <returns>A converted float value.</returns>
        private static float ShortToFloat(short value)
        {
            if (value == -1) return 1;
            return (float) ((value > 0 ? value - 32767 : value + 32767)/32767.0);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z},{W})";
        }
    }
