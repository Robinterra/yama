using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class KeyWord : ILexerToken
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
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public KeyWord ( string keyword, SyntaxKind keywordkind )
        {
            this.Word = keyword;
            this.Kind = keywordkind;
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

                return char.IsLetter(zeichen) ? TokenStatus.Cancel : TokenStatus.Complete;
            }

            bool isok = this.Word[this.Counter] == zeichen;

            if (!isok) return TokenStatus.Cancel;

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