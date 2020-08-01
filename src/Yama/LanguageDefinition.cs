using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Compiler;
using Yama.Compiler.Atmega328p;
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
        }

        // -----------------------------------------------

        public string ExpressionLine
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string Framework
        {
            get;
            set;
        }

        // -----------------------------------------------

        private Lexer.Lexer Tokenizer
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        #region ParserDefinition

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
            layer.ParserMembers.Add(new FunktionsDeklaration ( execlayer ));
            layer.ParserMembers.Add(new PropertyDeklaration ( inpropertyLayer ));
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
            rules.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#region asm" ), new ZeichenKette ( "#endregion asm" ) ) );
            rules.Add ( new BedingtesCompilieren ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Operator ( new ZeichenKette ( "∑" ), new ZeichenKette ( "<=" ), new ZeichenKette ( "++" ), new ZeichenKette ( "<>" ), new ZeichenKette ( ">=" ), new ZeichenKette ( "<" ), new ZeichenKette ( ">" ), new ZeichenKette ( "^^" ), new ZeichenKette ( "+" ), new ZeichenKette ( "-" ), new ZeichenKette ( "*" ), new ZeichenKette ( "/"), new ZeichenKette ( "%" ), new ZeichenKette ( "&" ), new ZeichenKette ( "|" ), new ZeichenKette ( "==" ), new ZeichenKette ( "=" ), new ZeichenKette ( "!=" ), new ZeichenKette ( "!" ), new ZeichenKette ( "^"), new ZeichenKette ( "~" ), new ZeichenKette ( "√" ), new ZeichenKette ( "?" ) ) );
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

        #region CompilerDefinition

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallGet()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "default";
            result.Description = "Der aufruf einer ganz normalen Variabel";
            result.Keys.Add("[VAR]");
            result.AssemblyCommands.Add("ldd r24,[VAR]");
            result.AssemblyCommands.Add("ldd r25,[VAR]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallSet()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "set";
            result.Description = "Das setzen einer ganz normalen Variabel";
            result.Keys.Add("[VAR]");
            result.AssemblyCommands.Add("std [VAR],r24");
            result.AssemblyCommands.Add("std [VAR],r25");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoExecutionCall()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ExecuteCall";
            result.Mode = "default";
            result.Description = "Aufruf einer Funktion";
            result.AssemblyCommands.Add("movw r30, r24");
            result.AssemblyCommands.Add("icall");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoFunktionsDeklaration()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "FunktionsDeklaration";
            result.Mode = "default";
            result.Description = "Die Deklaration einer Funktion";
            result.Keys.Add("[NAME]");
            result.AssemblyCommands.Add("[NAME]:");
            result.AssemblyCommands.Add("push r16");
            result.AssemblyCommands.Add("push r17");
            result.AssemblyCommands.Add("push r18");
            result.AssemblyCommands.Add("push r19");
            result.AssemblyCommands.Add("push r20");
            result.AssemblyCommands.Add("push r21");
            result.AssemblyCommands.Add("push r28");
            result.AssemblyCommands.Add("push r29");
            result.AssemblyCommands.Add("in r28,__SP_L__");
            result.AssemblyCommands.Add("in r29,__SP_H__");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoFunktionsEnde()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "FunktionsEnde";
            result.Mode = "default";
            result.Description = "Das ende einer Funktion";
            result.AssemblyCommands.Add("adiw r28, 6");
            result.AssemblyCommands.Add("in __tmp_reg__,__SREG__");
            result.AssemblyCommands.Add("cli");
            result.AssemblyCommands.Add("out __SP_H__,r29");
            result.AssemblyCommands.Add("out __SREG__,__tmp_reg__");
            result.AssemblyCommands.Add("out __SP_L__,r28");
            result.AssemblyCommands.Add("pop r29");
            result.AssemblyCommands.Add("pop r28");
            result.AssemblyCommands.Add("pop r21");
            result.AssemblyCommands.Add("pop r20");
            result.AssemblyCommands.Add("pop r19");
            result.AssemblyCommands.Add("pop r18");
            result.AssemblyCommands.Add("pop r17");
            result.AssemblyCommands.Add("pop r16");
            result.AssemblyCommands.Add("ret");

            return result;
        }

        // -----------------------------------------------
        private CompileAlgo CreateAlgoMoveResult()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "MovResult";
            result.Mode = "default";
            result.Description = "Das verschieben eines Ergebnisses";
            result.Keys.Add("[REG]");
            result.AssemblyCommands.Add("movw [REG], r24");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoUsePara()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "UsePara";
            result.Mode = "default";
            result.Description = "Ein Registry Cache Entry als Parameter nutzen";
            result.Keys.Add("[REGPOP]");
            result.Keys.Add("[PARA]");
            result.AssemblyCommands.Add("movw [PARA],[REGPOP]");

            return result;
        }

        // -----------------------------------------------

        private CompileAlgo CreateAlgoReferenceCallMethode()
        {
            CompileAlgo result = new CompileAlgo();

            result.Name = "ReferenceCall";
            result.Mode = "methode";
            result.Description = "Der aufruf einer ganz normalen Methode";
            result.Keys.Add("[METHODEREFCALL]");
            result.AssemblyCommands.Add("ldi r24,[METHODEREFCALL]");
            result.AssemblyCommands.Add("ldi r25,[METHODEREFCALL]");

            return result;
        }

        // -----------------------------------------------

        private List<CompileAlgo> CompileAlgos()
        {
            List<CompileAlgo> result = new List<CompileAlgo>();

            result.Add(this.CreateAlgoReferenceCallGet());
            result.Add(this.CreateAlgoReferenceCallMethode());
            result.Add(this.CreateAlgoReferenceCallSet());
            result.Add(this.CreateAlgoMoveResult());
            result.Add(this.CreateAlgoExecutionCall());
            result.Add(this.CreateAlgoFunktionsDeklaration());
            result.Add(this.CreateAlgoFunktionsEnde());
            result.Add(this.CreateAlgoUsePara());

            return result;
        }

        // -----------------------------------------------

        #endregion CompilerDefinition

        // -----------------------------------------------

        private bool Parse(List<IParseTreeNode> nodes)
        {
            List<string> files = this.GetFiles();

            foreach (string File in files)
            {
                System.IO.FileInfo file = new System.IO.FileInfo ( File );

                Parser.Parser p = new Parser.Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
                ParserLayer startlayer = p.ParserLayers.FirstOrDefault(t=>t.Name == "class");

                p.ErrorNode = new ParserError (  );

                if (!p.Parse(startlayer)) return this.PrintingErrors(p);

                IParseTreeNode node = p.ParentContainer;

                nodes.AddRange(node.GetAllChilds);
            }

            return true;
        }

        // -----------------------------------------------

        private bool Indezieren(List<IParseTreeNode> nodes)
        {
            Yama.Index.Index index = new Yama.Index.Index();
            index.Roots = nodes;

            if (!index.CreateIndex()) return this.PrintingIndexErrors(index);

            return true;
        }

        // -----------------------------------------------

        public bool Compile()
        {
            List<IParseTreeNode> nodes = new List<IParseTreeNode>();

            if (!this.Parse(nodes)) return false;

            if (!this.Indezieren(nodes)) return false;

            if (!this.Compilen(nodes)) return false;

            return true;
        }

        // -----------------------------------------------

        private bool Compilen(List<IParseTreeNode> nodes)
        {
            Compiler.Compiler compiler = new Compiler.Compiler();
            compiler.OutputFile = new FileInfo("out.S");
            compiler.Definition = new Atmega328pDefinition();
            compiler.Algos = this.CompileAlgos();

            return compiler.Compilen(nodes);
        }

        // -----------------------------------------------

        private List<string> GetFiles()
        {
            List<string> result = new List<string>();
            result.AddRange(this.Files);

            string path = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, this.Framework);

            string[] files = Directory.GetFiles(path);

            result.AddRange(files);

            return result;
        }

        // -----------------------------------------------

        #region PrintErrors

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