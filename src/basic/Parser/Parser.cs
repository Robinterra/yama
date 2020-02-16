using System;
using System.IO;
using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Parser
    {

        // -----------------------------------------------

        #region vars

        // -----------------------------------------------

        

        // -----------------------------------------------

        #endregion vars

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public int Position
        {
            get;
            private set;
        }

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

        private List<SyntaxToken> CleanTokens
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

        public IParseTreeToken ParentOfTree
        {
            get;
            private set;
        }

        // -----------------------------------------------

        public List<IParseTreeToken> ParserMembers
        {
            get;
        } = new List<IParseTreeToken> ();

        // -----------------------------------------------

        public SyntaxToken Current
        {
            get
            {
                if (this.CleanTokens.Count <= this.Position) return null;

                return this.CleanTokens[this.Position];
            }
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
            this.Tokenizer.LexerTokens.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#region asm" ), new ZeichenKette ( "#endregion asm" ) ) );
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
            this.Tokenizer.LexerTokens.Add ( new Zeichen (  ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "int", SyntaxKind.Int32Bit ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "char", SyntaxKind.Char ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "byte", SyntaxKind.Byte ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "set", SyntaxKind.Set ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "get", SyntaxKind.Get ) );
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
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "if", SyntaxKind.If ) );
            this.Tokenizer.LexerTokens.Add ( new KeyWord ( "else", SyntaxKind.Else ) );
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

        private bool PrintSyntaxError(SyntaxToken token, string msg)
        {
            if (token.Kind != SyntaxKind.Unknown) return false;

            this.SyntaxErrors.Add ( token );

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "({0},{1}) Syntax error - {3} \"{2}\"", token.Line, token.Column, token.Text, msg );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        private bool CheckTokens()
        {
            this.Tokenizer.Text = this.InputText;
            this.CleanTokens = new List<SyntaxToken>();

            foreach (SyntaxToken token in this.Tokenizer)
            {
                if (token.Kind == SyntaxKind.Whitespaces) continue;
                if (token.Kind == SyntaxKind.Comment) continue;
                if (this.PrintSyntaxError ( token, "unkown char" )) continue;

                this.CleanTokens.Add ( token );
                //Console.Write ( token.Kind.ToString() + " : " );
                //Console.WriteLine ( token.Value );
            }

            return this.SyntaxErrors.Count == 0;
        }

        // -----------------------------------------------

        private bool ParseCleanTokens (  )
        {
            if (this.CleanTokens.Count == 0) return false;

            this.ParentOfTree = this.ParseCleanToken ( this.Current );

            return true;
        }

        // -----------------------------------------------

        public IParseTreeToken ParseCleanToken ( SyntaxToken token )
        {
            foreach ( IParseTreeToken member in this.ParserMembers )
            {
                IParseTreeToken result = member.Parse ( this );

                if ( result != null ) return result;
            }

            token.Kind = SyntaxKind.Unknown;

            this.PrintSyntaxError ( token, "Parser fehler" );

            return null;
        }

        // -----------------------------------------------

        private IParseTreeToken ParseOneMember ( IParseTreeToken member, SyntaxToken token )
        {
            int pos = this.Position;

            IParseTreeToken result = member.Parse ( this );

            if ( result != null ) return result;

            this.Position = pos;

            return null;
        }

        // -----------------------------------------------

        public bool Parse (  )
        {
            if (!this.Fileinfo.Exists) return false;

            this.InputText = File.ReadAllText ( this.Fileinfo.FullName );

            if (!this.CheckTokens (  )) return false;

            

            return true;
        }

        // -----------------------------------------------

        public bool SetPos ( int pos )
        {
            this.Position = pos;

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken Peek ( int offset )
        {
            if (this.CleanTokens.Count <= offset + this.Position) return null;
            if (0 > offset + this.Position) return null;

            return this.CleanTokens[offset + this.Position];
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --