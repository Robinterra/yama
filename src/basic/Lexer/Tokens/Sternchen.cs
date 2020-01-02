namespace LearnCsStuf.Basic
{
    public class Sternchen : ILexerToken
    {
        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.SternchenToken;
            }
        }

        public bool CheckChar ( char zeichen )
        {
            return '*' == zeichen;
        }

        public object GetValue ( string text )
        {
            //if (!int.TryParse ( text, out int result )) return 0;

            return text;
        }
    }
}