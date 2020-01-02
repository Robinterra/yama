namespace LearnCsStuf.Basic
{
    public class SyntaxToken
    {

        #region get/set

        public SyntaxKind Kind
        {
            get;
        }

        public int Position
        {
            get;
        }
        public string Text
        {
            get;
        }
        private object Value
        {
            get;
        }

        #endregion get/set

        #region ctor

        public SyntaxToken ( SyntaxKind kind, int position, string text, object value )
        {
            this.Kind = kind;
            this.Position = position;
            this.Text = text;
            this.Value = value;
        }

        #endregion ctor
    }

    public enum SyntaxKind
    {
        Unknown,
        NumberToken,
        Whitespaces,
        PlusToken,
        Subtraktion,
        SternchenToken
    }
}