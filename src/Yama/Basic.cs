using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

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
            

            return true;
        }

        
        // -----------------------------------------------

        private ParserLayer KlassenLayer(ParserLayer inclassLayer)
        {
            ParserLayer layer = new ParserLayer("class");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new KlassenDeklaration ( inclassLayer ));
            layer.ParserMembers.Add(new BedingtesCompilierenParser (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InKlassenLayer(ParserLayer execlayer)
        {
            ParserLayer layer = new ParserLayer("inclass");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new FunktionsDeklaration ( execlayer ));
            layer.ParserMembers.Add ( new EnumartionExpression (  ) );
            layer.ParserMembers.Add ( new VariabelDeklaration ( 11 ) );
            layer.ParserMembers.Add(new BedingtesCompilierenParser (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer ExecutionLayer()
        {
            ParserLayer layer = new ParserLayer("execute");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new BedingtesCompilierenParser (  ));
            layer.ParserMembers.Add ( new IfKey (  ) );
            layer.ParserMembers.Add ( new ElseKey (  ) );
            layer.ParserMembers.Add ( new WhileKey (  ) );
            layer.ParserMembers.Add ( new ForKey (  ) );
            layer.ParserMembers.Add ( new NewKey (  ) );
            layer.ParserMembers.Add ( new ExplicitConverting ( 10 ) );
            layer.ParserMembers.Add ( new FunktionsCall ( SyntaxKind.OpenKlammer, SyntaxKind.CloseKlammer, 12 ) );
            layer.ParserMembers.Add ( new ContainerExpression ( 11 ) );
            layer.ParserMembers.Add ( new NormalExpression (  ) );
            layer.ParserMembers.Add ( new EnumartionExpression (  ) );
            layer.ParserMembers.Add ( new ReturnKey (  ) );
            layer.ParserMembers.Add ( new TrueFalseKey ( 1 ) );
            layer.ParserMembers.Add ( new VariabelDeklaration ( 11 ) );
            layer.ParserMembers.Add ( new ReferenceCall ( 1 ) );
            layer.ParserMembers.Add ( new VektorCall ( SyntaxKind.EckigeKlammerAuf, SyntaxKind.EckigeKlammerZu, 1 ) );
            layer.ParserMembers.Add ( new GenericCall ( SyntaxKind.KleinerAls, SyntaxKind.GroesserAls, 1 ) );
            layer.ParserMembers.Add ( new Number ( 1 ) );
            layer.ParserMembers.Add ( new TextParser ( 1 ) );
            layer.ParserMembers.Add ( new OperatorPoint ( 11 ) );
            layer.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "--", "++", "-", "~", "!" }, 11, new List<SyntaxKind> { SyntaxKind.NumberToken, SyntaxKind.Word, SyntaxKind.OpenKlammer }, new List<SyntaxKind> { SyntaxKind.OpenKlammer } ) );
            layer.ParserMembers.Add ( new Operator1ChildLeft ( new List<string> { "--", "++" }, 11, new List<SyntaxKind> { SyntaxKind.Word, SyntaxKind.Unknown } ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "|" }, 2 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "^" }, 3 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&" }, 4 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 5 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( SyntaxKind.KleinerAls, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( SyntaxKind.GroesserAls, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<=", ">=" }, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "<<", ">>" }, 7 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "+", "-" }, 8 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "*", "/", "%" }, 9 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "√", "^^" }, 10 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "=", "+=", "-=" }, 1 ) );
            layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "?" }, SyntaxKind.DoublePoint, 2 ) );
            layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "∑" }, SyntaxKind.DoublePoint, 2 ) );

            return layer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();

            ParserLayer executionlayer = this.ExecutionLayer();
            ParserLayer inclassLayer = this.InKlassenLayer(executionlayer);
            ParserLayer classLayer = this.KlassenLayer(inclassLayer);
            parserRules.Add(classLayer);
            parserRules.Add(inclassLayer);
            parserRules.Add(executionlayer);

            return parserRules;
        }

        public List<ILexerToken> GetLexerRules()
        {
            List<ILexerToken> rules = new List<ILexerToken>();

            Escaper escape = new Escaper ( new ZeichenKette ( "\\" ), new List<Replacer>
            {
                new Replacer ( new ZeichenKette ( "\\" ), "\\" ),
                new Replacer ( new ZeichenKette ( "0" ), "\0" ),
                new Replacer ( new ZeichenKette ( "n" ), "\n" ),
                new Replacer ( new ZeichenKette ( "r" ), "\r" ),
                new Replacer ( new ZeichenKette ( "t" ), "\t" ),
                new Replacer ( new ZeichenKette ( "\"" ), "\"" ),
                new Replacer ( new ZeichenKette ( "\'" ), "\'" ),
            } );
            rules.Add ( new Comment ( new ZeichenKette ( "/*" ), new ZeichenKette ( "*/" ) ) );
            rules.Add ( new Comment ( new ZeichenKette ( "//" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#region asm" ), new ZeichenKette ( "#endregion asm" ) ) );
            rules.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Operator ( new ZeichenKette ( "∑" ), new ZeichenKette ( "<=" ), new ZeichenKette ( "++" ), new ZeichenKette ( "<>" ), new ZeichenKette ( ">=" ), new ZeichenKette ( "^^" ), new ZeichenKette ( "+" ), new ZeichenKette ( "-" ), new ZeichenKette ( "*" ), new ZeichenKette ( "/"), new ZeichenKette ( "%" ), new ZeichenKette ( "&" ), new ZeichenKette ( "|" ), new ZeichenKette ( "==" ), new ZeichenKette ( "=" ), new ZeichenKette ( "!=" ), new ZeichenKette ( "!" ), new ZeichenKette ( "^"), new ZeichenKette ( "~" ), new ZeichenKette ( "√" ), new ZeichenKette ( "?" ) ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "(" ), SyntaxKind.OpenKlammer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "<" ), SyntaxKind.KleinerAls ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ">" ), SyntaxKind.GroesserAls ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ")" ), SyntaxKind.CloseKlammer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), SyntaxKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), SyntaxKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), SyntaxKind.EckigeKlammerAuf ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), SyntaxKind.EckigeKlammerZu ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "." ), SyntaxKind.Point ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "," ), SyntaxKind.Comma ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ":" ), SyntaxKind.DoublePoint ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ";" ), SyntaxKind.EndOfCommand ) );
            rules.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            rules.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
            rules.Add ( new KeyWord ( "int", SyntaxKind.Int32Bit ) );
            rules.Add ( new KeyWord ( "char", SyntaxKind.Char ) );
            rules.Add ( new KeyWord ( "byte", SyntaxKind.Byte ) );
            rules.Add ( new KeyWord ( "set", SyntaxKind.Set ) );
            rules.Add ( new KeyWord ( "get", SyntaxKind.Get ) );
            rules.Add ( new KeyWord ( "for", SyntaxKind.For ) );
            rules.Add ( new KeyWord ( "while", SyntaxKind.While ) );
            rules.Add ( new KeyWord ( "bool", SyntaxKind.Boolean ) );
            rules.Add ( new KeyWord ( "true", SyntaxKind.True ) );
            rules.Add ( new KeyWord ( "null", SyntaxKind.Null ) );
            rules.Add ( new KeyWord ( "enum", SyntaxKind.Enum ) );
            rules.Add ( new KeyWord ( "operator", SyntaxKind.OperatorKey ) );
            rules.Add ( new KeyWord ( "implicit", SyntaxKind.Implicit ) );
            rules.Add ( new KeyWord ( "explicit", SyntaxKind.Explicit ) );
            rules.Add ( new KeyWord ( "continue", SyntaxKind.Continue ) );
            rules.Add ( new KeyWord ( "break", SyntaxKind.Break ) );
            rules.Add ( new KeyWord ( "false", SyntaxKind.False ) );
            rules.Add ( new KeyWord ( "void", SyntaxKind.Void ) );
            rules.Add ( new KeyWord ( "return", SyntaxKind.Return ) );
            rules.Add ( new KeyWord ( "class", SyntaxKind.Class ) );
            rules.Add ( new KeyWord ( "static", SyntaxKind.Static ) );
            rules.Add ( new KeyWord ( "nonref", SyntaxKind.Nonref ) );
            rules.Add ( new KeyWord ( "public", SyntaxKind.Public ) );
            rules.Add ( new KeyWord ( "private", SyntaxKind.Private ) );
            rules.Add ( new KeyWord ( "uint", SyntaxKind.UInt32Bit ) );
            rules.Add ( new KeyWord ( "ushort", SyntaxKind.UInt16Bit ) );
            rules.Add ( new KeyWord ( "short", SyntaxKind.Int16Bit ) );
            rules.Add ( new KeyWord ( "long", SyntaxKind.Int64Bit ) );
            rules.Add ( new KeyWord ( "ulong", SyntaxKind.UInt64Bit ) );
            rules.Add ( new KeyWord ( "ref", SyntaxKind.Ref ) );
            rules.Add ( new KeyWord ( "is", SyntaxKind.Is ) );
            rules.Add ( new KeyWord ( "in", SyntaxKind.In ) );
            rules.Add ( new KeyWord ( "as", SyntaxKind.As ) );
            rules.Add ( new KeyWord ( "if", SyntaxKind.If ) );
            rules.Add ( new KeyWord ( "else", SyntaxKind.Else ) );
            rules.Add ( new KeyWord ( "foreach", SyntaxKind.Foreach ) );
            rules.Add ( new KeyWord ( "float", SyntaxKind.Float32Bit ) );
            rules.Add ( new KeyWord ( "new", SyntaxKind.New ) );
            rules.Add ( new KeyWord ( "delegate", SyntaxKind.Delegate ) );
            rules.Add ( new KeyWord ( "using", SyntaxKind.Using ) );
            rules.Add ( new KeyWord ( "this", SyntaxKind.This ) );
            rules.Add ( new KeyWord ( "base", SyntaxKind.Base ) );
            rules.Add ( new KeyWord ( "sizeof", SyntaxKind.Sizeof ) );
            rules.Add ( new KeyWord ( "namespace", SyntaxKind.Namespace ) );
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );

            return rules;
        }

        public Lexer GetBasicLexer()
        {
            Lexer lexer = new Lexer();

            lexer.LexerTokens = this.GetLexerRules();

            return lexer;
        }

        public bool Compile()
        {
            System.IO.FileInfo file = new System.IO.FileInfo ( this.File );

            Parser p = new Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
            ParserLayer startlayer = p.ParserLayers.FirstOrDefault(t=>t.Name == "class");

            p.ErrorNode = new ParserError (  );

            if (!p.Parse(startlayer)) return this.PrintingErrors(p);

            IParseTreeNode node = p.ParentContainer;

            Yama.Index.Index index = new Yama.Index.Index();
            index.Roots = node.GetAllChilds;

            if (!index.CreateIndex()) return this.PrintingIndexErrors(index, p);

            return true;
        }

        private bool PrintingIndexErrors(Yama.Index.Index index, Parser p)
        {
            foreach ( IndexError error in index.Errors )
            {
                SyntaxToken token = error.Use.Token;

                p.PrintSyntaxError ( token, error.Msg, "Index error" );
            }

            return false;
        }

        private bool PrintingErrors(Parser p)
        {
            foreach ( IParseTreeNode error in p.ParserErrors )
            {
                SyntaxToken token = error.Token;

                if (token.Kind == SyntaxKind.Unknown) token = error.Token.ParentNode.Token;

                p.PrintSyntaxError ( token, token.Text );
            }

            return false;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --