namespace LearnCsStuf.Basic
{
    public class Digit : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.NumberToken;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            bool isok = char.IsDigit(zeichen);

            if (kettenauswertung) return isok ? TokenStatus.Accept : TokenStatus.Complete;

            return isok ? TokenStatus.Accept : TokenStatus.Cancel;
        }

        // -----------------------------------------------

        public object GetValue ( string text )
        {
            if (!int.TryParse ( text, out int result )) return 0;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --