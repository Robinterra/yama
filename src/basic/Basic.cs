using System;
using System.Collections.Generic;

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

            this.Tokenizer.LexerTokens.Add ( new Operator ( '+', '-', '*', '/', '%', '&', '|', '=', '<', '>', '!', '^', '~' ) );
            this.Tokenizer.LexerTokens.Add ( new Digit (  ) );
            this.Tokenizer.LexerTokens.Add ( new Whitespaces (  ) );
            this.Tokenizer.LexerTokens.Add ( new OpenKlammer (  ) );
            this.Tokenizer.LexerTokens.Add ( new CloseKlammer (  ) );
            this.Tokenizer.LexerTokens.Add ( new Text (  ) );
            this.Tokenizer.LexerTokens.Add ( new Point (  ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "int" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "string" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "char" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "byte" ) );
            this.Tokenizer.LexerTokens.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );
            //this.Tokenizer.LexerTokens.Add ( new Plus (  ) );
            //this.Tokenizer.LexerTokens.Add ( new Sternchen (  ) );

            return true;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(SyntaxToken token)
        {
            if (token.Kind != SyntaxKind.Unknown) return false;

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "({0},{1}) Syntax error - unknown char \"{2}\"", token.Line, token.Column, token.Text );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        public bool DoStuff()
        {
            this.InitLexer (  );

            foreach (SyntaxToken token in this.Tokenizer)
            {
                if (this.PrintSyntaxError(token)) continue;

                Console.Write ( token.Kind.ToString() + " : " );
                Console.WriteLine ( token.Value );
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --