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

        public TokenStatus CheckChar ( Lexer lexer )
        {
            bool isok = true;
            bool firstrun = true;
            while (isok)
            {
                bool allCancel = true;
                foreach ( ILexerToken token in this.operators )
                {
                    if (token.CheckChar ( lexer ) != TokenStatus.Complete) continue;

                    allCancel = false;
                    firstrun = false;
                    lexer.NextChar (  );
                }

                isok = !allCancel;
            }

            return firstrun ? TokenStatus.Cancel : TokenStatus.Complete;
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