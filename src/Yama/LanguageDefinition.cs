using System.Diagnostics;
using Yama.Assembler;
using Yama.Compiler;
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
            layer.ParserMembers.Add(new EnumartionExpression());
            layer.ParserMembers.Add(new VariabelDeklaration(11));

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer InKlassenLayer(ParserLayer execlayer, ParserLayer inpropertyLayer, ParserLayer invektorlayer)
        {
            ParserLayer layer = new ParserLayer("inclass");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new MethodeDeclarationNode ( execlayer ));
            layer.ParserMembers.Add ( new VektorDeclaration ( invektorlayer ) );
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
            layer.ParserMembers.Add(new EnumartionExpression());
            layer.ParserMembers.Add(new EnumKeyValue());

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer ExecutionLayer()
        {
            ParserLayer layer = new ParserLayer("execute");

            layer.ParserMembers.Add(new Container ( IdentifierKind.BeginContainer, IdentifierKind.CloseContainer ));
            layer.ParserMembers.Add(new ConditionalCompilationNode (  ));
            layer.ParserMembers.Add ( new IfKey (  ) );
            layer.ParserMembers.Add ( new ElseKey (  ) );
            layer.ParserMembers.Add ( new WhileKey (  ) );
            layer.ParserMembers.Add ( new ForKey (  ) );
            layer.ParserMembers.Add ( new NewKey (  ) );
            layer.ParserMembers.Add ( new NullKey (  ) );
            layer.ParserMembers.Add ( new ContinueKey (  ) );
            layer.ParserMembers.Add ( new BreakKey (  ) );
            layer.ParserMembers.Add ( new TypePatternMatching ( 10 ) );
            layer.ParserMembers.Add ( new ExplicitlyConvert ( 10 ) );
            layer.ParserMembers.Add ( new MethodeCallNode ( IdentifierKind.OpenBracket, IdentifierKind.CloseBracket, 12 ) );
            layer.ParserMembers.Add ( new VektorCall ( IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket, 12 ) );
            layer.ParserMembers.Add ( new ContainerExpression ( 11 ) );
            layer.ParserMembers.Add ( new NormalExpression (  ) );
            layer.ParserMembers.Add ( new EnumartionExpression (  ) );
            layer.ParserMembers.Add ( new ReturnKey (  ) );
            layer.ParserMembers.Add ( new TrueFalseKey ( 1 ) );
            layer.ParserMembers.Add ( new VariabelDeklaration ( 11 ) );
            layer.ParserMembers.Add ( new ReferenceCall ( 1 ) );
            layer.ParserMembers.Add ( new Number ( 1 ) );
            layer.ParserMembers.Add ( new TextParser ( 1 ) );
            layer.ParserMembers.Add ( new OperatorPoint ( 11 ) );
            layer.ParserMembers.Add ( new Operator1ChildRight ( new List<string> { "--", "++", "-", "~", "!" }, 11, new List<IdentifierKind> { IdentifierKind.NumberToken, IdentifierKind.Word, IdentifierKind.OpenBracket }, new List<IdentifierKind> { IdentifierKind.OpenBracket } ) );
            layer.ParserMembers.Add ( new Operator1ChildLeft ( new List<string> { "--", "++", "!", "~" }, 11, new List<IdentifierKind> { IdentifierKind.Word, IdentifierKind.Unknown } ) );
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "&&", "||" }, 2 ) );
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
            layer.ParserMembers.Add ( new Operator2Childs ( new List<string> { "=" }, 1 ) );
            layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "?" }, IdentifierKind.DoublePoint, 2 ) );
            layer.ParserMembers.Add ( new Operator3Childs ( new List<string> { "∑" }, IdentifierKind.DoublePoint, 2 ) );

            return layer;
        }

        // -----------------------------------------------

        private ParserLayer GetGenericLayer()
        {
            ParserLayer layer = new ParserLayer("generic");

            layer.ParserMembers.Add(new GenericCall());

            return layer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();

            ParserLayer executionlayer = this.ExecutionLayer();
            ParserLayer inenumlayer = this.InEnumLayer();
            ParserLayer inpropertyLayer = this.InPropertyLayer(executionlayer);
            ParserLayer invektorlayer = this.InVektorLayer(executionlayer);
            ParserLayer inclassLayer = this.InKlassenLayer(executionlayer, inpropertyLayer, invektorlayer);
            ParserLayer classLayer = this.KlassenLayer(inclassLayer, inenumlayer);
            ParserLayer namespaceLayer = this.NamespaceLayer(classLayer);
            parserRules.Add(namespaceLayer);
            parserRules.Add(classLayer);
            parserRules.Add(inclassLayer);
            parserRules.Add(inpropertyLayer);
            parserRules.Add(executionlayer);
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
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ) } ) );

            return rules;
        }

        // -----------------------------------------------

        public Lexer.Lexer GetBasicLexer()
        {
            Lexer.Lexer lexer = new Lexer.Lexer();

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
            Lexer.Lexer lexer = this.GetBasicLexer();

            ParserLayer? startlayer = layers.Find(t=>t.Name == "namespace");
            if (startlayer == null) return false;

            bool isfailed = false;

            foreach (string file in files)
            {
                if (!this.ParseIteration(file, startlayer, layers, lexer, nodes)) isfailed = true;
            }

            return !isfailed;
        }

        // -----------------------------------------------

        public bool ParseIteration(string fullFileName, ParserLayer startlayer, List<ParserLayer> layers, Lexer.Lexer lexer, List<IParseTreeNode> nodes)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            System.IO.FileInfo file = new System.IO.FileInfo ( fullFileName );
            if (!file.Exists) return false;

            Stream stream;
            try {stream = file.OpenRead();} catch {return false;}

            Parser.Parser p = new Parser.Parser (layers, lexer, new ParserInputData(file.FullName, stream));

            if (this.ParseTime) this.Output.Print(new ParseFileStart(file));

            stopwatch.Start();

            if (!p.Parse(startlayer))
            {
                stopwatch.Stop();

                return this.PrintingErrors(p, file, stopwatch);
            }

            stopwatch.Stop();
            if (this.ParseTime) this.Output.Print(new OutputEnde(stopwatch, true));

            IParseTreeNode? node = p.ParentContainer;
            if (node is null) return false;

            if (this.PrintParserTree) this.Output.Print(new ParserTreeOut(node));

            nodes.AddRange(node.GetAllChilds);

            return true;
        }

        // -----------------------------------------------

        private bool Indezieren(ref List<IParseTreeNode> nodes, ref MethodeDeclarationNode? main)
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
            List<ICompileRoot> compileRoots = new List<ICompileRoot>();
            MethodeDeclarationNode? main = null;

            FileInfo file = this.OutputFile;
            if (file.Directory == null) return false;
            if (!file.Directory.Exists) file.Directory.Create();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            if (!this.Parse(nodes)) return this.Output.Print(new BuildEnde(stopwatch, false));

            if (!this.Indezieren(ref nodes, ref main)) return this.Output.Print(new BuildEnde(stopwatch, false));

            if (!this.Compilen(nodes, main, ref compileRoots)) return this.Output.Print(new BuildEnde(stopwatch, false));

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

            Assembler.Assembler assembler = new Assembler.Assembler();
            Assembler.Definitionen definitionen = new Assembler.Definitionen();
            definitionen.GenerateAssembler(assembler, def);
            assembler.Position = this.StartPosition;

            Assembler.RequestAssemble request = new Assembler.RequestAssemble();
            request.Stream = file.OpenWrite();
            request.Roots = compileRoots;
            request.WithMapper = true;

            assembler.Assemble(request);

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

            if (compiler.Compilen(nodes))
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
                if (!extensionPath.Exists) return this.PrintSimpleError(string.Format("'{}' extension path can not be found", extensionPath.FullName));

                if (!this.LoadExtensionFromDirectory(extensionPath, extensionsFiles)) return false;
            }

            return true;
        }

        // -----------------------------------------------

        private bool LoadExtensionFromDirectory(DirectoryInfo directory, List<FileInfo> extensionsFiles)
        {
            if (!directory.Exists) return this.PrintSimpleError(string.Format("can not find '{0}' extension path", directory.FullName));

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

        private bool PrintCompilerErrors(List<CompilerError> errors)
        {
            foreach (CompilerError error in errors)
            {
                if (error.Use != null)
                {
                    this.PrintCompilerError(error);

                    continue;
                }

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

            foreach (DirectoryInfo inc in this.Includes)
            {
                if (!inc.Exists) this.PrintSimpleError(string.Format("Cannot inlcude {0} Directory, it is not exist", inc));

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

        private bool PrintCompilerError(CompilerError error)
        {
            if (error.Use == null) return this.PrintSimpleError(error.Msg);

            this.PrintSyntaxError(error.Use.Token, error.Msg, "Compiler error");

            return false;
        }

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
            this.Output.Print(index.Errors.Select(t=>t.Output));

            return false;
        }

        // -----------------------------------------------

        public bool PrintSyntaxError(IdentifierToken? token, string? msg, string nexterrormsg = "Syntax error")
        {
            //if (token.Kind != SyntaxKind.Unknown) return false;
            if (token == null)
            {
                Console.Error.WriteLine ( "Unkown Error {0}, {1}", msg, nexterrormsg );
                return false;
            }

            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            string filename = "unknown";
            if (token.Info != null) filename = token.Info.Origin;

            Console.Error.WriteLine ( "{4}({0},{1}): {5} - {3} \"{2}\"", token.Line, token.Column, token.Text, msg, filename, nexterrormsg );

            Console.ForegroundColor = colr;

            return true;
        }

        // -----------------------------------------------

        private bool PrintingErrors(Parser.Parser p, FileInfo file, System.Diagnostics.Stopwatch stopWatch)
        {
            this.Output.Print(new ParseFileStart(file));
            this.Output.Print(new OutputEnde(stopWatch, false));

            List<ParserError> removes = new();
            IdentifierToken? previous = null;

            foreach ( ParserError error in p.ParserErrors )
            {
                IdentifierToken token = error.Token;

                if (previous == token) removes.Add(error);

                previous = token;

                if (token.Kind == IdentifierKind.Unknown && error.Token.ParentNode != null) token = error.Token.ParentNode.Token;
            }

            this.Output.Print(p.ParserErrors.Where(q=>!removes.Contains(q)).Select(t=>t.OutputNode));

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