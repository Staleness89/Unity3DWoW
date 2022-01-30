
    /// <summary>
    ///     A four component float vector.
    /// </summary>
    public struct C4Vector
    {
        public readonly float W, X, Y, Z;

        public C4Vector(float p1 = 0, float p2 = 0, float p3 = 0, float p4 = 0)
        {
            W = p1;
            X = p2;
            Y = p3;
            Z = p4;
        }

        public override string ToString()
        {
            return $"({W},{X},{Y},{Z})";
        }
    }
