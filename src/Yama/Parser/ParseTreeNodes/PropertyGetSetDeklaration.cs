using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;

namespace Yama.Parser
{
    public class PropertyGetSetDeklaration : IParseTreeNode
    {
        private ParserLayer layer;

        #region get/set

        public IndexPropertyGetSetDeklaration Deklaration
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

        public GetKey GetStatement
        {
            get;
            set;
        }

        public SetKey SetStatement
        {
            get;
            set;
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

        public IdentifierToken Token
        {
            get;
            set;
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

        public int GetVariabelCounter
        {
            get;
            private set;
        }

        public int SetVariabelCounter
        {
            get;
            private set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public PropertyGetSetDeklaration()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public PropertyGetSetDeklaration(ParserLayer layer)
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

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;

            return false;
        }

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, PropertyGetSetDeklaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        private IdentifierToken MakeZusatzValid( Parser parser, IdentifierToken token, PropertyGetSetDeklaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            PropertyGetSetDeklaration deklaration = new PropertyGetSetDeklaration();

            IdentifierToken token = this.MakeAccessValid ( request.Parser, request.Token, deklaration );

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );

            //IParseTreeNode rule = new Container(SyntaxKind.BeginContainer, SyntaxKind.CloseContainer);

            if ( token == null ) return null;

            IParseTreeNode container = request.Parser.ParseCleanToken(token, this.layer);

            if (container == null) return null;
            if (!(container is Container ab)) return null;
            if (container.GetAllChilds.Count != 2) return null;

            deklaration.AllTokens.Add(container.Token);

            if (container.GetAllChilds[0] is GetKey gk) deklaration.GetStatement = gk;
            if (container.GetAllChilds[1] is SetKey sk) deklaration.SetStatement = sk;

            if (deklaration.GetStatement == null) return null;
            if (deklaration.SetStatement == null) return null;

            return deklaration;
        }

        private PropertyGetSetDeklaration CleanUp(PropertyGetSetDeklaration deklaration)
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
            MethodeType type = MethodeType.PropertyGetSet;

            if (this.ZusatzDefinition != null) return MethodeType.Static;

            return type;
        }

        private IndexVariabelnReference GetReturnValueIndex(IndexKlassenDeklaration klasse)
        {
            return new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
        }

        private bool IndezierenNonStaticDek(IndexPropertyGetSetDeklaration deklaration)
        {

            if (deklaration.Type != MethodeType.PropertyGetSet) return true;

            IndexVariabelnDeklaration thisdek = new IndexVariabelnDeklaration();
            thisdek.Name = "this";
            deklaration.Parameters.Add(thisdek);

            return true;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexKlassenDeklaration klasse)) return request.Index.CreateError(this);

            IndexPropertyGetSetDeklaration deklaration = new IndexPropertyGetSetDeklaration();
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.ReturnValue = this.GetReturnValueIndex(klasse);
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Type = this.GetMethodeType();

            this.GetStatement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, deklaration));
            this.SetStatement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, deklaration));

            this.IndezierenNonStaticDek(deklaration);

            IndexVariabelnDeklaration invaluesdek = new IndexVariabelnDeklaration();
            invaluesdek.Name = "invalue";
            deklaration.Parameters.Add(invaluesdek);

            this.AddMethode(klasse, deklaration);

            return true;
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexPropertyGetSetDeklaration deklaration)
        {
            if (deklaration.Type == MethodeType.PropertyStaticGetSet) klasse.StaticMethods.Add(deklaration);
            else klasse.Methods.Add(deklaration);

            return true;
        }

        public bool CompileSetMethode(Request.RequestParserTreeCompile request)
        {
            CompileContainer compileContainer = new CompileContainer();
            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();

            request.Compiler.BeginNewMethode( this.SetRegisterInUse, compileContainer, this.SetStatement.Statement.IndexContainer.ThisUses );

            CompileFunktionsDeklaration dek = new CompileFunktionsDeklaration();

            dek.Compile(request.Compiler, this, "set");

            return this.CompileNormalFunktionSet(request.Compiler, compileContainer);
        }

        public bool CompileGetMethode(Compiler.Compiler compiler, string mode = "default")
        {
            CompileContainer compileContainer = new CompileContainer();
            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();

            compiler.BeginNewMethode( this.GetRegisterInUse, compileContainer, this.GetStatement.Statement.IndexContainer.ThisUses );

            CompileFunktionsDeklaration dek = new CompileFunktionsDeklaration();

            dek.Compile(compiler, this, "get");

            return this.CompileNormalFunktionGet(compiler, compileContainer);
        }

        private bool CanCompile(Compiler.Compiler compiler)
        {
            if (compiler.OptimizeLevel == Optimize.None) return true;

            return this.CanCompile();
        }

        public bool CanCompile(int depth = 0)
        {
            if (this.Deklaration.Name == "main") return true;

            bool isused = this.Deklaration.IsInUse(depth);
            if (isused) return true;
            if (this.Deklaration.Klasse.InheritanceBase == null) return false;
            if (!(this.Deklaration.Klasse.InheritanceBase.Deklaration is IndexKlassenDeklaration dek)) return false;
            IMethode parentMethods = dek.Methods.FirstOrDefault(u=>u.Name == this.Deklaration.Name);
            if (parentMethods == null) return false;
            if (!(parentMethods.Use is PropertyGetSetDeklaration t)) return false;
            if (t.Equals(this)) return false;

            return t.CanCompile();
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (!this.CanCompile(request.Compiler)) return true;

            this.CompileGetMethode(request.Compiler, request.Mode);

            this.CompileSetMethode(request);

            return true;
        }

        private bool CompileNormalFunktionSet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            int count = 0;

            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                IParent tp = this.Deklaration.SetUses.Deklarationen.FirstOrDefault(t=>t.Name == node.Name);
                IndexVariabelnDeklaration target = node;
                if (tp is IndexVariabelnDeklaration u) target = u;

                CompilePopResult compilePopResult = new CompilePopResult();
                compilePopResult.Position = count;
                compilePopResult.Compile(compiler, target, "default");

                count++;
            }

            compiler.Definition.ParaClean();

            compileContainer.Begin.Compile(compiler, this, "default");

            this.SetStatement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();
            ende.ArgsCount = count;

            this.SetVariabelCounter = compiler.Definition.VariabelCounter;

            ende.Compile(compiler, this, "set");

            return true;
        }

        private bool CompileNormalFunktionGet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            int count = 0;

            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                if (node.Name == "invalue") continue;

                CompilePopResult compilePopResult = new CompilePopResult();
                compilePopResult.Position = count;
                compilePopResult.Compile(compiler, node, "default");

                count++;
            }

            compiler.Definition.ParaClean();

            compileContainer.Begin.Compile(compiler, this, "default");

            this.GetStatement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();
            ende.ArgsCount = count;
            this.GetVariabelCounter = compiler.Definition.VariabelCounter;

            ende.Compile(compiler, this, "get");

            return true;
        }

        #endregion methods
    }
}