namespace LearnCsStuf.Basic
{
    public class Digit : ILexerToken
    {
        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.NumberToken;
            }
        }

        public bool CheckChar ( char zeichen )
        {
            return char.IsDigit(zeichen);
        }

        public object GetValue ( string text )
        {
            if (!int.TryParse ( text, out int result )) return 0;

            return result;
        }
    }
}