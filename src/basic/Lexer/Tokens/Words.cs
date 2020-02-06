using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Words : ILexerToken
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        private List<ILexerToken> operators
        {
            get;
        }

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Word;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        public Words ( List<ILexerToken> tokens )
        {
            this.operators = tokens;
        }

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenStatus CheckChar ( char zeichen, bool kettenauswertung )
        {
            bool isok = char.IsLetter(zeichen);

            if (kettenauswertung) return isok ? TokenStatus.Accept : TokenStatus.Complete;

            return isok ? TokenStatus.Accept : TokenStatus.Cancel;
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