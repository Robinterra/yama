namespace LearnCsStuf.Basic
{
    public class Whitespaces : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Whitespaces;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if (!char.IsWhiteSpace ( lexer.CurrentChar)) return TokenStatus.Cancel;

            bool isok = true;

            while ( isok )
            {
                isok = char.IsWhiteSpace ( lexer.CurrentChar );
                lexer.NextChar (  );
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            //if (!int.TryParse ( text, out int result )) return 0;

            return text;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --