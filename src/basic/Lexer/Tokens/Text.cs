using System;

namespace LearnCsStuf.Basic
{
    public class Text : ILexerToken
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
                return SyntaxKind.Text;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( Lexer lexer )
        {
            bool actuallyOnZeichenkette = true;

            if (lexer.CurrentChar != '"') return TokenStatus.Cancel;

            while (actuallyOnZeichenkette)
            {
                lexer.NextChar();

                if (lexer.CurrentChar == '\0') return TokenStatus.SyntaxError;
                if (lexer.CurrentChar == '"' && !isonEscape) actuallyOnZeichenkette = false;

                isonEscape = lexer.CurrentChar == '\\';
            }

            lexer.NextChar();

            return TokenStatus.Complete;
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
            string result = string.Empty;

            foreach (char zeichen in text)
            {
                if ( zeichen == '"' && !isonEscape ) continue;

                if (this.CheckEscapeChar ( zeichen, ref result )) continue;

                result += zeichen;
            }

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --