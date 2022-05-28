using System.Diagnostics;
using Yama.Assembler;
using Yama.Compiler;
using Yama.Index;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Lexer;
using Yama.Parser;

namespace Yama
{
    public class LanguageDefinition
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public uint StartPosition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool ParseTime
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<string> Files
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public FileInfo? OutputAssemblerFile
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo OutputFile
        {
            get;
            set;
        } = new FileInfo("out.bin");

        // -----------------------------------------------

        public IProcessorDefinition? Definition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<DirectoryInfo> Includes
        {
            get;
            set;
        } = new List<DirectoryInfo>();

        // -----------------------------------------------

        public List<string> AllFilesInUse
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public List<string> Defines
        {
            get;
            set;
        } = new List<string>();

        // -----------------------------------------------

        public bool PrintParserTree
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool PhaseTime
        {
            get;
            set;
        }

        // -----------------------------------------------

        public string StartNamespace
        {
            get;
            set;
        } = "Program";

        // -----------------------------------------------

        public Optimize OptimizeLevel
        {
            get;
            set;
        } = Optimize.SSA;

        // -----------------------------------------------

        public List<ICommand>? Sequence
        {
            get;
            set;
        }

        // -----------------------------------------------

        public FileInfo? IROutputFile
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<DirectoryInfo> Extensions
        {
            get;
            set;
        } = new List<DirectoryInfo>();

        // -----------------------------------------------

        public OutputController Output
        {
            get;
        } = new OutputController();

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public LanguageDefinition()
        {

        }

        // -----------------------------------------------

        public LanguageDefinition(OutputController outputController)
        {
            this.Output = outputController;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        #region ParserDefinition

        // -----------------------------------------------

        private ParserLayer NamespaceLayer(ParserLayer inNamespaceLayer)
        {
            ParserLayer layer = new ParserLayer("namespace");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new NamespaceKey ( inNamespaceLayer ) );

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer KlassenLayer(ParserLayer inclassLayer, ParserLayer inenumLayer)
        {
            ParserLayer layer = new ParserLayer("class");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new KlassenDeklaration ( inclassLayer ));
            layer.ParserMembers.Add(new EnumDeklaration ( inenumLayer ));
            layer.ParserMembers.Add(new UsingKey (  ));
            layer.ParserMembers.Add(new NormalExpression (  ) );
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer MethodenVektorDeklarationsHeader()
        {
            ParserLayer layer = new ParserLayer("methodevektorDeklarationsHeader");

            layer.ParserMembers.Add(new VariabelDeklaration(0, true));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InPropertyLayer(ParserLayer execlayer)
        {
            ParserLayer layer = new ParserLayer("inproperty");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new GetKey(execlayer));
            layer.ParserMembers.Add(new SetKey(execlayer));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InVektorLayer(ParserLayer execlayer)
        {
            ParserLayer layer = new ParserLayer("invektor");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new GetKey(execlayer));
            layer.ParserMembers.Add(new SetKey(execlayer));
            layer.ParserMembers.Add(new VariabelDeklaration(11));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InKlassenLayer(ParserLayer execlayer, ParserLayer inpropertyLayer, ParserLayer invektorlayer, ParserLayer methodeVektorDeklarationsHeader)
        {
            ParserLayer layer = new ParserLayer("inclass");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new MethodeDeclarationNode ( execlayer, methodeVektorDeklarationsHeader ));
            layer.ParserMembers.Add(new VektorDeclaration(invektorlayer, methodeVektorDeklarationsHeader));
            layer.ParserMembers.Add(new PropertyDeklaration ( inpropertyLayer ));
            layer.ParserMembers.Add(new PropertyGetSetDeklaration ( invektorlayer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InEnumLayer()
        {
            ParserLayer layer = new ParserLayer("inenum");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new EnumKeyValue());

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InContainerStatement(ParserLayer statementLayer, ParserLayer identifierStatementLayer, ParserLayer expressionLayer)
        {
            ParserLayer layer = new ParserLayer("incontainerStatement");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));
            layer.ParserMembers.Add ( new IfKey ( expressionLayer, layer ) );
            layer.ParserMembers.Add ( new ElseKey (  ) );
            layer.ParserMembers.Add ( new WhileKey (expressionLayer  ) );
            layer.ParserMembers.Add ( new ForKey ( expressionLayer ) );
            layer.ParserMembers.Add ( new ContinueKey (  ) );
            layer.ParserMembers.Add ( new BreakKey (  ) );
            layer.ParserMembers.Add ( new ReturnKey ( expressionLayer ) );
            layer.ParserMembers.Add ( new NormalStatementNode(identifierStatementLayer, statementLayer));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer StatementLayer(ParserLayer expressionLayer)
        {
            ParserLayer layer = new ParserLayer("statementLayer");

            layer.ParserMembers.Add(new AssigmentNode(expressionLayer));
            layer.ParserMembers.Add ( new MethodeCallNode ( IdentifierKind.OpenBracket, IdentifierKind.CloseBracket, 12, expressionLayer ) );
            layer.ParserMembers.Add ( new VektorCall ( IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket, 12, expressionLayer ) );

            return layer;

        }

        // -----------------------------------------------

        private ParserLayer IdentifierStatementLayer()
        {
            ParserLayer layer = new ParserLayer("IdentifierStatementLayer");

            layer.ParserMembers.Add(new PointIdentifier());
            layer.ParserMembers.Add(new VariabelDeklaration(0));
            layer.ParserMembers.Add(new ReferenceCall());

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer ExecutionLayer()
        {
            ParserLayer layer = new ParserLayer("execute");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));
            //layer.ParserMembers.Add ( new IfKey (  ) );
            layer.ParserMembers.Add ( new ElseKey (  ) );
            //layer.ParserMembers.Add ( new WhileKey (  ) );
            layer.ParserMembers.Add ( new ForKey ( layer ) );
            //layer.ParserMembers.Add ( new NewKey (  ) );
            layer.ParserMembers.Add ( new NullKey (  ) );
            layer.ParserMembers.Add ( new ContinueKey (  ) );
            layer.ParserMembers.Add ( new BreakKey (  ) );
            layer.ParserMembers.Add ( new TypePatternMatching ( 10 ) );
            layer.ParserMembers.Add ( new ExplicitlyConvert ( 10 ) );
            layer.ParserMembers.Add ( new MethodeCallNode ( IdentifierKind.OpenBracket, IdentifierKind.CloseBracket, 12, layer ) );
            //layer.ParserMembers.Add ( new VektorCall ( IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket, 12 ) );
            layer.ParserMembers.Add ( new ContainerExpression ( 11 ) );
            layer.ParserMembers.Add ( new NormalExpression (  ) );
            layer.ParserMembers.Add ( new EnumartionExpression (  ) );
            //layer.ParserMembers.Add ( new ReturnKey (  ) );
            layer.ParserMembers.Add ( new TrueFalseKey ( 1 ) );
            layer.ParserMembers.Add ( new VariabelDeklaration ( 11 ) );
            layer.ParserMembers.Add ( new ReferenceCall ( 1 ) );
            layer.ParserMembers.Add ( new Number ( 1 ) );
            layer.ParserMembers.Add ( new TextParser ( 1 ) );
            layer.ParserMembers.Add ( new OperatorPoint ( 11 ) );
            //layer.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "-", "~", "!" }, 11, new List<IdentifierKind> { IdentifierKind.NumberToken, IdentifierKind.Word, IdentifierKind.OpenBracket }, new List<IdentifierKind> { IdentifierKind.OpenBracket } ) );
            //layer.ParserMembers.Add ( new Operator1ChildLeft ( new List<string> { "--", "++", "!", "~" }, 11, new List<IdentifierKind> { IdentifierKind.Word, IdentifierKind.Unknown } ) );
            /*layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 2 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( IdentifierKind.LessThan, 3 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( IdentifierKind.GreaterThan, 3 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<=", ">=", "<", ">" }, 3 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "|" }, 4 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "^" }, 5 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&" }, 6 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "<<", ">>" }, 7 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "+", "-" }, 8 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "*", "/", "%" }, 9 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "√", "^^" }, 10 ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "=" }, 1 ) );*/
            //layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "?" }, IdentifierKind.DoublePoint, 2 ) );
            //layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "∑" }, IdentifierKind.DoublePoint, 2 ) );

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer ExpressionIdenLayer()
        {
            ParserLayer layer = new ParserLayer("ExpressionIdenLayer");

            layer.ParserMembers.Add ( new TextParser ( 1 ) );
            layer.ParserMembers.Add ( new NullKey (  ) );
            layer.ParserMembers.Add ( new Number ( 1 ) );
            layer.ParserMembers.Add ( new TrueFalseKey ( 1 ) );
            layer.ParserMembers.Add(new PointIdentifier());
            layer.ParserMembers.Add(new ReferenceCall());

            return layer;
        }


        private ParserLayer ExpressionLayer(ParserLayer expressionCallLayer, ParserLayer expressionIdenLayer, ParserLayer operationLayer, ParserLayer expressionLayer)
        {

            expressionLayer.ParserMembers.Add ( new NewKey ( expressionLayer ) );
            expressionLayer.ParserMembers.Add(new ConditionalCompilationNode (  ));
            expressionLayer.ParserMembers.Add(new ExpressionNode(expressionIdenLayer, expressionCallLayer, operationLayer));

            return expressionLayer;
        }

        // -----------------------------------------------

        private ParserLayer GetGenericLayer()
        {
            ParserLayer layer = new ParserLayer("generic");

            layer.ParserMembers.Add(new GenericCall());
            layer.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "-", "~", "!" }, 11, new List<IdentifierKind> { IdentifierKind.NumberToken, IdentifierKind.Word, IdentifierKind.OpenBracket }, new List<IdentifierKind> { IdentifierKind.OpenBracket } ) );

            return layer;
        }

        private ParserLayer ExpressionCallLayer(ParserLayer expressionLayer)
        {
            ParserLayer layer = new ParserLayer("expressionCall");

            layer.ParserMembers.Add ( new ExplicitlyConvert ( 10 ) );
            layer.ParserMembers.Add ( new TypePatternMatching ( 10 ) );
            layer.ParserMembers.Add ( new MethodeCallNode ( IdentifierKind.OpenBracket, IdentifierKind.CloseBracket, 12, expressionLayer ) );
            layer.ParserMembers.Add ( new VektorCall ( IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket, 12, expressionLayer ) );

            return layer;
        }

        private ParserLayer OperationLayer(ParserLayer layer, ParserLayer expressionLayer)
        {
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 2 , expressionLayer) );
            layer.ParserMembers.Add ( new Operator2Childs ( IdentifierKind.LessThan, 3, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( IdentifierKind.GreaterThan, 3, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "==", "!=", "<=", ">=", "<", ">" }, 3, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "|" }, 4, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "^" }, 5, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&" }, 6, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "<<", ">>" }, 7, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "+", "-" }, 8, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "*", "/", "%" }, 9, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "√", "^^" }, 10, expressionLayer ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "=" }, 1, expressionLayer ) );

            return layer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();
            ParserLayer operationLayer = new ParserLayer("operationLayer");

            ParserLayer expressionLayer = new ParserLayer("expressionLayer");

            ParserLayer methodenVektorDeklarationsHeader = this.MethodenVektorDeklarationsHeader();
            //ParserLayer executionlayer = this.ExecutionLayer();
            ParserLayer expressionIdenLayer = this.ExpressionIdenLayer();
            ParserLayer expressionCallLayer = this.ExpressionCallLayer(expressionLayer);

            this.ExpressionLayer(expressionCallLayer, expressionIdenLayer, operationLayer, expressionLayer);
            this.OperationLayer(operationLayer, expressionLayer);

            ParserLayer identifierLayer = this.IdentifierStatementLayer();
            ParserLayer statementLayer = this.StatementLayer(expressionLayer);
            ParserLayer inContainerLayer = this.InContainerStatement(statementLayer, identifierLayer, expressionLayer);
            ParserLayer inenumlayer = this.InEnumLayer();
            ParserLayer inpropertyLayer = this.InPropertyLayer(inContainerLayer);
            ParserLayer invektorlayer = this.InVektorLayer(inContainerLayer);
            ParserLayer inclassLayer = this.InKlassenLayer(inContainerLayer, inpropertyLayer, invektorlayer, methodenVektorDeklarationsHeader);
            ParserLayer classLayer = this.KlassenLayer(inclassLayer, inenumlayer);
            ParserLayer namespaceLayer = this.NamespaceLayer(classLayer);
            parserRules.Add(namespaceLayer);
            parserRules.Add(operationLayer);
            parserRules.Add(expressionLayer);
            parserRules.Add(expressionCallLayer);
            parserRules.Add(expressionIdenLayer);
            parserRules.Add(methodenVektorDeklarationsHeader);
            parserRules.Add(classLayer);
            parserRules.Add(identifierLayer);
            parserRules.Add(statementLayer);
            parserRules.Add(inContainerLayer);
            parserRules.Add(inclassLayer);
            parserRules.Add(inpropertyLayer);
            //parserRules.Add(executionlayer);
            parserRules.Add(invektorlayer);
            parserRules.Add(inenumlayer);
            parserRules.Add(this.GetGenericLayer());

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
            rules.Add ( new ConditionalCompilationToken ( new ZeichenKette ( "#tag" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new ConditionalCompilationToken ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Operator ( new ZeichenKette ( "∑" ), new ZeichenKette ( "<=" ), new ZeichenKette ( "--" ), new ZeichenKette ( "++" ), new ZeichenKette ( "<>" ), new ZeichenKette ( ">=" ), new ZeichenKette ( "<<" ), new ZeichenKette ( ">>" ), new ZeichenKette ( "<" ), new ZeichenKette ( ">" ), new ZeichenKette ( "^^" ), new ZeichenKette ( "+" ), new ZeichenKette ( "-" ), new ZeichenKette ( "*" ), new ZeichenKette ( "/"), new ZeichenKette ( "%" ), new ZeichenKette ( "&&" ), new ZeichenKette ( "||" ), new ZeichenKette ( "&" ), new ZeichenKette ( "|" ), new ZeichenKette ( "==" ), new ZeichenKette ( "=" ), new ZeichenKette ( "!=" ), new ZeichenKette ( "!" ), new ZeichenKette ( "^"), new ZeichenKette ( "~" ), new ZeichenKette ( "√" ), new ZeichenKette ( "?" ) ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "(" ), IdentifierKind.OpenBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ")" ), IdentifierKind.CloseBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), IdentifierKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), IdentifierKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), IdentifierKind.OpenSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), IdentifierKind.CloseSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "." ), IdentifierKind.Point ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "," ), IdentifierKind.Comma ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ":" ), IdentifierKind.DoublePoint ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ";" ), IdentifierKind.EndOfCommand ) );
            rules.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            rules.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
            rules.Add ( new KeyWord ( "char", IdentifierKind.Char ) );
            rules.Add ( new KeyWord ( "byte", IdentifierKind.Byte ) );
            rules.Add ( new KeyWord ( "set", IdentifierKind.Set ) );
            rules.Add ( new KeyWord ( "get", IdentifierKind.Get ) );
            rules.Add ( new KeyWord ( "for", IdentifierKind.For ) );
            rules.Add ( new KeyWord ( "while", IdentifierKind.While ) );
            rules.Add ( new KeyWord ( "true", IdentifierKind.True ) );
            rules.Add ( new KeyWord ( "null", IdentifierKind.Null ) );
            rules.Add ( new KeyWord ( "enum", IdentifierKind.Enum ) );
            rules.Add ( new KeyWord ( "operator", IdentifierKind.OperatorKey ) );
            rules.Add ( new KeyWord ( "implicit", IdentifierKind.Implicit ) );
            rules.Add ( new KeyWord ( "explicit", IdentifierKind.Explicit ) );
            rules.Add ( new KeyWord ( "continue", IdentifierKind.Continue ) );
            rules.Add ( new KeyWord ( "break", IdentifierKind.Break ) );
            rules.Add ( new KeyWord ( "false", IdentifierKind.False ) );
            rules.Add ( new KeyWord ( "void", IdentifierKind.Void ) );
            rules.Add ( new KeyWord ( "return", IdentifierKind.Return ) );
            rules.Add ( new KeyWord ( "class", IdentifierKind.Class ) );
            rules.Add ( new KeyWord ( "static", IdentifierKind.Static ) );
            rules.Add ( new KeyWord ( "primitive", IdentifierKind.Primitive ) );
            rules.Add ( new KeyWord ( "nonref", IdentifierKind.Nonref ) );
            rules.Add ( new KeyWord ( "public", IdentifierKind.Public ) );
            rules.Add ( new KeyWord ( "private", IdentifierKind.Private ) );
            rules.Add ( new KeyWord ( "simple", IdentifierKind.Simple ) );
            rules.Add ( new KeyWord ( "copy", IdentifierKind.Copy ) );
            rules.Add ( new KeyWord ( "uint", IdentifierKind.UInt32Bit ) );
            rules.Add ( new KeyWord ( "ushort", IdentifierKind.UInt16Bit ) );
            rules.Add ( new KeyWord ( "short", IdentifierKind.Int16Bit ) );
            rules.Add ( new KeyWord ( "long", IdentifierKind.Int64Bit ) );
            rules.Add ( new KeyWord ( "ulong", IdentifierKind.UInt64Bit ) );
            rules.Add ( new KeyWord ( "ref", IdentifierKind.Ref ) );
            rules.Add ( new KeyWord ( "is", IdentifierKind.Is ) );
            rules.Add ( new KeyWord ( "in", IdentifierKind.In ) );
            rules.Add ( new KeyWord ( "as", IdentifierKind.As ) );
            rules.Add ( new KeyWord ( "if", IdentifierKind.If ) );
            rules.Add ( new KeyWord ( "else", IdentifierKind.Else ) );
            rules.Add ( new KeyWord ( "foreach", IdentifierKind.Foreach ) );
            rules.Add ( new KeyWord ( "float", IdentifierKind.Float32Bit ) );
            rules.Add ( new KeyWord ( "new", IdentifierKind.New ) );
            rules.Add ( new KeyWord ( "delegate", IdentifierKind.Delegate ) );
            rules.Add ( new KeyWord ( "this", IdentifierKind.This ) );
            rules.Add ( new KeyWord ( "base", IdentifierKind.Base ) );
            rules.Add ( new KeyWord ( "sizeof", IdentifierKind.Sizeof ) );
            rules.Add ( new KeyWord ( "using", IdentifierKind.Using ) );
            rules.Add ( new KeyWord ( "namespace", IdentifierKind.Namespace ) );
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit ( false ), new Underscore (  ) } ) );

            return rules;
        }

        // -----------------------------------------------

        public Lexer.Lexer GetBasicLexer(Stream stream)
        {
            Lexer.Lexer lexer = new Lexer.Lexer(stream);

            lexer.LexerTokens.AddRange(this.GetLexerRules());

            return lexer;
        }

        // -----------------------------------------------

        #endregion LexerDefinition

        // -----------------------------------------------

        private bool Parse(List<IParseTreeNode> nodes)
        {
            List<string> files = this.GetFiles();

            List<ParserLayer> layers = this.GetParserRules();

            ParserLayer? startlayer = layers.Find(t=>t.Name == "namespace");
            if (startlayer == null) return false;

            bool isfailed = false;

            RequestParseIteration? requestParseIteration = null;

            foreach (string file in files)
            {
                if (!this.ParseIteration(file, startlayer, layers, nodes, ref requestParseIteration)) isfailed = true;
            }

            return !isfailed;
        }

        // -----------------------------------------------

        record RequestParseIteration(Parser.Parser Parser, Lexer.Lexer Lexer);

        private bool ParseIteration(string fullFileName, ParserLayer startlayer, List<ParserLayer> layers, List<IParseTreeNode> nodes, ref RequestParseIteration? requestParseIteration)
        {
            System.IO.FileInfo file = new System.IO.FileInfo ( fullFileName );
            if (!file.Exists) return false;

            Stream stream;
            try {stream = file.OpenRead();} catch {return false;}

            ParserInputData parserInputData = new ParserInputData(file.FullName, stream);

            if (requestParseIteration is null)
            {
                Lexer.Lexer lexer = this.GetBasicLexer(stream);
                requestParseIteration = new RequestParseIteration(new Parser.Parser (layers, lexer, parserInputData), lexer);
            }
            else
            {
                requestParseIteration.Lexer.Daten = stream;
                requestParseIteration.Parser.NewParse(requestParseIteration.Lexer, parserInputData);
            }

            Parser.Parser parser = requestParseIteration.Parser;

            using (MessaureTimeOutput messaureTimeOutput = new MessaureTimeOutput(new ParseFileStart(file), this.ParseTime, this.Output))
            {
                if (!parser.Parse(startlayer))
                {
                    messaureTimeOutput.IsPrintingEnabled = true;

                    this.PrintingErrors(parser);

                    if (this.PrintParserTree && parser.ParentContainer is not null) this.Output.Print(new ParserTreeOut(parser.ParentContainer));

                    return messaureTimeOutput.IsOK = false;
                }
            }

            IParseTreeNode? node = parser.ParentContainer;
            if (node is null) return false;

            if (this.PrintParserTree) this.Output.Print(new ParserTreeOut(node));

            nodes.AddRange(node.GetAllChilds);

            return true;
        }

        // -----------------------------------------------

        private bool Indezieren(ref List<IParseTreeNode> nodes, ref MethodeDeclarationNode? main)
        {
            Yama.Index.Index index = new Yama.Index.Index(nodes.Cast<IIndexNode>(), this.StartNamespace, this.AllFilesInUse);

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
            List<ICompileRoot> compileRoots = new List<ICompileRoot>();
            MethodeDeclarationNode? main = null;

            FileInfo file = this.OutputFile;
            if (file.Directory == null) return false;
            if (!file.Directory.Exists) file.Directory.Create();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            using (_ = new MessaureTimeOutput("Parsen ", this.PhaseTime, this.Output))
            if (!this.Parse(nodes)) return this.Output.Print(new BuildEnde(stopwatch, false));

            using (_ = new MessaureTimeOutput("Indezieren ", this.PhaseTime, this.Output))
            if (!this.Indezieren(ref nodes, ref main)) return this.Output.Print(new BuildEnde(stopwatch, false));

            using (_ = new MessaureTimeOutput("Compilen ", this.PhaseTime, this.Output))
            if (!this.Compilen(nodes, main, ref compileRoots)) return this.Output.Print(new BuildEnde(stopwatch, false));

            using (_ = new MessaureTimeOutput("Assemblen ", this.PhaseTime, this.Output))
            if (!this.Assemblen(compileRoots)) return this.Output.Print(new BuildEnde(stopwatch, false));

            this.Output.Print(new BuildEnde(stopwatch, true));

            //Console.WriteLine("Those blasted swamps... eating bugs... wading through filth... complete compilation");

            return true;
        }

        // -----------------------------------------------

        private bool Assemblen(List<ICompileRoot> compileRoots)
        {
            FileInfo file = this.OutputFile;
            if (file.Directory == null) return false;
            if (!file.Directory.Exists) file.Directory.Create();
            if (file.Exists) file.Delete();
            if (this.Definition == null) return false;

            string? def = this.Definition.Name;
            if (this.Defines.Contains("runtime")) def = "runtime";
            if (def == "avr") return true;
            if (def is null) return false;

            Assembler.Assembler assembler = new Assembler.Assembler(this.Output);
            Assembler.Definitionen definitionen = new Assembler.Definitionen();
            definitionen.GenerateAssembler(assembler, def);
            assembler.Position = this.StartPosition;

            Assembler.RequestAssemble request = new Assembler.RequestAssemble();
            request.Stream = file.OpenWrite();
            request.Roots = compileRoots;
            request.WithMapper = true;

            if (!assembler.Assemble(request))
            {
                request.Stream.Close();

                return false;
            }

            request.Stream.Close();

            this.Sequence = assembler.Sequence;

            return true;
        }

        // -----------------------------------------------

        private bool Compilen(List<IParseTreeNode> nodes, MethodeDeclarationNode? main, ref List<ICompileRoot> roots)
        {
            if (this.Definition == null) return false;

            Compiler.Compiler compiler = new Compiler.Compiler(this.Definition, this.Defines);
            compiler.OptimizeLevel = this.OptimizeLevel;
            compiler.OutputFile = this.OutputAssemblerFile;
            compiler.MainFunction = main;
            this.Definition.Compiler = compiler;

            this.InitCompilerIrPrinting(compiler);

            List<FileInfo> extensionsFiles = new List<FileInfo>();
            if (!this.LoadAllExtensionFiles(extensionsFiles)) return this.PrintSimpleError("failed to find extensions");

            if (!compiler.Definition.LoadExtensions(extensionsFiles)) return this.PrintCompilerErrors(compiler.Errors);

            if (compiler.Compilen(nodes.Cast<ICompileNode>()))
            {
                roots = compiler.AssemblerSequence;

                return true;
            }

            return this.PrintCompilerErrors(compiler.Errors);
        }

        // -----------------------------------------------

        private bool LoadAllExtensionFiles(List<FileInfo> extensionsFiles)
        {
            foreach (string yamaFile in this.AllFilesInUse)
            {
                FileInfo extFile = new FileInfo(Path.ChangeExtension(yamaFile, ".json"));
                if (!extFile.Exists) continue;

                extensionsFiles.Add(extFile);
            }

            foreach (DirectoryInfo extensionPath in this.Extensions)
            {
                if (!extensionPath.Exists) return this.PrintSimpleError($"'{extensionPath.FullName}' extension path can not be found");

                if (!this.LoadExtensionFromDirectory(extensionPath, extensionsFiles)) return false;
            }

            return true;
        }

        // -----------------------------------------------

        private bool LoadExtensionFromDirectory(DirectoryInfo directory, List<FileInfo> extensionsFiles)
        {
            if (!directory.Exists) return this.PrintSimpleError($"can not find '{directory.FullName}' extension path");

            foreach (DirectoryInfo childDirectory in directory.GetDirectories())
            {
                this.LoadExtensionFromDirectory(childDirectory, extensionsFiles);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Extension != ".json") continue;

                extensionsFiles.Add(file);
            }

            return true;
        }

        // -----------------------------------------------

        private bool InitCompilerIrPrinting(Compiler.Compiler compiler)
        {
            if ( this.IROutputFile == null ) return true;

            if ( this.IROutputFile.Exists ) this.IROutputFile.Delete ();

            compiler.IRCodeStream = new StreamWriter ( this.IROutputFile.OpenWrite () );

            compiler.IRCodeStream.AutoFlush = true;

            return true;
        }

        // -----------------------------------------------

        private List<string> GetFiles()
        {
            List<string> result = new List<string>();

            result.AddRange(this.Files);

            foreach(string file in this.Files)
            {
                if (!File.Exists(file)) this.PrintSimpleError($"Cannot add {file} File, it is not exist");
            }

            List<DirectoryInfo> infos = new List<DirectoryInfo>();

            foreach (DirectoryInfo inc in this.Includes)
            {
                if (!inc.Exists) this.PrintSimpleError($"Cannot inlcude {inc} Directory, it is not exist");

                infos.Add ( inc );
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

        // -----------------------------------------------

        private bool GetAllFilesFromADirectory(List<string> result, List<DirectoryInfo> next, DirectoryInfo check)
        {
            if (!check.Exists) return this.PrintSimpleError($"Inlcude path {check.FullName} can not be found");

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

        private bool PrintCompilerErrors(List<CompilerError> errors)
        {
            this.Output.Print(errors.Select(t=>t.Output));

            return false;
        }

        // -----------------------------------------------

        public bool PrintSimpleError(string msg)
        {
            this.Output.Print(new SimpleErrorOut(msg));

            return false;
        }

        // -----------------------------------------------

        private bool PrintingIndexErrors(Yama.Index.Index index)
        {
            this.Output.Print(index.Errors.Select(t=>t.Output));

            return false;
        }

        // -----------------------------------------------

        private bool PrintingErrors(Parser.Parser p)
        {
            List<ParserError> removes = new();
            IdentifierToken? previous = null;

            foreach ( ParserError error in p.ParserErrors )
            {
                IdentifierToken token = error.Token;

                if (previous == token) removes.Add(error);

                previous = token;

                if (token.Kind == IdentifierKind.Unknown && error.Token.ParentNode != null) token = error.Token.ParentNode.Token;
            }

            IEnumerable<ParserError> validParserErrors = p.ParserErrors.Where(q=>!removes.Contains(q));

            IEnumerable<IOutputNode> outputNodes = validParserErrors.Select(t=>t.OutputNode);

            this.Output.Print(outputNodes);

            return false;
        }

        // -----------------------------------------------

        #endregion PrintErrors

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

    public enum Optimize
    {
        None,
        Level1,
        SSA
    }

}

// -- [EOF] --