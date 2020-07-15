namespace Yama.Lexer
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
            lexer.CurrentCharMode (  );

            if (!char.IsWhiteSpace ( lexer.CurrentChar)) return TokenStatus.Cancel;

            while ( char.IsWhiteSpace ( lexer.CurrentChar ) )
            {
                lexer.NextChar (  );
            }

            return TokenStatus.Complete;
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