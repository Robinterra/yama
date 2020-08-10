using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama
{
    public class LanguageDefinition
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<string> Files
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public string OutputFile
        {
            get;
            set;
        } = "out.S";

        // -----------------------------------------------

        public IProcessorDefinition Definition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<string> Includes
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public List<FileInfo> AllFilesInUse
        {
            get;
            set;
        } = new List<FileInfo>();

        // -----------------------------------------------

        public List<string> Defines
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public string StartNamespace
        {
            get;
            set;
        } = "Program";

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        #region ParserDefinition

        // -----------------------------------------------

        private ParserLayer NamespaceLayer(ParserLayer inNamespaceLayer)
        {
            ParserLayer layer = new ParserLayer("namespace");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new NamespaceKey ( inNamespaceLayer ) );

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer KlassenLayer(ParserLayer inclassLayer)
        {
            ParserLayer layer = new ParserLayer("class");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new KlassenDeklaration ( inclassLayer ));
            layer.ParserMembers.Add(new UsingKey (  ));
            layer.ParserMembers.Add(new NormalExpression (  ) );
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InPropertyLayer(ParserLayer execlayer)
        {
            ParserLayer layer = new ParserLayer("inproperty");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new GetKey(execlayer));
            layer.ParserMembers.Add(new SetKey(execlayer));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InKlassenLayer(ParserLayer execlayer, ParserLayer inpropertyLayer)
        {
            ParserLayer layer = new ParserLayer("inclass");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new MethodeDeclarationNode ( execlayer ));
            layer.ParserMembers.Add(new PropertyDeklaration ( inpropertyLayer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer ExecutionLayer()
        {
            ParserLayer layer = new ParserLayer("execute");

            layer.ParserMembers.Add(new Container ( SyntaxKind.BeginContainer, SyntaxKind.CloseContainer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));
            layer.ParserMembers.Add ( new IfKey (  ) );
            layer.ParserMembers.Add ( new ElseKey (  ) );
            layer.ParserMembers.Add ( new WhileKey (  ) );
            layer.ParserMembers.Add ( new ForKey (  ) );
            layer.ParserMembers.Add ( new NewKey (  ) );
            layer.ParserMembers.Add ( new NullKey (  ) );
            layer.ParserMembers.Add ( new ContinueKey (  ) );
            layer.ParserMembers.Add ( new ExplicitConverting ( 10 ) );
            layer.ParserMembers.Add ( new MethodeCallNode ( SyntaxKind.OpenBracket, SyntaxKind.CloseBracket, 12 ) );
            layer.ParserMembers.Add ( new ContainerExpression ( 11 ) );
            layer.ParserMembers.Add ( new NormalExpression (  ) );
            layer.ParserMembers.Add ( new EnumartionExpression (  ) );
            layer.ParserMembers.Add ( new ReturnKey (  ) );
            layer.ParserMembers.Add ( new TrueFalseKey ( 1 ) );
            layer.ParserMembers.Add ( new VariabelDeklaration ( 11 ) );
            layer.ParserMembers.Add ( new ReferenceCall ( 1 ) );
            layer.ParserMembers.Add ( new VektorCall ( SyntaxKind.OpenSquareBracket, SyntaxKind.CloseSquareBracket, 1 ) );
            layer.ParserMembers.Add ( new Number ( 1 ) );
            layer.ParserMembers.Add ( new TextParser ( 1 ) );
            layer.ParserMembers.Add ( new OperatorPoint ( 11 ) );
            layer.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "--", "++", "-", "~", "!" }, 11, new List<SyntaxKind> { SyntaxKind.NumberToken, SyntaxKind.Word, SyntaxKind.OpenBracket }, new List<SyntaxKind> { SyntaxKind.OpenBracket } ) );
            layer.ParserMembers.Add ( new Operator1ChildLeft ( new List<string> { "--", "++" }, 11, new List<SyntaxKind> { SyntaxKind.Word, SyntaxKind.Unknown } ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "|" }, 2 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "^" }, 3 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&" }, 4 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 5 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( SyntaxKind.LessThan, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( SyntaxKind.GreaterThan, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<=", ">=", "<", ">" }, 6 ) );
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
            ParserLayer inpropertyLayer = this.InPropertyLayer(executionlayer);
            ParserLayer inclassLayer = this.InKlassenLayer(executionlayer, inpropertyLayer);
            ParserLayer classLayer = this.KlassenLayer(inclassLayer);
            ParserLayer namespaceLayer = this.NamespaceLayer(classLayer);
            parserRules.Add(namespaceLayer);
            parserRules.Add(classLayer);
            parserRules.Add(inclassLayer);
            parserRules.Add(inpropertyLayer);
            parserRules.Add(executionlayer);

            return parserRules;
        }

        // -----------------------------------------------

        #endregion ParserDefinition

        // -----------------------------------------------

        #region LexerDefinition

        // -----------------------------------------------

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
            rules.Add ( new ConditionalCompilationToken ( new ZeichenKette ( "#defalgo" ), new ZeichenKette ( ":" ) ) );
            rules.Add ( new ConditionalCompilationToken ( new ZeichenKette ( "#region asm" ), new ZeichenKette ( "#endregion asm" ) ) );
            rules.Add ( new ConditionalCompilationToken ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Operator ( new ZeichenKette ( "∑" ), new ZeichenKette ( "<=" ), new ZeichenKette ( "++" ), new ZeichenKette ( "<>" ), new ZeichenKette ( ">=" ), new ZeichenKette ( "<" ), new ZeichenKette ( ">" ), new ZeichenKette ( "^^" ), new ZeichenKette ( "+" ), new ZeichenKette ( "-" ), new ZeichenKette ( "*" ), new ZeichenKette ( "/"), new ZeichenKette ( "%" ), new ZeichenKette ( "&" ), new ZeichenKette ( "|" ), new ZeichenKette ( "==" ), new ZeichenKette ( "=" ), new ZeichenKette ( "!=" ), new ZeichenKette ( "!" ), new ZeichenKette ( "^"), new ZeichenKette ( "~" ), new ZeichenKette ( "√" ), new ZeichenKette ( "?" ) ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "(" ), SyntaxKind.OpenBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ")" ), SyntaxKind.CloseBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), SyntaxKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), SyntaxKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), SyntaxKind.OpenSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), SyntaxKind.CloseSquareBracket ) );
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
            rules.Add ( new KeyWord ( "this", SyntaxKind.This ) );
            rules.Add ( new KeyWord ( "base", SyntaxKind.Base ) );
            rules.Add ( new KeyWord ( "sizeof", SyntaxKind.Sizeof ) );
            rules.Add ( new KeyWord ( "using", SyntaxKind.Using ) );
            rules.Add ( new KeyWord ( "namespace", SyntaxKind.Namespace ) );
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );

            return rules;
        }

        // -----------------------------------------------

        public Lexer.Lexer GetBasicLexer()
        {
            Lexer.Lexer lexer = new Lexer.Lexer();

            lexer.LexerTokens = this.GetLexerRules();

            return lexer;
        }

        // -----------------------------------------------

        #endregion LexerDefinition

        // -----------------------------------------------

        private bool Parse(List<IParseTreeNode> nodes)
        {
            List<string> files = this.GetFiles();

            foreach (string File in files)
            {
                System.IO.FileInfo file = new System.IO.FileInfo ( File );

                Parser.Parser p = new Parser.Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
                ParserLayer startlayer = p.ParserLayers.FirstOrDefault(t=>t.Name == "namespace");

                p.ErrorNode = new ParserError (  );

                if (!p.Parse(startlayer)) return this.PrintingErrors(p);

                IParseTreeNode node = p.ParentContainer;

                nodes.AddRange(node.GetAllChilds);
            }

            return true;
        }

        // -----------------------------------------------

        private bool Indezieren(ref List<IParseTreeNode> nodes, ref MethodeDeclarationNode main)
        {
            Yama.Index.Index index = new Yama.Index.Index();
            index.Roots = nodes;
            index.StartNamespace = this.StartNamespace;
            index.AllUseFiles = this.AllFilesInUse;

            if (!index.CreateIndex()) return this.PrintingIndexErrors(index);

            main = index.MainFunction;
            nodes = index.ZuCompilenNodes;

            return true;
        }

        // -----------------------------------------------

        public bool Compile()
        {
            if (this.Definition == null) return false;

            List<IParseTreeNode> nodes = new List<IParseTreeNode>();
            MethodeDeclarationNode main = null;

            if (!this.Parse(nodes)) return false;

            if (!this.Indezieren(ref nodes, ref main)) return false;

            if (!this.Compilen(nodes, main)) return false;

            return true;
        }

        // -----------------------------------------------

        private bool Compilen(List<IParseTreeNode> nodes, MethodeDeclarationNode main)
        {
            Compiler.Compiler compiler = new Compiler.Compiler();
            compiler.OutputFile = new FileInfo(this.OutputFile);
            compiler.Definition = this.Definition;
            compiler.MainFunction = main;
            compiler.Defines = this.Defines;

            if (!compiler.Definition.LoadExtensions(this.AllFilesInUse)) return false;

            if (compiler.Compilen(nodes)) return true;

            foreach (CompilerError error in compiler.Errors)
            {
                this.PrintSimpleError(error.Msg);
            }

            return false;
        }

        // -----------------------------------------------

        private List<string> GetFiles()
        {
            List<string> result = new List<string>();

            result.AddRange(this.Files);

            foreach(string file in this.Files)
            {
                if (!File.Exists(file)) this.PrintSimpleError(string.Format("Cannot add {0} File, it is not exist", file));
            }

            List<DirectoryInfo> infos = new List<DirectoryInfo>();

            foreach (string inc in this.Includes)
            {
                if (!Directory.Exists(inc)) this.PrintSimpleError(string.Format("Cannot inlcude {0} Directory, it is not exist", inc));

                infos.Add ( new DirectoryInfo ( inc ) );
            }

            result.AddRange ( this.GetFilesIterativ ( infos ) );

            return result;
        }

        // -----------------------------------------------

        private List<string> GetFilesIterativ(List<DirectoryInfo> infos)
        {
            List<DirectoryInfo> next = infos;
            List<string> result = new List<string>();

            while (infos.Count != 0)
            {
                next = new List<DirectoryInfo>();

                foreach ( DirectoryInfo info in infos )
                {
                    this.GetAllFilesFromADirectory(result, next, info);
                }

                infos.Clear();

                infos = next;
            }

            return result;
        }

        private bool GetAllFilesFromADirectory(List<string> result, List<DirectoryInfo> next, DirectoryInfo check)
        {
            if (!check.Exists) return this.PrintSimpleError(string.Format("Inlcude path {0} can not be found", check.FullName));

            foreach ( FileInfo file in check.GetFiles (  ) )
            {
                if ( file.Extension != ".yama" ) continue;

                result.Add ( file.FullName );
            }

            foreach ( DirectoryInfo dir in check.GetDirectories (  ) )
            {
                next.Add ( dir );
            }

            return true;
        }

        // -----------------------------------------------

        #region PrintErrors

        // -----------------------------------------------

        public bool PrintSimpleError(string text)
        {
            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( text );

            Console.ForegroundColor = colr;

            return false;
        }

        // -----------------------------------------------

        private bool PrintingIndexErrors(Yama.Index.Index index)
        {
            foreach ( IndexError error in index.Errors )
            {
                SyntaxToken token = error.Use.Token;

                this.PrintSyntaxError ( token, error.Msg, "Index error" );
            }

            return false;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(SyntaxToken token, string msg, string nexterrormsg = "Syntax error")
        {
            //if (token.Kind != SyntaxKind.Unknown) return false;

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( "{4}({0},{1}): {5} - {3} \"{2}\"", token.Line, token.Column, token.Text, msg, token.FileInfo.FullName, nexterrormsg );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        private bool PrintingErrors(Parser.Parser p)
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

        #endregion PrintErrors

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --