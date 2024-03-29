using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Yama.Compiler.Definition;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Lexer;
using Yama.Parser;
using Yama.ProjectConfig.Nodes;

namespace Yama.ProjectConfig
{
    public class ConfigDefinition
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public DefinitionManager TargetManager
        {
            get;
        }

        // -----------------------------------------------

        public Dictionary<string, Package> BinPackages
        {
            get;
        }

        // -----------------------------------------------

        public OutputController Output
        {
            get;
        }

        // -----------------------------------------------

        public bool DoPackageRefresh
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public ConfigDefinition (DefinitionManager defs, OutputController outputController)
        {
            this.TargetManager = defs;
            this.BinPackages = new Dictionary<string, Package> ();
            this.Output = outputController;
        }
        
        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public bool Build(LanguageDefinition definition, FileInfo file)
        {
            if (file == null) return false;
            if (!file.Exists) return false;

            return this.BuildOne(definition, file, true);
        }

        // -----------------------------------------------

        private bool BuildOne(LanguageDefinition definition, FileInfo file, bool ismain = false)
        {
            List<IDeserialize> nodes = new List<IDeserialize>();
            if (!this.Parse(nodes, file)) return false;
            if (file.Directory == null) return false;

            Project project = this.Deserialize(file.Directory, nodes);
            if (project == null) return false;

            if (!this.TranslateToDefinition(project, file, definition)) return false;

            if (!ismain) return true;

            return this.TranslateToDefinitionMain ( project, file, definition );
        }

        // -----------------------------------------------

        private bool TranslateToDefinitionMain(Project project, FileInfo file, LanguageDefinition definition)
        {
            if (this.TargetManager == null) return false;
            if (!this.ValidetProjectConfig(project, file)) return false;

            definition.Definition = this.TargetManager.GetDefinition(project.TargetPlattform);
            if (definition.Definition == null) return false;

            definition.StartPosition = (uint)project.Skip;
            definition.OSHeader = project.OsHeader;
            definition.ReflectionActive = project.ReflectionActive;

            if ( !string.IsNullOrEmpty ( project.StartNamespace ) ) definition.StartNamespace = project.StartNamespace;

            if ( project.Optimize != Optimize.SSA ) definition.OptimizeLevel = project.Optimize;

            if ( project.OutputFile != null ) definition.OutputFile = project.OutputFile;
            if ( project.IROutputFile != null ) definition.IROutputFile = project.IROutputFile;
            if ( project.AssemblerOutputFile != null ) definition.OutputAssemblerFile = project.AssemblerOutputFile;

            return true;
        }

        private bool ValidetProjectConfig(Project project, FileInfo file)
        {
            if (project.Skip < 0) return this.PrintingError("skip is lower 0, that is not allowed", file);

            return true;
        }

        // -----------------------------------------------

        private bool TranslateToDefinition(Project project, FileInfo file, LanguageDefinition definition)
        {
            definition.Defines.AddRange(project.Defines);

            definition.Includes.AddRange(project.SourcePaths);

            definition.Extensions.AddRange(project.ExtensionsPaths);

            this.AddTargets(project.LanguageDefinitions);

            if (project.Packages == null) return true;

            bool isok = true;

            foreach ( Package package in project.Packages )
            {
                if ( !this.BindPackage ( definition, package, file ) ) isok = false;
            }

            return isok;
        }

        // -----------------------------------------------

        private void AddTargets(List<FileInfo> languageDefinitions)
        {
            foreach ( FileInfo langDef in languageDefinitions )
            {
                this.TargetManager.AddDefinition ( langDef );
            }
        }

        // -----------------------------------------------

        private bool BindPackage ( LanguageDefinition definition, Package package, FileInfo file)
        {
            if ( string.IsNullOrEmpty ( package.Name ) ) return this.PrintingError("Package name can not be empty, please Define a Packagename", file);
            if ( this.BinPackages.ContainsKey ( package.Name ) ) return true;

            this.BinPackages.Add(package.Name, package);

            DirectoryInfo? packDir = package.Type switch
            {
                Package.PackageType.Git => this.BindPackage_Git(definition, package),
                Package.PackageType.Local => this.BindPackage_Local(definition, package),
                _ => null
            };

            if (packDir is null) return false;
            if (!packDir.Exists) return false;

            FileInfo projectConfig = new FileInfo ( Path.Combine ( packDir.FullName, "config.yproj" ) );
            if ( !projectConfig.Exists ) return this.PrintingError("Project config file can not be found", projectConfig);

            return this.BuildOne ( definition, projectConfig );
        }

        private DirectoryInfo? BindPackage_Local(LanguageDefinition definition, Package package)
        {
            if ( string.IsNullOrEmpty ( package.LocalPath ) ) return null;

            string packagePath = Path.Combine ( package.LocalPath );
            DirectoryInfo packDir = new DirectoryInfo ( packagePath );

            return packDir;
        }

        // -----------------------------------------------

        private DirectoryInfo? BindPackage_Git(LanguageDefinition definition, Package package)
        {
            string packagePath = Path.Combine ( Program.PackagePath.FullName, package.Name! );
            DirectoryInfo packDir = new DirectoryInfo ( packagePath );

            bool isok = this.TryCloneOrPullRepository ( packDir, package );

            if ( !isok ) return null;
            return packDir;
        }

        // -----------------------------------------------

        private bool TryCloneOrPullRepository ( DirectoryInfo packDir, Package package )
        {
            if ( !packDir.Exists ) return this.ClonePackage (packDir, package);
            if ( !package.GitAutomaticUpdate && !this.DoPackageRefresh ) return true;

            this.Output.Print(new ConfigPackageRefreshStart(package));
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            try
            {
                Repository repository = new Repository ( packDir.FullName );

                Remote origin = repository.Network.Remotes["origin"];

                IEnumerable<string> refSpecs = origin.FetchRefSpecs.Select ( x => x.Specification );

                Commands.Fetch ( repository, origin.Name, refSpecs, null, string.Empty );

                Commands.Checkout ( repository, "origin/" + package.GitBranch );
            }
            catch
            {
                stopwatch.Stop();
                this.Output.Print(new OutputEnde(stopwatch, false));

                return false;
            }

            stopwatch.Stop();
            this.Output.Print(new OutputEnde(stopwatch, true));

            return true;
        }

        // -----------------------------------------------

        private bool ClonePackage ( DirectoryInfo packDir, Package package )
        {
            this.Output.Print(new ConfigPackageRefreshStart(package, true));
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            CloneOptions cloneOptions = new CloneOptions ();

            try
            {
                Repository.Clone ( package.GitRepository, packDir.FullName, cloneOptions );

                Repository repository = new Repository ( packDir.FullName );

                Commands.Checkout ( repository, package.GitBranch );
            }
            catch
            {
                stopwatch.Stop();
                this.Output.Print(new OutputEnde(stopwatch, false));
                return false;
            }

            stopwatch.Stop();
            this.Output.Print(new OutputEnde(stopwatch, true));
            return true;
        }

        // -----------------------------------------------

        #region Lexer

        // -----------------------------------------------

