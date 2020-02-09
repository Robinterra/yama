using System;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Parser
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

        public List<SyntaxToken> SyntaxErrors
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
            this.Tokenizer.LexerTokens.Add ( new GeschweifteKlammerAuf (  ) );
            this.Tokenizer.LexerTokens.Add ( new GeschweifteKlammerZu (  ) );
            this.Tokenizer.LexerTokens.Add ( new EckigeKlammerZu (  ) );
            this.Tokenizer.LexerTokens.Add ( new EckigeKlammerAuf (  ) );
            this.Tokenizer.LexerTokens.Add ( new Point (  ) );
            this.Tokenizer.LexerTokens.Add ( new Comma (  ) );
            this.Tokenizer.LexerTokens.Add ( new DoublePoint (  ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "int" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "char" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "byte" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "for" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "while" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "bool" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "true" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "null" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "enum" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "continue" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "break" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "false" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "void" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "return" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "class" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "static" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "nonref" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "public" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "private" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "uint" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ushort" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "short" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "long" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ulong" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ref" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "is" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "in" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "as" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "foreach" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "float" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "new" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "delegate" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "using" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "this" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "base" ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "sizeof" ) );
            this.Tokenizer.LexerTokens.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );
            this.Tokenizer.LexerTokens.Add ( new Semikolon (  ) );

            return true;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(SyntaxToken token)
        {
            if (token.Kind != SyntaxKind.Unknown) return false;

            this.SyntaxErrors.Add ( token );

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "({0},{1}) Syntax error - unknown char \"{2}\"", token.Line, token.Column, token.Text );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        public bool Parse()
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