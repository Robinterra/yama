using System;

namespace LearnCsStuf.Basic
{
    public class BasicExpressionEvaluator
    {
        public string File
        {
            get;
            set;
        }

        public string ExpressionLine
        {
            get;
            set;
        }

        private Lexer Tokenizer
        {
            get;
            set;
        }

        private bool InitLexer (  )
        {
            this.Tokenizer = new Lexer ( this.ExpressionLine );

            this.Tokenizer.LexerTokens.Add ( new Digit (  ) );

            return true;
        }

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
    }

}