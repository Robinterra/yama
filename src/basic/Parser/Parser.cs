using System;
using System.IO;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Parser
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public FileInfo Fileinfo
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string InputText
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

        #region ctor

        // -----------------------------------------------

        private Parser (  )
        {
            this.InitLexer (  );
        }

        // -----------------------------------------------

        public Parser ( FileInfo file )
            : this (  )
        {
            this.Fileinfo = file;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private bool InitLexer (  )
        {
            this.Tokenizer = new Lexer (  );
            this.SyntaxErrors = new List<SyntaxToken> (  );

            this.Tokenizer.LexerTokens.Add ( new Comment ( new ZeichenKette ( "/*" ), new ZeichenKette ( "*/" ) ) );
            this.Tokenizer.LexerTokens.Add ( new Comment ( new ZeichenKette ( "//" ), new ZeichenKette ( "\n" ) ) );
            this.Tokenizer.LexerTokens.Add ( new Operator ( '+', '-', '*', '/', '%', '&', '|', '=', '<', '>', '!', '^', '~' ) );
            this.Tokenizer.LexerTokens.Add ( new Digit (  ) );
            this.Tokenizer.LexerTokens.Add ( new Whitespaces (  ) );
            this.Tokenizer.LexerTokens.Add ( new OpenKlammer (  ) );
            this.Tokenizer.LexerTokens.Add ( new CloseKlammer (  ) );
            this.Tokenizer.LexerTokens.Add ( new Text (  ) );
            this.Tokenizer.LexerTokens.Add ( new BeginContainer ( '{' ) );
            this.Tokenizer.LexerTokens.Add ( new CloseContainer ( '}' ) );
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
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "namespace" ) );
            this.Tokenizer.LexerTokens.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );
            this.Tokenizer.LexerTokens.Add ( new EndOfCommand ( ';' ) );

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

        private bool CheckTokens()
        {
            this.Tokenizer.Text = this.InputText;

            foreach (SyntaxToken token in this.Tokenizer)
            {
                if (token.Kind == SyntaxKind.Whitespaces) continue;
                if (token.Kind == SyntaxKind.Comment) continue;
                if (this.PrintSyntaxError(token)) continue;

                //Console.Write ( token.Kind.ToString() + " : " );
                //Console.WriteLine ( token.Value );
            }

            return this.SyntaxErrors.Count == 0;
        }

        // -----------------------------------------------

        public bool Parse()
        {
            if (!this.Fileinfo.Exists) return false;

            this.InputText = File.ReadAllText ( this.Fileinfo.FullName );

            if (!this.CheckTokens (  )) return false;

            

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --