namespace LearnCsStuf.Basic
{
    public class Whitespaces : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Whitespaces;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool CheckChar ( char zeichen )
        {
            return char.IsWhiteSpace(zeichen);
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