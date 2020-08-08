using System.IO;
using Yama.Parser;
namespace Yama.Lexer
{
    public class SyntaxToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int Position
        {
            get;
            set;
        }

        // -----------------------------------------------

        public int Line
        {
            get;
        }

        // -----------------------------------------------

        public int Column
        {
            get;
        }

        // -----------------------------------------------

        public FileInfo FileInfo
        {
            get;
            set;
        }

        // -----------------------------------------------

        public byte[] Data
        {
            get;
            private set;
        }

        // -----------------------------------------------

        public byte[] CleanDaten
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string Text
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString ( this.Data );
            }
            private set
            {
                this.Data = System.Text.Encoding.UTF8.GetBytes ( value );
            }
        }

        // -----------------------------------------------

        public object Value
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode ParentNode
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode Node
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public SyntaxToken()
        {

        }

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

        public SyntaxToken ( SyntaxKind kind, int position, int line, int column, byte[] data, object value )
        {
            this.Kind = kind;
            this.Position = position;
            this.Line = line;
            this.Column = column;
            this.Data = data;
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
        Set,
        Get,
        Else,
        If,
        Escape,
        Splitter,
        Replacer,
        KleinerAls,
        GroesserAls,
        OperatorKey,
        Explicit,
        Implicit,
    }
}

// -- [EOF] --