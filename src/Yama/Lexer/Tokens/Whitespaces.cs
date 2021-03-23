namespace Yama.Lexer
{
    public class Whitespaces : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Whitespaces;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

            if (!char.IsWhiteSpace ( lexer.CurrentChar)) return TokenState.Cancel;

            while ( char.IsWhiteSpace ( lexer.CurrentChar ) )
            {
                lexer.NextChar (  );
            }

            return TokenState.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            return System.Text.Encoding.UTF8.GetString ( daten );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --