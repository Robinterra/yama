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
                isok = this.CheckAllOperators ( lexer );
                if (isok) firstrun = false;
            }

            return firstrun ? TokenStatus.Cancel : TokenStatus.Complete;
        }

        // -----------------------------------------------

        private bool CheckAllOperators ( Lexer lexer )
        {
            foreach ( ILexerToken token in this.operators )
            {
                TokenStatus status = token.CheckChar ( lexer );

                if (status == TokenStatus.CompleteOne)
                {
                    lexer.NextChar (  );
                    status = TokenStatus.Complete;
                }

                if (status != TokenStatus.Complete) continue;

                return true;
            }

            return false;
        }

        // -----------------------------------------------

        public object GetValue ( byte[] daten )
        {
            return System.Text.Encoding.UTF8.GetString ( daten );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --