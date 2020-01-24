namespace LearnCsStuf.Basic
{
    public class Plus : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.PlusToken;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool CheckChar ( char zeichen )
        {
            return '+' == zeichen;
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