using System;

namespace LearnCsStuf.Basic
{
    public class BasicExpressionEvaluator
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string File
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string ExpressionLine
        {
            get;
            set;
        }

        // -----------------------------------------------

        private Lexer Tokenizer
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private bool InitLexer (  )
        {
            this.Tokenizer = new Lexer ( this.ExpressionLine );

            this.Tokenizer.LexerTokens.Add ( new Digit (  ) );
            this.Tokenizer.LexerTokens.Add ( new Whitespaces (  ) );
            this.Tokenizer.LexerTokens.Add ( new Plus (  ) );
            this.Tokenizer.LexerTokens.Add ( new Sternchen (  ) );

            return true;
        }

        // -----------------------------------------------

        public bool DoStuff()
        {
            this.InitLexer (  );

            foreach (SyntaxToken token in this.Tokenizer)
            {
                if (token.Kind == SyntaxKind.Unknown) continue;

                Console.WriteLine ( token.Text );
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --