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
            this.Tokenizer.LexerTokens.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            this.Tokenizer.LexerTokens.Add ( new Operator ( '+', '-', '*', '/', '%', '&', '|', '=', '<', '>', '!', '^', '~', '√', '?' ) );
            this.Tokenizer.LexerTokens.Add ( new Digit (  ) );
            this.Tokenizer.LexerTokens.Add ( new Whitespaces (  ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "(" ), SyntaxKind.OpenKlammer ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( ")" ), SyntaxKind.CloseKlammer ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "{" ), SyntaxKind.BeginContainer ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "}" ), SyntaxKind.CloseContainer ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "[" ), SyntaxKind.EckigeKlammerAuf ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "]" ), SyntaxKind.EckigeKlammerZu ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "." ), SyntaxKind.Point ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( "," ), SyntaxKind.Comma ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( ":" ), SyntaxKind.DoublePoint ) );
            this.Tokenizer.LexerTokens.Add ( new Punctuation ( new ZeichenKette ( ";" ), SyntaxKind.EndOfCommand ) );
            this.Tokenizer.LexerTokens.Add ( new Text (  ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "int", SyntaxKind.Int32Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "char", SyntaxKind.Char ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "byte", SyntaxKind.Byte ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "for", SyntaxKind.For ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "while", SyntaxKind.While ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "bool", SyntaxKind.Boolean ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "true", SyntaxKind.True ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "null", SyntaxKind.Null ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "enum", SyntaxKind.Enum ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "continue", SyntaxKind.Continue ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "break", SyntaxKind.Break ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "false", SyntaxKind.False ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "void", SyntaxKind.Void ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "return", SyntaxKind.Return ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "class", SyntaxKind.Class ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "static", SyntaxKind.Static ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "nonref", SyntaxKind.Nonref ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "public", SyntaxKind.Public ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "private", SyntaxKind.Private ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "uint", SyntaxKind.UInt32Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ushort", SyntaxKind.UInt16Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "short", SyntaxKind.Int16Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "long", SyntaxKind.Int64Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ulong", SyntaxKind.UInt64Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "ref", SyntaxKind.Ref ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "is", SyntaxKind.Is ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "in", SyntaxKind.In ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "as", SyntaxKind.As ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "foreach", SyntaxKind.Foreach ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "float", SyntaxKind.Float32Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "new", SyntaxKind.New ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "delegate", SyntaxKind.Delegate ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "using", SyntaxKind.Using ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "this", SyntaxKind.This ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "base", SyntaxKind.Base ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "sizeof", SyntaxKind.Sizeof ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "namespace", SyntaxKind.Namespace ) );
            this.Tokenizer.LexerTokens.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );

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