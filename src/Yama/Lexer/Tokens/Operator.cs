using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator : ILexerToken
    {

        // -----------------------------------------------

        private List<ILexerToken> operators;

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public SyntaxKind Kind
        {
            get
            {
                return SyntaxKind.Operator;
            }
        }

        // -----------------------------------------------

        public List<ILexerToken> Operators
        {
            get
            {
                if (this.operators != null) return this.operators;

                this.operators = new List<ILexerToken>();

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

        public TokenStatus CheckChar ( Lexer lexer )
        {
            return lexer.SubLexen ( this.Operators );
        }

        // -----------------------------------------------

        private bool ConatinsOperator ( Lexer lexer )
        {
            foreach ( ZeichenKette oper in this.Operators )
            {
                if ( oper.CheckChar ( lexer ) == TokenStatus.Complete ) return true;
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