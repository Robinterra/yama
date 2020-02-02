namespace LearnCsStuf.Basic
{
    public interface ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        SyntaxKind Kind
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        TokenStatus CheckChar ( char zeichen, bool kettenauswertung );

        // -----------------------------------------------

        object GetValue ( string text );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --