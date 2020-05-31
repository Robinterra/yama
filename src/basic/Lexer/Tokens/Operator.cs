using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator : ILexerToken
    {

        // -----------------------------------------------

        private List<ZeichenKette> operators;

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

        public List<ZeichenKette> Operators
        {
            get
            {
                if (this.operators != null) return this.operators;

                this.operators = new List<ZeichenKette>();

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
            if ( !this.ConatinsOperator ( lexer ) ) return TokenStatus.Cancel;

            return TokenStatus.Complete;
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