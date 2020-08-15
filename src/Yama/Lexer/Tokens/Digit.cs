namespace Yama.Lexer
{
    public class Digit : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.NumberToken;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

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