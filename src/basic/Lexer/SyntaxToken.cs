namespace LearnCsStuf.Basic
{
    public class SyntaxToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get;
        }

        // -----------------------------------------------

        public int Position
        {
            get;
        }

        // -----------------------------------------------

        public int Line
        {
            get;
        }

        // -----------------------------------------------

        public int  Column
        {
            get;
        }

        // -----------------------------------------------

        public string Text
        {
            get;
        }

        // -----------------------------------------------

        public object Value
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public SyntaxToken ( SyntaxKind kind, int position, int line, int column, string text, object value )
        {
            this.Kind = kind;
            this.Position = position;
            this.Line = line;
            this.Column = column;
            this.Text = text;
            this.Value = value;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

    }

    public enum SyntaxKind
    {
        Unknown,
        NumberToken,
        Whitespaces,
        PlusToken,
        Subtraktion,
        SternchenToken,
        Operator,
        OpenKlammer,
        CloseKlammer,
        Text,
        Word,
        Point,
        LowerAlpabet,
        HigherAlpabet,
        KeyWord,
        Underscore,
        Semikolon,
        GeschweifteKlammerAuf,
        GeschweifteKlammerZu,
        EckigeKlammerAuf,
        EckigeKlammerZu,
        DoublePoint,
        Comma,
    }
}

// -- [EOF] --