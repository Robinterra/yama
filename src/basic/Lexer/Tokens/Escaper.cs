using System;
using System.Collections.Generic;
using System.Text;

namespace LearnCsStuf.Basic
{
    public class Escaper : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Escape;
            }
        }

        // -----------------------------------------------

        public ZeichenKette EscapeZeichen
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<Replacer> Replacers
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Escaper ( ZeichenKette escapeZeichen, List<Replacer> replaces )
        {
            this.EscapeZeichen = escapeZeichen;
            this.Replacers = replaces;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            if ( this.EscapeZeichen.CheckChar ( lexer ) != TokenStatus.Complete ) return TokenStatus.Cancel;

            foreach ( Replacer replacer in this.Replacers )
            {
                if ( replacer.CheckChar ( lexer ) == TokenStatus.Complete ) return TokenStatus.Complete;
            }

            return TokenStatus.SyntaxError;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            text = text.Substring ( this.EscapeZeichen.Word.Length );

            Lexer lexer = new Lexer ( text );
            lexer.LexerTokens.AddRange ( this.Replacers );

            StringBuilder builder = new StringBuilder();
            foreach ( SyntaxToken token in lexer )
            {
                if (token.Kind == SyntaxKind.Unknown) continue;
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