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

        public IndexVektorDeklaration Deklaration
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

                result.AddRange ( this.Parameters );
                result.Add ( this.GetStatement );
                result.Add ( this.SetStatement );

                return result;
            }
        }

        public List<IdentifierKind> Ausnahmen
        {
            get;
        }

        public List<string> SetRegisterInUse
        {
            get;
            set;
        } = new List<string>();

        public List<string> GetRegisterInUse
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
        public int GetVariabelCounter { get; private set; }
        public int SetVariabelCounter { get; private set; }

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

        private bool CheckHashValidName ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }
        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;

            return false;
        }

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, VektorDeclaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;

            return false;
        }

        private IdentifierToken MakeZusatzValid( Parser parser, IdentifierToken token, VektorDeclaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {

            VektorDeclaration deklaration = new VektorDeclaration();

            token = this.MakeAccessValid(parser, token, deklaration);

            token = this.MakeZusatzValid ( parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            token = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket);

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

            container.Token.Node = deklaration;

            deklaration.GetStatement = container.GetAllChilds[0];
            deklaration.SetStatement = container.GetAllChilds[1];

            ab.Token.ParentNode = deklaration;

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

            if (this.ZusatzDefinition != null) return MethodeType.VektorStatic;

            return type;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexKlassenDeklaration klasse)) return index.CreateError(this);

            IndexVektorDeklaration deklaration = new IndexVektorDeklaration();
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.ReturnValue = this.GetReturnValueIndex(klasse);
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Type = this.GetMethodeType();

            this.GetStatement.Indezieren(index, deklaration);
            this.SetStatement.Indezieren(index, deklaration);

            VariabelDeklaration dek = null;

            this.IndezierenNonStaticDek(deklaration);

            foreach (IParseTreeNode par in this.Parameters)
            {
                if (par is VariabelDeklaration t) dek = t;
                if (par is EnumartionExpression b)
                {
                    if (b.ExpressionParent == null) continue;
                    dek = (VariabelDeklaration)b.ExpressionParent;
                }

                if (dek == null) { index.CreateError(this, "A Index error by the parameters of this method"); continue; }

                if (!dek.Indezieren(index, deklaration.SetContainer)) continue;

                deklaration.Parameters.Add(dek.Deklaration);

                if (dek.Token.Text == "invalue") continue;
                if (!dek.Indezieren(index, deklaration.GetContainer)) continue;
            }

            IndexVariabelnDeklaration invaluesdek = new IndexVariabelnDeklaration();
            invaluesdek.Name = "invalue";
            deklaration.Parameters.Add(invaluesdek);

            this.AddMethode(klasse, deklaration);

            return true;
        }

        private bool IndezierenNonStaticDek(IndexVektorDeklaration deklaration)
        {

            if (deklaration.Type != MethodeType.VektorMethode) return true;

            IndexVariabelnDeklaration thisdek = new IndexVariabelnDeklaration();
            thisdek.Name = "this";
            deklaration.Parameters.Add(thisdek);

            return true;
        }

        private IndexVariabelnReference GetReturnValueIndex(IndexKlassenDeklaration klasse)
        {
            return new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexVektorDeklaration deklaration)
        {
            if (deklaration.Type == MethodeType.VektorStatic) klasse.StaticMethods.Add(deklaration);
            else klasse.Methods.Add(deklaration);

            return true;
        }

        public bool CompileSetMethode(Compiler.Compiler compiler, string mode = "default")
        {
            compiler.Definition.BeginNewMethode(this.SetRegisterInUse);

            CompileContainer compileContainer = new CompileContainer();

            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();
            compiler.SetNewContainer(compileContainer);

            CompileFunktionsDeklaration dek = new CompileFunktionsDeklaration();

            dek.Compile(compiler, this, "set");

            return this.CompileNormalFunktionSet(compiler, compileContainer);
        }

        public bool CompileGetMethode(Compiler.Compiler compiler, string mode = "default")
        {
            compiler.Definition.BeginNewMethode(this.GetRegisterInUse);

            CompileContainer compileContainer = new CompileContainer();

            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();
            compiler.SetNewContainer(compileContainer);

            CompileFunktionsDeklaration dek = new CompileFunktionsDeklaration();

            dek.Compile(compiler, this, "get");

            return this.CompileNormalFunktionGet(compiler, compileContainer);
        }

        
        private bool CanCompile(Compiler.Compiler compiler)
        {
            if (compiler.OptimizeLevel != Optimize.Level1) return true;

            return this.Deklaration.IsInUse(0);
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (!this.CanCompile(compiler)) return true;

            this.CompileGetMethode(compiler, mode);

            this.CompileSetMethode(compiler, mode);

            return true;
        }

        private bool CompileNormalFunktionSet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                CompileUsePara usePara = new CompileUsePara();
                usePara.Uses = this.Deklaration.SetContainer.ThisUses;

                IParent tp = this.Deklaration.SetUses.Deklarationen.FirstOrDefault(t=>t.Name == node.Name);
                IndexVariabelnDeklaration target = node;
                if (tp is IndexVariabelnDeklaration u) target = u;

                usePara.CompileIndexNode(compiler, target, "get");
            }

            compiler.Definition.ParaClean();

            compileContainer.Begin.Compile(compiler, this, "default");

            this.SetStatement.Compile(compiler, "default");

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();

            ende.Compile(compiler, this, "set");

            this.SetVariabelCounter = compiler.Definition.VariabelCounter;

            return true;
        }

        private bool CompileNormalFunktionGet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                if (node.Name == "invalue") continue;

                CompileUsePara usePara = new CompileUsePara();
                usePara.Uses = this.Deklaration.GetContainer.ThisUses;
                usePara.CompileIndexNode(compiler, node, "get");
            }

            compiler.Definition.ParaClean();

            compileContainer.Begin.Compile(compiler, this, "default");

            this.GetStatement.Compile(compiler, "default");

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();

            ende.Compile(compiler, this, "get");

            this.GetVariabelCounter = compiler.Definition.VariabelCounter;

            return true;
        }

        #endregion methods
    }
}