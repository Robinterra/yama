using System.Collections.Generic;

namespace Yama.Lexer
{
    public class Operator : ILexerToken
    {

        // -----------------------------------------------

        private List<ILexerToken> operators;

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public IdentifierKind Kind
        {
            get
            {
                return IdentifierKind.Operator;
            }
        }

        // -----------------------------------------------

        public List<ILexerToken> Operators
        {
            get
            {
                return this.operators;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Operator ( params ZeichenKette[] param )
        {
            this.operators = new List<ILexerToken>();

            foreach (ZeichenKette zeichen in param)
            {
                this.Operators.Add ( zeichen );
            }
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public TokenState CheckChar ( Lexer lexer )
        {
            return lexer.SubLexen ( this.Operators );
        }

        // -----------------------------------------------

        private bool ConatinsOperator ( Lexer lexer )
        {
            foreach ( ZeichenKette oper in this.Operators )
            {
                if ( oper.CheckChar ( lexer ) == TokenState.Complete ) return true;
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