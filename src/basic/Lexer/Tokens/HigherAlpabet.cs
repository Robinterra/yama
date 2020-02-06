using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class HigherAlpabet : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.HigherAlpabet;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            if (!char.IsUpper ( zeichen )) return kettenauswertung ? TokenStatus.Complete : TokenStatus.Cancel;

            return char.IsLetter ( zeichen ) ? TokenStatus.Accept : TokenStatus.SyntaxError;
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