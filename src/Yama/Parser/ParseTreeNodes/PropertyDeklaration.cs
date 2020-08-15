using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;

namespace Yama.Parser
{
    public class PropertyDeklaration : IParseTreeNode//, IPriority
    {
        private ParserLayer layer;

        #region get/set

        public IndexPropertyDeklaration Deklaration
        {
            get;
            set;
        }

        public IdentifierToken AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken ZusatzDefinition
        {
            get;
            set;
        }

        public IdentifierToken TypeDefinition
        {
            get;
            set;
        }

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

        public List<string> RegisterInUseGet
        {
            get;
            set;
        } = new List<string>();

        public List<string> RegisterInUseSet
        {
            get;
            set;
        } = new List<string>();

        public CompileFunktionsDeklaration FunktionsDeklarationCompileGet
        {
            get;
            set;
        } = new CompileFunktionsDeklaration();

        public CompileFunktionsEnde FunktionsEndeCompileSet
        {
            get;
            set;
        } = new CompileFunktionsEnde();
        public CompileFunktionsDeklaration FunktionsDeklarationCompileSet
        {
            get;
            set;
        } = new CompileFunktionsDeklaration();

        public CompileFunktionsEnde FunktionsEndeCompileGet
        {
            get;
            set;
        } = new CompileFunktionsEnde();

        public IdentifierToken Token
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

                result.Add ( this.GetStatement );
                result.Add ( this.SetStatement );

                return result;
            }
        }

        public List<IdentifierKind> Ausnahmen
        {
            get;
        }
        public CompileContainer CompileContainerGet
        {
            get;
            set;
        } = new CompileContainer();

        public CompileContainer CompileContainerSet
        {
            get;
            set;
        } = new CompileContainer();

        #endregion get/set

        #region ctor

        public PropertyDeklaration()
        {

        }

        public PropertyDeklaration(ParserLayer layer)
        {
            this.layer = layer;
        }

        public PropertyDeklaration ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidName ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Operator) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;
            if (token.Kind == IdentifierKind.New) return true;

            return false;
        }

        /*private bool CheckAusnahmen ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.Ausnahmen )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }*/
        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;
            if (token.Kind == IdentifierKind.This) return true;
            if (token.Kind == IdentifierKind.Implicit) return true;
            if (token.Kind == IdentifierKind.Explicit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;

            return false;
        }

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;
            if (token.Kind == IdentifierKind.OperatorKey) return true;

            return false;
        }

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private IdentifierToken MakeZusatzValid( Parser parser, IdentifierToken token, PropertyDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {

            PropertyDeklaration deklaration = new PropertyDeklaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            token = this.MakeZusatzValid ( parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            token = parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            //IParseTreeNode rule = new Container(SyntaxKind.BeginContainer, SyntaxKind.CloseContainer);

            if ( token == null ) return null;

            IParseTreeNode klammer = parser.ParseCleanToken(token, this.layer);

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;
            if (klammer.GetAllChilds.Count != 2) return null;

            deklaration.GetStatement = klammer.GetAllChilds[0];
            deklaration.SetStatement = klammer.GetAllChilds[1];

            t.Token.ParentNode = deklaration;

            if (deklaration.GetStatement == null) return null;
            if (deklaration.SetStatement == null) return null;

            if (!(deklaration.GetStatement is GetKey)) return null;
            if (!(deklaration.SetStatement is SetKey)) return null;

            return this.CleanUp(deklaration);
        }

        private PropertyDeklaration CleanUp(PropertyDeklaration deklaration)
        {
            deklaration.SetStatement.Token.ParentNode = deklaration;
            deklaration.GetStatement.Token.ParentNode = deklaration;

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
            deklaration.TypeDefinition.ParentNode = deklaration;
            deklaration.Token.Node = deklaration;

            return deklaration;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.Property;

            if (this.ZusatzDefinition != null)
            {
                if (this.ZusatzDefinition.Kind == IdentifierKind.Static) type = MethodeType.Static;
            }

            return type;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexKlassenDeklaration klasse)) return index.CreateError(this);

            IndexPropertyDeklaration deklaration = new IndexPropertyDeklaration();
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.Type = new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Zusatz = this.GetMethodeType();

            this.AddMethode(klasse, deklaration);

            this.SetStatement.Indezieren(index, deklaration);
            this.GetStatement.Indezieren(index, deklaration);

            return true;
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexPropertyDeklaration deklaration)
        {
            klasse.IndexProperties.Add(deklaration);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        /*private bool CompileGetFunktion(Compiler.Compiler compiler, string mode)
        {
            compiler.Definition.BeginNeueMethode(this.RegisterInUseGet);

            this.CompileContainerGet.Begin = new CompileSprungPunkt();
            this.CompileContainerGet.Ende = new CompileSprungPunkt();
            compiler.SetNewContainer(this.CompileContainerGet);

            this.FunktionsDeklarationCompileGet.Compile(compiler, this, mode);

            foreach(IndexVariabelnDeklaration node in this.Deklaration.ParametersGet)
            {
                CompileUsePara usePara = new CompileUsePara();

                usePara.CompileIndexNode(compiler, node, "get");
            }

            compiler.Definition.ParaClean();

            this.CompileContainer.Begin.Compile(compiler, this, mode);

            this.Statement.Compile(compiler, mode);

            this.CompileContainer.Ende.Compile(compiler, this, mode);

            this.FunktionsEndeCompile.Compile(compiler, this, mode);

            return true;
        }*/

        #endregion methods
    }
}