using System;

namespace LearnCsStuf.Basic
{
    public class Text : ILexerToken
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        private bool actuallyOnZeichenkette = false;
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

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            if ( zeichen == '"' && !isonEscape )
            {
                if (kettenauswertung) this.actuallyOnZeichenkette = !this.actuallyOnZeichenkette;

                return TokenStatus.Accept;
            }

            if (!this.actuallyOnZeichenkette) return kettenauswertung ? TokenStatus.Complete : TokenStatus.Cancel;

            isonEscape = zeichen == '\\';

            return TokenStatus.Accept;
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