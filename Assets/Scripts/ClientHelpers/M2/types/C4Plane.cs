
    /// <summary>
    ///     A 3D plane defined by four floats
    /// </summary>
    public struct C4Plane
    {
        public readonly float Distance;
        public readonly C3Vector Normal;

        public C4Plane(C3Vector vec, float dist)
        {
            Normal = vec;
            Distance = dist;
        }

        public override string ToString()
        {
            return $"Normal:{Normal} Dist:{Distance}";
        }
    }
