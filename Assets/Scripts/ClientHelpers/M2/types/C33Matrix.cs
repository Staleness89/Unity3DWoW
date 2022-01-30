 /// <summary>
    ///     A three by three matrix.
    /// </summary>
    public class C33Matrix
    {
        public readonly C3Vector[] Columns = new C3Vector[3];

        public C33Matrix() : this(new C3Vector(), new C3Vector(), new C3Vector()) { }

        public C33Matrix(C3Vector col0, C3Vector col1, C3Vector col2)
        {
            //Columns = new C3Vector[3];
            Columns[0] = col0;
            Columns[1] = col1;
            Columns[2] = col2;
        }

        public override string ToString()
        {
            return $"({Columns[0]},{Columns[1]},{Columns[2]})";
        }
    }
