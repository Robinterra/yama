using System;
using System.Collections.Generic;

namespace Yama.Lexer
{
    public class Punctuation : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private ZeichenKette punctuation
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

        public IdentifierKind Kind
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Punctuation ( ZeichenKette punctuation, IdentifierKind punctuationkind )
        {
            this.punctuation = punctuation;
            this.Kind = punctuationkind;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            return this.punctuation.CheckChar ( lexer );

            /*if (!kettenauswertung) return this.punctuation[0] == zeichen ? TokenStatus.Accept : TokenStatus.Cancel;

            if (this.Counter >= this.punctuation.Length)
            {
                this.Counter = 0;

                if (char.IsWhiteSpace(zeichen)) return TokenStatus.Complete;

                TokenStatus returnByCharLast = TokenStatus.Complete;

                if (char.IsLetter(this.punctuation[this.punctuation.Length - 1])) returnByCharLast = TokenStatus.Cancel;

                if (char.IsLetter(zeichen)) return returnByCharLast;

                return TokenStatus.Cancel;
            }

            bool isok = this.punctuation[this.Counter] == zeichen;

            if (!isok) return TokenStatus.Cancel;

            this.Counter += 1;

            return TokenStatus.Accept;*/
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            return System.Text.Encoding.UTF8.GetString ( daten );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --