using System;
using System.Collections.Generic;

namespace Yama.Lexer
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

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Word;
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

        public TokenState CheckChar ( Lexer lexer )
        {
            bool isok = true;
            bool firstrun = true;
            while (isok)
            {
                isok = this.CheckAllOperators ( lexer );
                if (isok) firstrun = false;
            }

            return firstrun ? TokenState.Cancel : TokenState.Complete;
        }

        // -----------------------------------------------

        private bool CheckAllOperators ( Lexer lexer )
        {
            foreach ( ILexerToken token in this.operators )
            {
                TokenState status = token.CheckChar ( lexer );

                if (status == TokenState.CompleteOne)
                {
                    lexer.NextByte (  );
                    status = TokenState.Complete;
                }

                if (status != TokenState.Complete) continue;

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