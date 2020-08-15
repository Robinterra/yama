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

        public TokenStatus CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

            if (!char.IsLetter ( lexer.CurrentChar )) return TokenStatus.Cancel;

            return char.IsUpper ( lexer.CurrentChar ) ? TokenStatus.CompleteOne : TokenStatus.Cancel;
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