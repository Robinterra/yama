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
        EndOfCommand,
        BeginContainer,
        CloseContainer,
        EckigeKlammerAuf,
        EckigeKlammerZu,
        DoublePoint,
        Comma,
        Comment,
        Int32Bit,
        Char,
        Byte,
        For,
        While,
        Boolean,
        True,
        Null,
        Enum,
        Continue,
        Break,
        False,
        Void,
        Return,
        Class,
        Static,
        Nonref,
        Private,
        Public,
        UInt32Bit,
        UInt16Bit,
        Int16Bit,
        Int64Bit,
        UInt64Bit,
        Ref,
        Is,
        In,
        As,
        Foreach,
        Float32Bit,
        New,
        Delegate,
        Using,
        This,
        Base,
        Sizeof,
        Namespace,
        BedingtesCompilieren,
        Zeichen,
    }
}

// -- [EOF] --