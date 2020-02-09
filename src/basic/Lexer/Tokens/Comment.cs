using System;

namespace LearnCsStuf.Basic
{
    public class Comment : ILexerToken
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
                return SyntaxKind.Comment;
            }
        }

        // -----------------------------------------------

        public ZeichenKette Begin
        {
            get;
        }

        // -----------------------------------------------

        public ZeichenKette End
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Comment ( ZeichenKette begin, ZeichenKette end )
        {
            this.Begin =  begin;

            this.End = end;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            if (this.isonEscape && !this.actuallyOnZeichenkette)
            {
                TokenStatus status = this.Begin.CheckChar(zeichen, true);

                if (status == TokenStatus.Accept) return TokenStatus.Accept;

                this.isonEscape = false;

                if (status == TokenStatus.Cancel) return TokenStatus.Cancel;

                this.actuallyOnZeichenkette = true;

                return TokenStatus.Accept;
            }

            if ( !this.actuallyOnZeichenkette )
            {
                if (this.Begin.CheckChar(zeichen, kettenauswertung) != TokenStatus.Accept) return TokenStatus.Cancel;

                if (kettenauswertung) this.isonEscape = true;

                return TokenStatus.Accept;
            }

            if (this.isonEscape)
            {
                TokenStatus status = this.End.CheckChar(zeichen, true);

                if (status == TokenStatus.Accept) return TokenStatus.Accept;

                this.isonEscape = false;

                if (status == TokenStatus.Cancel) return TokenStatus.Accept;

                this.actuallyOnZeichenkette = false;

                return TokenStatus.CompleteOne;
            }

            if (this.End.CheckChar(zeichen, true) == TokenStatus.Accept) this.isonEscape = true;

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