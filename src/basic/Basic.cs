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

        private List<IParseTreeNode> GetParserRules (  )
        {
            List<IParseTreeNode> parserRules = new List<IParseTreeNode>();

            parserRules.Add ( new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ) );
            parserRules.Add ( new IfKey (  ) );
            parserRules.Add ( new ElseKey (  ) );
            parserRules.Add ( new WhileKey (  ) );
            parserRules.Add ( new ContainerExpression ( 10 ) );
            parserRules.Add ( new NormalExpression (  ) );
            parserRules.Add ( new ReturnKey (  ) );
            parserRules.Add ( new TrueFalseKey ( 0 ) );
            parserRules.Add ( new ReferenceCall ( 0 ) );
            parserRules.Add ( new Number ( 0 ) );
            parserRules.Add ( new Operator1ChildRight ( new List<string> { "--", "++", "-", "~", "!" }, 10, new List<SyntaxKind> { SyntaxKind.NumberToken, SyntaxKind.Word, SyntaxKind.OpenKlammer }, new List<SyntaxKind> { SyntaxKind.OpenKlammer } ) );
            parserRules.Add ( new Operator1ChildLeft ( new List<string> { "--", "++" }, 10, new List<SyntaxKind> { SyntaxKind.Word, SyntaxKind.Unknown } ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "|" }, 1 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "^" }, 2 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "&" }, 3 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 4 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<", ">", "<=", ">=" }, 5 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "<<", ">>" }, 6 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "+", "-" }, 7 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "*", "/", "%" }, 8 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "√", "^^" }, 9 ) );
            parserRules.Add ( new Operator2Childs ( new List<string> { "=", "+=", "-=" }, 0 ) );
            parserRules.Add ( new Operator3Childs ( new List<string> { "?" }, SyntaxKind.DoublePoint, 1 ) );
            parserRules.Add ( new Operator3Childs ( new List<string> { "∑" }, SyntaxKind.DoublePoint, 1 ) );

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
            rules.Add ( new Operator ( new ZeichenKette ( "∑" ), new ZeichenKette ( "<=" ), new ZeichenKette ( "++" ), new ZeichenKette ( "<>" ), new ZeichenKette ( ">=" ), new ZeichenKette ( "^^" ), new ZeichenKette ( "+" ), new ZeichenKette ( "-" ), new ZeichenKette ( "*" ), new ZeichenKette ( "/"), new ZeichenKette ( "%" ), new ZeichenKette ( "&" ), new ZeichenKette ( "|" ), new ZeichenKette ( "==" ), new ZeichenKette ( "=" ), new ZeichenKette ( "<" ), new ZeichenKette ( ">" ), new ZeichenKette ( "!=" ), new ZeichenKette ( "!" ), new ZeichenKette ( "^"), new ZeichenKette ( "~" ), new ZeichenKette ( "√" ), new ZeichenKette ( "?" ) ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "(" ), SyntaxKind.OpenKlammer ) );
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

            p.ErrorNode = new ParserError (  );

            p.Parse (  );

            foreach ( IParseTreeNode error in p.ParserErrors )
            {
                SyntaxToken token = error.Token;

                if (token.Kind == SyntaxKind.Unknown) token = error.Token.ParentNode.Token;

                p.PrintSyntaxError ( token, token.Text );
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --