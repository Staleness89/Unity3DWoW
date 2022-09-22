
    /// <summary>
    ///     An axis aligned sphere described by position and radius.
    /// </summary>
    public struct CAaSphere
    {
        public readonly C3Vector Position;
        public readonly float Radius;

        public CAaSphere(C3Vector pos, float rad)
        {
            Position = pos;
            Radius = rad;
        }

        public override string ToString()
        {
            return $"Pos:{Position} Radius:{Radius}";
        }
    }
