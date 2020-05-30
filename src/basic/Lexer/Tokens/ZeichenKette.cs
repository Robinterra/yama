using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ZeichenKette : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Word
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

        public int Length { get; internal set; }

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