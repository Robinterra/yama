using System.Collections.Generic;

namespace Yama.Lexer
{
    public class LowerAlpabet : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.LowerAlpabet;
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

            return char.IsLower ( lexer.CurrentChar ) ? TokenState.CompleteOne : TokenState.Cancel;
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