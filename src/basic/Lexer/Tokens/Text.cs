using System;
using System.Text;

namespace LearnCsStuf.Basic
{
    public class Text : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Text;
            }
        }

        // -----------------------------------------------

        public ZeichenKette Begin
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ZeichenKette Ende
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

        #region ctor

        // -----------------------------------------------

        public Text ( ZeichenKette begin, ZeichenKette ende, Escaper escape )
        {
            this.Begin = begin;
            this.Ende = ende;
            this.Escape = escape;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( this.Begin.CheckChar ( lexer ) != TokenStatus.Complete ) return TokenStatus.Cancel;

            while ( true )
            {
                if ( this.Ende.CheckChar ( lexer ) == TokenStatus.Complete ) return TokenStatus.Complete;

                this.Escape.CheckChar ( lexer );

                lexer.NextChar (  );

                if ( lexer.CurrentChar == '\0' ) return TokenStatus.SyntaxError;
            }

        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            Lexer lexer = new Lexer ( text );
            lexer.LexerTokens.Add ( this.Escape );
            lexer.LexerTokens.Add ( new Replacer ( this.Begin, string.Empty ) );
            lexer.LexerTokens.Add ( new Replacer ( this.Ende, string.Empty ) );

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