        private List<ILexerToken> GetLexerRules()
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
            rules.Add ( new Comment ( new ZeichenKette ( "#" ), new ZeichenKette ( "\n" ) ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( ":" ), IdentifierKind.DoublePoint ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "{" ), IdentifierKind.BeginContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "}" ), IdentifierKind.CloseContainer ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "[" ), IdentifierKind.OpenSquareBracket ) );
            rules.Add ( new Punctuation ( new ZeichenKette ( "]" ), IdentifierKind.CloseSquareBracket ) );
            rules.Add ( new Digit (  ) );
            rules.Add ( new Whitespaces (  ) );
            rules.Add ( new Text ( new ZeichenKette ( "\"" ), new ZeichenKette ( "\"" ), escape ) );
            rules.Add ( new Text ( new ZeichenKette ( "\'" ), new ZeichenKette ( "\'" ), escape ) );
            rules.Add(new KeyWord("true", IdentifierKind.True));
            rules.Add(new KeyWord("false", IdentifierKind.False));
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ), new Punctuation( new ZeichenKette("."), IdentifierKind.Point) } ) );

            return rules;
        }

        // -----------------------------------------------

        private Lexer.Lexer GetBasicLexer(Stream stream)
        {
            Lexer.Lexer lexer = new Lexer.Lexer(stream);

            lexer.LexerTokens.AddRange(this.GetLexerRules());

            return lexer;
        }

        // -----------------------------------------------

        #endregion Lexer

        // -----------------------------------------------

        #region Parser

        // -----------------------------------------------

        private ParserLayer RootLayer(ParserLayer packagelayer)
        {
            ParserLayer parserLayer = new ParserLayer("root");

            parserLayer.ParserMembers.Add(new SourcePathsNode());
            parserLayer.ParserMembers.Add(new DefineNode());
            parserLayer.ParserMembers.Add(new ExtensionPathNode());
            parserLayer.ParserMembers.Add(new OutputNode());
            parserLayer.ParserMembers.Add(new TargetNode());
            parserLayer.ParserMembers.Add(new StartNamespaceNode());
            parserLayer.ParserMembers.Add(new IROutputFileNode());
            parserLayer.ParserMembers.Add(new AssemblerOutputNode());
            parserLayer.ParserMembers.Add(new OptimizeNode());
            parserLayer.ParserMembers.Add(new SkipNode());
            parserLayer.ParserMembers.Add(new OSHeaderNode());
            parserLayer.ParserMembers.Add(new ReflectionActiveNode());
            parserLayer.ParserMembers.Add ( new PackageGroupNode ( packagelayer ) );

            return parserLayer;
        }

        // -----------------------------------------------

        private ParserLayer PackageLayer()
        {
            ParserLayer parserLayer = new ParserLayer("package");

            parserLayer.ParserMembers.Add ( new PackageGitRepositoryNode () );
            parserLayer.ParserMembers.Add ( new PackageGitBranchNode () );
            parserLayer.ParserMembers.Add ( new PackageLocalPathNode () );
            parserLayer.ParserMembers.Add ( new PackageNameNode () );
            parserLayer.ParserMembers.Add ( new PackageGitAutoUpdateNode () );

            return parserLayer;
        }

        // -----------------------------------------------

        private List<ParserLayer> GetParserRules (  )
        {
            List<ParserLayer> parserRules = new List<ParserLayer>();

            ParserLayer packagelayer = this.PackageLayer();
            ParserLayer rootLayer = this.RootLayer(packagelayer);
            parserRules.Add(packagelayer);
            parserRules.Add(rootLayer);

            return parserRules;
        }

        // -----------------------------------------------

        private bool Parse(List<IDeserialize> nodes, FileInfo file)
        {
            if (!file.Exists) return false;

            Stream stream;
            try {stream = file.OpenRead();} catch {return false;}

            Parser.Parser p = new Parser.Parser (this.GetParserRules(), this.GetBasicLexer(stream), new ParserInputData(file.FullName, stream));
            ParserLayer? startlayer = p.ParserLayers.Find(t=>t.Name == "root");

            if (startlayer == null) return false;
            p.ErrorNode = new ParserError (  );

            System.Diagnostics.Stopwatch stopWatch = new();
            stopWatch.Start();

            if (!p.Parse(startlayer)) return this.PrintingErrors(p, file, stopWatch);

            IParseTreeNode? node = p.ParentContainer;
            if (node is null) return false;

            nodes.AddRange(node.GetAllChilds.Cast<IDeserialize>());

            return true;
        }

        // -----------------------------------------------

        #endregion Parser

        // -----------------------------------------------

        #region Deserialize

        // -----------------------------------------------

        private Project Deserialize(DirectoryInfo dir, List<IDeserialize> nodes)
        {
            RequestDeserialize request = new RequestDeserialize(new Project());
            request.Project.Directory = dir;

            foreach (IDeserialize node in nodes)
            {
                node.Deserialize(request);
            }

            return request.Project;
        }

        // -----------------------------------------------

        #endregion Deserialize

        // -----------------------------------------------

        #region PrintErrors

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

        private bool PrintingError(string msg, FileInfo? file)
        {
            string filename = file == null ? "config.yproj" : file.FullName;

            this.Output.Print(new SimpleErrorOut($"{filename}: {msg}"));

            return false;
        }

        // -----------------------------------------------

        #endregion PrintErrors

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}