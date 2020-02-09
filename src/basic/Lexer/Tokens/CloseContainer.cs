namespace LearnCsStuf.Basic
{
    public class CloseContainer : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.CloseContainer;
            }
        }

        // -----------------------------------------------

        private char zeichen
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public CloseContainer ( char zeichen )
        {
            this.zeichen = zeichen;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            bool isok = this.zeichen == zeichen;

            if (kettenauswertung) return isok ? TokenStatus.CompleteOne : TokenStatus.SyntaxError;

            return isok ? TokenStatus.Accept : TokenStatus.Cancel;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            //if (!int.TryParse ( text, out int result )) return 0;

            return text;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --