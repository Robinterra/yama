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
            bool isonEscape = false;

            while ( !this.CheckSplit ( lexer ) )
            {
                isonEscape = this.Escape.CheckChar ( lexer ) == TokenStatus.Complete;

                lexer.NextChar (  );

                if ( lexer.CurrentChar == '\0' ) return TokenStatus.Complete;
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            Lexer lexer = new Lexer ( text );
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