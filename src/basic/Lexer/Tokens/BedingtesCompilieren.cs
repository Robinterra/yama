using System;

namespace LearnCsStuf.Basic
{
    public class BedingtesCompilieren : ILexerToken
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
                return SyntaxKind.BedingtesCompilieren;
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

        public BedingtesCompilieren ( ZeichenKette begin, ZeichenKette end )
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

            if (zeichen == '\0') return TokenStatus.Complete;

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

        public object GetValue ( string text )
        {
            return text;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --