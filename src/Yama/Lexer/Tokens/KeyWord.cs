using System;
using System.Collections.Generic;

namespace Yama.Lexer
{
    public class KeyWord : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private string Word
        {
            get;
        }

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public KeyWord ( string keyword, IdentifierKind keywordkind )
        {
            this.Word = keyword;
            this.Kind = keywordkind;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            lexer.CurrentCharMode (  );

            foreach ( char zeichen in this.Word )
            {
                if (lexer.CurrentChar != zeichen) return TokenStatus.Cancel;

                lexer.NextChar();
            }

            return char.IsLetter(lexer.CurrentChar) ? TokenStatus.Cancel : TokenStatus.Complete;
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