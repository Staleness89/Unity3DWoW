
    /// <summary>
    ///     A color given in values of red, green, blue and alpha.
    /// </summary>
    public struct CArgb
    {
        public readonly byte R, G, B, A;

        public CArgb(byte p1, byte p2, byte p3, byte p4)
        {
            R = p1;
            G = p2;
            B = p3;
            A = p4;
        }

        public override string ToString()
        {
            return $"(R:{R} G:{G} B:{B} A:{A})";
        }
    }
