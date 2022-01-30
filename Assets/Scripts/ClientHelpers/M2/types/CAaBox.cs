
    /// <summary>
    ///     An axis aligned box described by the minimum and maximum point.
    /// </summary>
    public struct CAaBox
    {
        public readonly C3Vector Min, Max;

        public CAaBox(C3Vector min, C3Vector max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"{Min}->{Max}";
        }
    }
