using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;

namespace Yama.Parser
{
    public class VektorDeclaration : IParseTreeNode
    {
        private ParserLayer layer;

        #region get/set

        public IndexMethodDeklaration Deklaration
        {
            get;
            set;
        }

        public SyntaxToken AccessDefinition
        {
            get;
            set;
        }

        public SyntaxToken ZusatzDefinition
        {
            get;
            set;
        }

        public SyntaxToken TypeDefinition
        {
            get;
            set;
        }

        public IParseTreeNode GenericDefintion
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
            set;
        }

        public CompileFunktionsDeklaration FunktionsDeklarationCompile
        {
            get;
            set;
        } = new CompileFunktionsDeklaration();

        public CompileFunktionsEnde FunktionsEndeCompile
        {
            get;
            set;
        } = new CompileFunktionsEnde();

        public IParseTreeNode GetStatement
        {
            get;
            set;
        }

        public IParseTreeNode SetStatement
        {
            get;
            set;
        }

        public SyntaxToken Token
        {
            get;
            set;
        }

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.AddRange ( this.Parameters );
                result.Add ( this.GetStatement );
                result.Add ( this.SetStatement );

                return result;
            }
        }

        public List<SyntaxKind> Ausnahmen
        {
            get;
        }

        public List<string> RegisterInUse
        {
            get;
            set;
        } = new List<string>();

        public CompileContainer CompileContainer
        {
            get;
            set;
        } = new CompileContainer();

        public int VariabelCounter
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public VektorDeclaration()
        {

        }

        public VektorDeclaration(ParserLayer layer)
        {
            this.layer = layer;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidName ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;

            return false;
        }
        private bool CheckHashValidTypeDefinition ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Public) return true;
            if (token.Kind == SyntaxKind.Private) return true;

            return false;
        }

        private SyntaxToken MakeAccessValid( Parser parser, SyntaxToken token, VektorDeclaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {

            VektorDeclaration deklaration = new VektorDeclaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(SyntaxKind.OpenSquareBracket, SyntaxKind.CloseSquareBracket);

            if ( token == null ) return null;

            parser.ActivateLayer(this.layer);

            IParseTreeNode parametersVektor = rule.Parse(parser, token);

            parser.VorherigesLayer();

            if (parametersVektor == null) return null;
            if (!(parametersVektor is Container t)) return null;

            t.Token.ParentNode = deklaration;
            deklaration.Parameters = t.Statements;

            token = parser.Peek ( t.Ende, 1);

            if ( token == null ) return null;

            IParseTreeNode container = parser.ParseCleanToken(token, this.layer);

            if (container == null) return null;
            if (!(container is Container ab)) return null;
            if (container.GetAllChilds.Count != 2) return null;

            deklaration.GetStatement = container.GetAllChilds[0];
            deklaration.SetStatement = container.GetAllChilds[1];

            t.Token.ParentNode = deklaration;

            if (deklaration.GetStatement == null) return null;
            if (deklaration.SetStatement == null) return null;

            if (!(deklaration.GetStatement is GetKey)) return null;
            if (!(deklaration.SetStatement is SetKey)) return null;

            return this.CleanUp(deklaration);
        }

        private VektorDeclaration CleanUp(VektorDeclaration deklaration)
        {
            deklaration.GetStatement.Token.ParentNode = deklaration;
            deklaration.SetStatement.Token.ParentNode = deklaration;

            if (deklaration.AccessDefinition != null)
            {
                deklaration.AccessDefinition.Node = deklaration;
                deklaration.AccessDefinition.ParentNode = deklaration;
            }
            if (deklaration.ZusatzDefinition != null)
            {
                deklaration.ZusatzDefinition.Node = deklaration;
                deklaration.ZusatzDefinition.ParentNode = deklaration;
            }

            deklaration.TypeDefinition.Node = deklaration;
            deklaration.Token.Node = deklaration;

            return deklaration;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.VektorMethode;

            return type;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexKlassenDeklaration klasse)) return index.CreateError(this);

            return true;
        }

        private bool IndezierenNonStaticDek(IndexMethodDeklaration deklaration)
        {
            IndexVariabelnDeklaration thisdek = new IndexVariabelnDeklaration();
            thisdek.Name = "this";
            deklaration.Parameters.Add(thisdek);

            return true;
        }

        private IndexVariabelnReference GetReturnValueIndex(IndexKlassenDeklaration klasse)
        {
            return new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexMethodDeklaration deklaration)
        {
            // @todo add variabeldek methode to index klasse

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        #endregion methods
    }
}