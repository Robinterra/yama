namespace LearnCsStuf.Basic
{
    public class Digit : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.NumberToken;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( !char.IsDigit ( lexer.CurrentChar ) ) return TokenStatus.Cancel;

            while ( char.IsDigit ( lexer.CurrentChar ) )
            {
                lexer.NextChar (  );
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            string text = System.Text.Encoding.UTF8.GetString ( daten );

            if (!int.TryParse ( text, out int result )) return 0;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --