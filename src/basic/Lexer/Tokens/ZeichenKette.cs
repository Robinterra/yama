using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ZeichenKette : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private string Word
        {
            get;
        }

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.KeyWord;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public ZeichenKette ( string keyword )
        {
            this.Word = keyword;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            foreach ( char zeichen in this.Word )
            {
                if (lexer.CurrentChar != zeichen) return TokenStatus.Cancel;

                lexer.NextChar();
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            return text;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --