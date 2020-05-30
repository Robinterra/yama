using System;
using System.Collections.Generic;
using System.Text;

namespace LearnCsStuf.Basic
{
    public class Splitter : ILexerToken
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Splitter;
            }
        }

        // -----------------------------------------------

        public List<ZeichenKette> Split
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Escaper Escape
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Splitter ( List<ZeichenKette> split, Escaper escape )
        {
            this.Split = split;
            this.Escape = escape;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private bool CheckSplit ( Lexer lexer )
        {
            foreach ( ZeichenKette kette in this.Split )
            {
                if ( kette.CheckChar ( lexer ) == TokenStatus.Complete ) return true;
            }

            return false;
        }

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            while ( !this.CheckSplit ( lexer ) )
            {
                this.Escape.CheckChar ( lexer );

                lexer.NextChar (  );

                if ( lexer.CurrentChar == '\0' ) return TokenStatus.Complete;
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] data )
        {
            Lexer lexer = new Lexer ( new System.IO.MemoryStream ( data ) );
            lexer.LexerTokens.Add ( this.Escape );
            foreach ( ZeichenKette z in this.Split ) lexer.LexerTokens.Add ( new Replacer ( z, string.Empty ) );

            StringBuilder builder = new StringBuilder();
            foreach ( SyntaxToken token in lexer )
            {
                builder.Append ( token.Value );
            }

            return builder.ToString (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --