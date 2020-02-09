using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ZeichenKette : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private string Word
        {
            get;
        }

        // -----------------------------------------------

        private int Counter
        {
            get;
            set;
        }

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.KeyWord;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public ZeichenKette ( string keyword )
        {
            this.Word = keyword;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            if (!kettenauswertung) return this.Word[0] == zeichen ? TokenStatus.Accept : TokenStatus.Cancel;

            if (this.Counter >= this.Word.Length)
            {
                this.Counter = 0;

                return TokenStatus.Complete;
            }

            bool isok = this.Word[this.Counter] == zeichen;

            if (!isok) { this.Counter = 0; return TokenStatus.Cancel; }

            this.Counter += 1;

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