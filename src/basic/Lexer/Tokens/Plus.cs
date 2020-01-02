namespace LearnCsStuf.Basic
{
    public class Plus : ILexerToken
    {
        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.PlusToken;
            }
        }

        public bool CheckChar ( char zeichen )
        {
            return '+' == zeichen;
        }

        public object GetValue ( string text )
        {
            //if (!int.TryParse ( text, out int result )) return 0;

            return text;
        }
    }
}