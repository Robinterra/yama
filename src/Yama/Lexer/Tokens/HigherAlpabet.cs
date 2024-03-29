using System.Collections.Generic;

namespace Yama.Lexer
{
    public class HigherAlpabet : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.HigherAlpabet;
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

            if (!char.IsLetter ( lexer.CurrentChar )) return TokenState.Cancel;
            if (!char.IsUpper ( lexer.CurrentChar )) return TokenState.Cancel;

            lexer.NextChar();
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