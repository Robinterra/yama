using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Yama.Compiler.Definition;
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
            set;
        }

        // -----------------------------------------------

        public Dictionary<string, Package> BinPackages
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public ConfigDefinition ()
        {
            this.BinPackages = new Dictionary<string, Package> ();
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

            Project project = this.Deserialize(file.Directory, nodes);
            if (project == null) return false;

            if (!this.TranslateToDefinition(project, file, definition)) return false;

            if ( !ismain ) return true;

            return this.TranslateToDefinitionMain ( project, file, definition );
        }

        // -----------------------------------------------

        private bool TranslateToDefinitionMain(Project project, FileInfo file, LanguageDefinition definition)
        {
            definition.Definition = this.TargetManager.GetDefinition(project.TargetPlattform);
            if (definition.Definition == null) return false;

            if (project.Skip < 0) return this.PrintingError("skip is lower 0, that is not allowed", file);
            definition.StartPosition = (uint)project.Skip;

            if ( !string.IsNullOrEmpty ( project.StartNamespace ) ) definition.StartNamespace = project.StartNamespace;

            if ( project.Optimize != Optimize.SSA ) definition.OptimizeLevel = project.Optimize;

            if ( project.OutputFile != null ) definition.OutputFile = project.OutputFile;
            if ( project.IROutputFile != null ) definition.IROutputFile = project.IROutputFile;
            if ( project.AssemblerOutputFile != null ) definition.OutputAssemblerFile = project.AssemblerOutputFile;

            return true;
        }

        // -----------------------------------------------

        private bool TranslateToDefinition(Project project, FileInfo file, LanguageDefinition definition)
        {
            definition.Defines.AddRange(project.Defines);

            definition.Includes.AddRange(project.SourcePaths);

            definition.Extensions.AddRange(project.ExtensionsPaths);

            foreach ( FileInfo langDef in project.LanguageDefinitions )
            {
                this.TargetManager.AddDefinition ( langDef );
            }

            bool isok = true;
            foreach ( Package package in project.Packages )
            {
                if ( !this.BindPackage ( definition, package ) ) isok = false;
            }

            return isok;
        }

        // -----------------------------------------------

        private bool BindPackage ( LanguageDefinition definition, Package package )
        {
            if ( string.IsNullOrEmpty ( package.Name ) ) return false;
            if ( this.BinPackages.ContainsKey ( package.Name ) ) return true;

            this.BinPackages.Add(package.Name, package);

            string packagePath = Path.Combine ( Program.PackagePath.FullName, package.Name );
            DirectoryInfo packDir = new DirectoryInfo ( packagePath );

            if ( !this.TryCloneOrPullRepository ( packDir, package ) ) return this.PrintingError(string.Format("can not get newest git repository {0}", package.Name), null);

            FileInfo projectConfig = new FileInfo ( Path.Combine ( packDir.FullName, "config.yproj" ) );
            if ( !projectConfig.Exists ) return this.PrintingError("Project config file can not be found", projectConfig);

            return this.BuildOne ( definition, projectConfig );
        }

        // -----------------------------------------------

        private bool TryCloneOrPullRepository ( DirectoryInfo packDir, Package package )
        {
            if ( !packDir.Exists ) return this.ClonePackage (packDir, package);

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
                return false;
            }

            return true;
        }

        // -----------------------------------------------

        private bool ClonePackage ( DirectoryInfo packDir, Package package )
        {
            CloneOptions cloneOptions = new CloneOptions ();

            try
            {
                Repository.Clone ( package.GitRepository, packDir.FullName, cloneOptions );

                Repository repository = new Repository ( packDir.FullName );

                Commands.Checkout ( repository, package.GitBranch );
            }
            catch
            {
                return false;
            }

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
            rules.Add ( new Words ( new List<ILexerToken> () { new HigherAlpabet (  ), new LowerAlpabet (  ), new Digit (  ), new Underscore (  ), new Punctuation( new ZeichenKette("."), IdentifierKind.Point) } ) );

            return rules;
        }

        // -----------------------------------------------

        private Lexer.Lexer GetBasicLexer()
        {
            Lexer.Lexer lexer = new Lexer.Lexer();

            lexer.LexerTokens = this.GetLexerRules();

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
            parserLayer.ParserMembers.Add ( new PackageGroupNode ( packagelayer ) );

            return parserLayer;
        }

        // -----------------------------------------------

        private ParserLayer PackageLayer()
        {
            ParserLayer parserLayer = new ParserLayer("package");

            parserLayer.ParserMembers.Add ( new PackageGitRepositoryNode () );
            parserLayer.ParserMembers.Add ( new PackageGitBranchNode () );

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
            Parser.Parser p = new Parser.Parser ( file, this.GetParserRules(), this.GetBasicLexer() );
            ParserLayer startlayer = p.ParserLayers.FirstOrDefault(t=>t.Name == "root");

            p.ErrorNode = new ParserError (  );

            if (!p.Parse(startlayer)) return this.PrintingErrors(p);

            IParseTreeNode node = p.ParentContainer;

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
            RequestDeserialize request = new RequestDeserialize();
            request.Project = new Project();
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

        private bool PrintingErrors(Parser.Parser p)
        {
            foreach ( IParseTreeNode error in p.ParserErrors )
            {
                IdentifierToken token = error.Token;

                if (token.Kind == IdentifierKind.Unknown) token = error.Token.ParentNode.Token;

                p.PrintSyntaxError ( token, token.Text );
            }

            return false;
        }

        // -----------------------------------------------

        private bool PrintingError(string msg, FileInfo file)
        {
            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            string filename = file == null ? "config.yproj" : file.FullName;

            Console.Error.WriteLine ( "{0}: {1}", filename, msg );

            Console.ForegroundColor = colr;

            return false;
        }

        // -----------------------------------------------

        #endregion PrintErrors

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}