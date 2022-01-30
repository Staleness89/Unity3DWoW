
    /// <summary>
    ///     A four by four matrix.
    /// </summary>
    public class C44Matrix
    {
        public readonly C4Vector[] Columns = new C4Vector[4];

        public C44Matrix() : this(new C4Vector(), new C4Vector(), new C4Vector(), new C4Vector()) { }

        public C44Matrix(C4Vector col0, C4Vector col1, C4Vector col2, C4Vector col3)
        {
            Columns[0] = col0;
            Columns[1] = col1;
            Columns[2] = col2;
            Columns[3] = col3;
        }

        public override string ToString()
        {
            return $"({Columns[0]},{Columns[1]},{Columns[2]},{Columns[3]})";
        }
    }
