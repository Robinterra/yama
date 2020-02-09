using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class LowerAlpabet : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.LowerAlpabet;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            if (!char.IsLetter ( zeichen )) return kettenauswertung ? TokenStatus.Complete : TokenStatus.Cancel;

            return char.IsLower ( zeichen ) ? TokenStatus.Accept : TokenStatus.Cancel;
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