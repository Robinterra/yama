using System;
using System.Collections.Generic;
using System.Text;

namespace LearnCsStuf.Basic
{
    public class Escaper : ILexerToken
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private bool isonEscape = false;

        // -----------------------------------------------

        #endregion vars

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

        private char TranslateEscapeChar ( char zeichen )
        {
            if (zeichen == '"') return zeichen;
            if (zeichen == '0') return '\0';
            if (zeichen == '\\') return zeichen;
            if (zeichen == 'n') return '\n';
            if (zeichen == 't') return '\t';
            if (zeichen == 'r') return '\r';

            return zeichen;
        }

        // -----------------------------------------------

        private bool CheckEscapeChar(char zeichen, ref string result)
        {
            if (isonEscape)
            {
                result += this.TranslateEscapeChar ( zeichen );

                isonEscape = false;

                return true;
            }

            return isonEscape = zeichen == '\\';
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
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