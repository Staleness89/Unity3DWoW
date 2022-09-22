
    /// <summary>
    ///     A four floats quaternion.
    /// </summary>
    public struct C4Quaternion
    {
        public readonly float X, Y, Z, W;

        public C4Quaternion(float p1, float p2, float p3, float p4)
        {
            X = p1;
            Y = p2;
            Z = p3;
            W = p4;
        }

        public static explicit operator M2CompQuat(C4Quaternion quat)
        {
            return new M2CompQuat(FloatToShort(quat.X), FloatToShort(quat.Y), FloatToShort(quat.Z), FloatToShort(quat.W));
        }

        /// <summary>
        ///     Compress a float in a short
        /// </summary>
        /// <param name="value">Float to compress.</param>
        /// <returns>A short, compressed version of value.</returns>
        private static short FloatToShort(float value)
        {
            return (short) (value > 0 ? value*32767.0 - 32768 : value*32767.0 + 32768);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z},{W})";
        }
    }
