namespace LearnCsStuf.Basic
{
    public class EndOfCommand : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private char zeichen
        {
            get;
        }

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.EndOfCommand;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public EndOfCommand ( char zeichen )
        {
            this.zeichen = zeichen;
        }

        // -----------------------------------------------

        #endregion ctor

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