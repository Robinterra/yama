using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator : ILexerToken
    {

        // -----------------------------------------------

        private List<char> operators;

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

        public List<char> Operators
        {
            get
            {
                if (this.operators != null) return this.operators;

                this.operators = new List<char>();

                return this.operators;
            }
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Operator ( params char[] param )
        {
            foreach (char zeichen in param)
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
            if ( !this.ConatinsOperator ( lexer.CurrentChar ) ) return TokenStatus.Cancel;

            while ( this.ConatinsOperator ( lexer.CurrentChar ) )
            {
                lexer.NextChar (  );
            }

            return TokenStatus.Complete;
        }

        // -----------------------------------------------

        private bool ConatinsOperator ( char zeichen )
        {
            foreach (char vergleichswert in this.operators)
            {
                if (zeichen == vergleichswert) return true;
            }

            return false;
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