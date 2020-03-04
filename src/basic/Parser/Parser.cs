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

        private int grosstePrio = -1;

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

        public Container ParentContainer
        {
            get;
            private set;
        }

        // -----------------------------------------------

        public int Max
        {
            get;
            set;
        }

        // -----------------------------------------------

        public IParseTreeNode ErrorNode
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> ParserMembers
        {
            get;
        } = new List<IParseTreeNode> ();

        // -----------------------------------------------

        public SyntaxToken Current
        {
            get
            {
                if (this.CleanTokens.Count <= this.Position) return null;

                SyntaxToken result = this.CleanTokens[this.Position];
                result.Position = this.Position;

                return result;
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
            this.InitParser (  );
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

        public IParseTreeNode GetRule<T>() where T : IParseTreeNode
        {
            foreach ( IParseTreeNode rule in this.ParserMembers )
            {
                if ( rule is T ) return rule;
            }

            return null;
        }

        // -----------------------------------------------

        private bool InitParser (  )
        {
            //this.ParserMembers.Add ( new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ) );
            this.ParserMembers.Add ( new IfKey (  ) );
            this.ParserMembers.Add ( new ElseKey (  ) );
            this.ParserMembers.Add ( new ReturnKey (  ) );
            this.ParserMembers.Add ( new ContainerExpression ( 10 ) );
            this.ParserMembers.Add ( new NormalExpression (  ) );
            this.ParserMembers.Add ( new Number ( 0 ) );
            this.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "--", "++", "-", "~", "!" }, 10, new List<SyntaxKind> { SyntaxKind.NumberToken, SyntaxKind.Word, SyntaxKind.OpenKlammer }, new List<SyntaxKind> { SyntaxKind.OpenKlammer } ) );
            this.ParserMembers.Add ( new Operator1ChildLeft ( new List<string> { "--", "++" }, 10, new List<SyntaxKind> { SyntaxKind.Word, SyntaxKind.Unknown } ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "|" }, 1 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "^" }, 2 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&" }, 3 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 4 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<", ">", "<=", ">=" }, 5 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "<<", ">>" }, 6 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "+", "-" }, 7 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "*", "/", "%" }, 8 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "√", "^^" }, 9 ) );
            this.ParserMembers.Add ( new Operator2Childs ( new List<string> { "=", "+=", "-=" }, 0 ) );
            this.ParserMembers.Add ( new Operator3Childs ( new List<string> { "?" }, SyntaxKind.DoublePoint, 1 ) );
            this.ParserMembers.Add ( new Operator3Childs ( new List<string> { "∑" }, SyntaxKind.DoublePoint, 1 ) );
            this.ErrorNode = new ParserError (  );

            return true;
        }

        // -----------------------------------------------

        private bool InitLexer (  )
        {
            this.Tokenizer = new Lexer (  );
            this.SyntaxErrors = new List<SyntaxToken> (  );


            Escaper escape = new Escaper ( new ZeichenKette ( "\\" ), new List<Replacer>
            {
                new Replacer ( new ZeichenKette ( "\\" ), "\\" ),
                new Replacer ( new ZeichenKette ( "\0" ), "\0" ),
                new Replacer ( new ZeichenKette ( "n" ), "\n" ),
                new Replacer ( new ZeichenKette ( "r" ), "\r" ),
                new Replacer ( new ZeichenKette ( "t" ), "\t" ),
                new Replacer ( new ZeichenKette ( "\"" ), "\"" ),
                new Replacer ( new ZeichenKette ( "\'" ), "\'" ),
            } );
            this.Tokenizer.LexerTokens.Add ( new Comment ( new ZeichenKette ( "/*" ), new ZeichenKette ( "*/" ) ) );
            this.Tokenizer.LexerTokens.Add ( new Comment ( new ZeichenKette ( "//" ), new ZeichenKette ( "\n" ) ) );
            this.Tokenizer.LexerTokens.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#region asm" ), new ZeichenKette ( "#endregion asm" ) ) );
            this.Tokenizer.LexerTokens.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            this.Tokenizer.LexerTokens.Add ( new Operator ( '+', '-', '*', '/', '%', '&', '|', '=', '<', '>', '!', '^', '~', '√', '?', '∑' ) );
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
            this.Tokenizer.LexerTokens.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            this.Tokenizer.LexerTokens.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
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

            Console.Error.WriteLine ( "{4}({0},{1}): Syntax error - {3} \"{2}\"", token.Line, token.Column, token.Text, msg, this.Fileinfo.FullName );

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

        public List<IParseTreeNode> ParseCleanTokens ( int von, int bis )
        {
            if (this.CleanTokens.Count == 0) return null;
            if ( von == bis ) return new List<IParseTreeNode>();
            if ( von >= bis ) return null;
            if (bis > this.Max) return null;

            int currentpos = this.Position;
            this.Position = von;
            int tempmax = this.Max;
            this.Max = bis;

            List<IParseTreeNode> possibleParents = new List<IParseTreeNode>();
            List<IParseTreeNode> nodeParents = new List<IParseTreeNode>();

            bool isok = true;
            bool vorgangOhneNeueNodes = true;
            while ( isok )
            {
                IParseTreeNode node = this.ParsePrimary ( this.Max );

                if ( node != null ) vorgangOhneNeueNodes = false;

                isok = node != null;

                possibleParents.Add ( node );

                this.NextToken (  );

                if ( isok ) continue;
                if ( vorgangOhneNeueNodes ) continue;

                this.Position = von;
                isok = true;
                vorgangOhneNeueNodes = true;
            }

            foreach ( IParseTreeNode node in possibleParents )
            {
                if ( node == null ) continue;
                if ( node.Token.ParentNode != null ) continue;

                nodeParents.Add ( node );
            }

            this.Position = currentpos;
            this.Max = tempmax;

            return nodeParents;
        }

        private IParseTreeNode ParsePrimary ( int max )
        {
            int pos = this.Position;

            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    IParseTreeNode node = this.ParseSteuerTokens ( this.Current );
                    if ( node != null ) return node;
                }

                this.NextToken (  );
            }

            this.Position = pos;

            for ( int i = this.GetGrosstePrio (  ); i > -1; i-- )
            {
                pos = this.Position;

                IParseTreeNode node = this.ParsePrimaryPrioSystem ( max, i );

                this.Position = pos;

                if ( node != null ) return node;
            }

            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    return this.SyntaxToken ( this.Current );
                }

                this.NextToken (  );
            }

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode ParsePrimaryPrioSystem ( int max, int prio )
        {
            while ( this.Position < max )
            {
                if ( this.Current.Node == null )
                {
                    IParseTreeNode node = this.ParsePrioSystem ( this.Current, prio );
                    if ( node != null ) return node;
                }

                this.NextToken (  );
            }

            return null;
        }

        // -----------------------------------------------

        public bool NextToken (  )
        {
            this.Position++;
            return true;
        }

        // -----------------------------------------------

        private IParseTreeNode GetNodeFromToken ( SyntaxToken token )
        {
            if ( token.ParentNode != null ) return this.GetNodeFromToken ( token.ParentNode.Token );

            return token.Node;
        }

        // -----------------------------------------------

        public IParseTreeNode ParseCleanToken ( SyntaxToken token )
        {
            if ( token.Node != null ) return this.GetNodeFromToken ( token );

            IParseTreeNode result = this.ParsePrioSystem ( token, this.GetGrosstePrio (  ), true );

            if ( result != null ) return result;

            result = this.ParseSteuerTokens ( token );

            if ( result != null ) return result;

            return this.SyntaxToken ( token );
        }

        // -----------------------------------------------

        private IParseTreeNode SyntaxToken ( SyntaxToken token )
        {
            token.Kind = SyntaxKind.Unknown;

            this.PrintSyntaxError ( token, "Parser fehler" );

            return this.ErrorNode.Parse ( this, token );
        }

        // -----------------------------------------------

        private IParseTreeNode ParseSteuerTokens ( SyntaxToken token )
        {
            foreach ( IParseTreeNode member in this.ParserMembers )
            {
                if ( member is IPriority ) continue;

                IParseTreeNode result = member.Parse ( this, token );

                if ( result != null ) return result;
            }

            return null;
        }

        // -----------------------------------------------

        private int GetGrosstePrio()
        {
            if ( this.grosstePrio != -1 ) return this.grosstePrio;

            foreach ( IParseTreeNode member in this.ParserMembers )
            {
                if ( !(member is IPriority t) ) continue;
                if ( t.Prio < this.grosstePrio ) continue;

                this.grosstePrio = t.Prio;
            }

            return this.grosstePrio;
        }

        // -----------------------------------------------

        private IParseTreeNode ParsePrioSystem ( SyntaxToken token, int prio, bool isrekursiv = false )
        {
            if ( prio < 0 ) return null;

            foreach ( IParseTreeNode member in this.ParserMembers )
            {
                if ( !(member is IPriority t) ) continue;
                if ( t.Prio != prio ) continue;

                IParseTreeNode result = member.Parse ( this, token );

                if ( result != null ) return result;
            }

            if (isrekursiv) return this.ParsePrioSystem ( token, prio - 1, isrekursiv );

            return null;
        }

        // -----------------------------------------------

        private IParseTreeNode ParseOneMember ( IParseTreeNode member, SyntaxToken token )
        {
            int pos = this.Position;

            IParseTreeNode result = member.Parse ( this, token );

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

            this.Max = this.CleanTokens.Count;
            List<IParseTreeNode> parentNodes = this.ParseCleanTokens ( 0, this.CleanTokens.Count );
            if ( parentNodes == null ) return false;

            this.ParentContainer = new Container (  );
            this.ParentContainer.Statements = parentNodes;
            this.ParentContainer.Token = new SyntaxToken ( SyntaxKind.BeginContainer, 0, 0, 0, "File", this.Fileinfo.FullName );

            foreach ( IParseTreeNode node in parentNodes )
            {
                node.Token.ParentNode = this.ParentContainer;
            }

            this.PrintPretty ( this.ParentContainer );

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken FindEndToken ( SyntaxToken begin, SyntaxKind endKind, SyntaxKind escapeKind )
        {
            SyntaxToken kind = begin;

            for ( int i = 1; kind.Kind != endKind; i++ )
            {
                kind = this.Peek ( begin, i );

                if ( kind == null ) return null;

                if ( kind.Kind != escapeKind ) continue;

                IParseTreeNode nodeCon = this.ParseCleanToken ( kind );

                if ( nodeCon == null ) return null;

                if ( !(nodeCon is ContainerExpression c) ) return null;

                i = c.Ende.Position;
            }

            return kind;
        }

        // -----------------------------------------------

        public bool Repleace ( SyntaxToken token, int pos )
        {
            this.CleanTokens[pos] = token;

            return true;
        }

        // -----------------------------------------------

        private bool PrintPretty ( IParseTreeNode node, string lebchilds = "" )
        {
            if (node == null) return true;

            Console.Write ( node.Token.Value );
            Console.WriteLine (  );

            List<IParseTreeNode> childs = node.GetAllChilds;

            if ( childs == null ) return true;

            string neuchild = lebchilds + "│   ";
            int counter = 0;
            string normalChildPrint = "├── ";
            foreach (IParseTreeNode child in childs)
            {
                if (child == null) continue;

                if (counter >= childs.Count - 1)
                {
                    normalChildPrint = "└── ";
                    neuchild = lebchilds + "    ";
                }

                Console.Write ( lebchilds );
                Console.Write ( normalChildPrint );

                this.PrintPretty ( child, neuchild );

                counter++;
            }

            return true;
        }

        // -----------------------------------------------

        public SyntaxToken FindAToken ( SyntaxToken von, SyntaxKind zufinden )
        {
            SyntaxToken kind = von;

            for ( int i = 1; kind.Kind != zufinden; i++ )
            {
                kind = this.Peek ( von, i );

                if ( kind == null ) return null;
            }

            return kind;
        }

        // -----------------------------------------------

        public SyntaxToken Peek ( int offset )
        {
            return this.Peek ( this.Current, offset );
        }

        // -----------------------------------------------

        public SyntaxToken Peek ( SyntaxToken token, int offset )
        {
            if (this.Max <= offset + token.Position) return null;
            if (0 > offset + token.Position) return null;

            SyntaxToken result = this.CleanTokens[offset + token.Position];

            result.Position = offset + token.Position;

            return result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --