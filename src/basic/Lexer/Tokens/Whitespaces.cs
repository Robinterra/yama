namespace LearnCsStuf.Basic
{
    public class Whitespaces : ILexerToken
    {
        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Whitespaces;
            }
        }

        public bool CheckChar ( char zeichen )
        {
            return char.IsWhiteSpace(zeichen);
        }

        public object GetValue ( string text )
        {
            //if (!int.TryParse ( text, out int result )) return 0;

            return text;
        }
    }
}