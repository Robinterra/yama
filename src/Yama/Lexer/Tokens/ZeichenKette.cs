using System;
using System.Collections.Generic;

namespace Yama.Lexer
{
    public class ZeichenKette : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public byte[] Data
        {
            get;
        }

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.KeyWord;
            }
        }

        public int Length { get; internal set; }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public ZeichenKette ( string keyword )
        {
            this.Data = System.Text.Encoding.UTF8.GetBytes ( keyword );
        }

        // -----------------------------------------------

        public ZeichenKette ( byte[] data )
        {
            this.Data = data;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            foreach ( byte zeichen in this.Data )
            {
                if (lexer.CurrentByte != zeichen) return TokenStatus.Cancel;

                lexer.NextByte (  );
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