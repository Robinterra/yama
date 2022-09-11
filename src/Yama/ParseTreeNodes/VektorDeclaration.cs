using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;

namespace Yama.Parser
{
    public class VektorDeclaration : IParseTreeNode, IIndexNode, ICompileNode
    {
        private ParserLayer getsetlayer;

        private ParserLayer methodeVektorDeklarationsHeader;

        #region get/set

        public IndexVektorDeklaration? Deklaration
        {
            get;
            set;
        }

        public IdentifierToken? AccessDefinition
        {
            get;
            set;
        }

        public IdentifierToken? ZusatzDefinition
        {
            get;
            set;
        }

        public IdentifierToken? TypeDefinition
        {
            get;
            set;
        }

        public List<IParseTreeNode> Parameters
        {
            get;
        }

        public GetKey? GetStatement
        {
            get;
            set;
        }

        public SetKey? SetStatement
        {
            get;
            set;
        }

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

                result.AddRange ( this.Parameters );
                if (this.GetStatement is not null) result.Add ( this.GetStatement );
                if (this.SetStatement is not null) result.Add ( this.SetStatement );

                return result;
            }
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

        public string InValue
        {
            get;
            private set;
        }

        public IdentifierToken? BorrowingToken
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public VektorDeclaration(ParserLayer getsetlayer, ParserLayer methodeVektorDeklarationsHeader)
        {
            this.InValue = string.Empty;
            this.Parameters = new();
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.getsetlayer = getsetlayer;
            this.methodeVektorDeklarationsHeader = methodeVektorDeklarationsHeader;
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

        private IdentifierToken? MakeAccessValid( Parser parser, IdentifierToken token, VektorDeclaration deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;

            return false;
        }

        private IdentifierToken? MakeZusatzValid( Parser parser, IdentifierToken token, VektorDeclaration deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;
            deklaration.AllTokens.Add(token);

            return parser.Peek(token, 1);
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            VektorDeclaration deklaration = new VektorDeclaration(this.getsetlayer, this.methodeVektorDeklarationsHeader);

            IdentifierToken? token = this.MakeAccessValid(request.Parser, request.Token, deklaration);
            if (token is null) return null;

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );
            if (token is null) return null;

            token = this.TryParseBorrwoing(request.Parser, token, deklaration);
            if (token is null) return null;
            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );
            if (token is null) return null;

            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(IdentifierKind.OpenSquareBracket, IdentifierKind.CloseSquareBracket);

            if ( token == null ) return null;

            request.Parser.ActivateLayer(this.methodeVektorDeklarationsHeader);

            IParseTreeNode? parametersVektor = request.Parser.TryToParse ( rule, token );

            request.Parser.VorherigesLayer();

            if (parametersVektor is null) return null;
            if (parametersVektor is not Container t) return null;

            t.Token.ParentNode = deklaration;
            deklaration.Parameters.AddRange(t.Statements);

            token = request.Parser.Peek ( t.Ende, 1);
            if ( token is null ) return null;

            IParseTreeNode? container = request.Parser.ParseCleanToken(token, this.getsetlayer, false);

            if (container is not Container ab) return null;

            ab.Token.ParentNode = deklaration;

            foreach (IParseTreeNode node in ab.GetAllChilds)
            {
                if (node is GetKey gk) deklaration.GetStatement = gk;
                if (node is SetKey sk) deklaration.SetStatement = sk;
            }

            if (deklaration.GetStatement is null) return new ParserError(deklaration.Token, "The Property need to have a get statement", deklaration.AllTokens.ToArray());
            if (deklaration.SetStatement is null) return new ParserError(deklaration.Token, "The Property need to have a set statement", deklaration.AllTokens.ToArray());

            return deklaration;
        }

        private IdentifierToken? TryParseBorrwoing(Parser parser, IdentifierToken token, VektorDeclaration node)
        {
            if (token.Kind != IdentifierKind.Operator) return token;
            if (token.Text != "&") return token;

            node.BorrowingToken = token;
            node.AllTokens.Add(token);

            IdentifierToken? nextToken = parser.Peek(token, 1);
            return nextToken;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.VektorMethode;

            if (this.ZusatzDefinition != null) return MethodeType.VektorStatic;

            return type;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexKlassenDeklaration klasse) return request.Index.CreateError(this);
            if (this.GetStatement is null) return request.Index.CreateError(this);
            if (this.SetStatement is null) return request.Index.CreateError(this);
            if (this.TypeDefinition is null) return request.Index.CreateError(this);

            IndexVektorDeklaration deklaration = new IndexVektorDeklaration(this, this.Token.Text, this.GetReturnValueIndex(klasse, this.TypeDefinition));
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Type = this.GetMethodeType();

            this.GetStatement.Indezieren(new RequestParserTreeIndezieren ( request.Index, deklaration ));
            this.SetStatement.Indezieren(new RequestParserTreeIndezieren ( request.Index, deklaration ));

            VariabelDeklaration? dek = null;

            this.InValue = request.Index.Nameing.InValue;

            this.IndezierenNonStaticDek(deklaration, klasse, request.Index.Nameing);

            foreach (IParseTreeNode par in this.Parameters)
            {
                if (par is VariabelDeklaration t) dek = t;

                if (dek is null) { request.Index.CreateError(this, "A Index error by the parameters of this method"); continue; }
                if (!dek.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration.SetContainer))) continue;
                if (dek.Deklaration is null) return request.Index.CreateError(this);

                deklaration.Parameters.Add(dek.Deklaration);

                if (dek.Token.Text == this.InValue) continue;
                if (!dek.Indezieren(new RequestParserTreeIndezieren(request.Index, deklaration.GetContainer))) continue;
            }

            IndexVariabelnDeklaration invaluesdek = new IndexVariabelnDeklaration(this, this.InValue, deklaration.ReturnValue);
            deklaration.Parameters.Add(invaluesdek);

            this.AddMethode(klasse, deklaration);

            return true;
        }

        private bool IndezierenNonStaticDek(IndexVektorDeklaration deklaration, IndexKlassenDeklaration klasse, VariableNameing nameing)
        {
            if (deklaration.Type != MethodeType.VektorMethode) return true;

            IndexVariabelnReference varref = new IndexVariabelnReference(this, klasse.Name);
            IndexVariabelnDeklaration thisdek = new IndexVariabelnDeklaration(this, nameing.This, varref);

            deklaration.Parameters.Add(thisdek);

            return true;
        }

        private IndexVariabelnReference GetReturnValueIndex(IndexKlassenDeklaration klasse, IdentifierToken typeDef)
        {
            return new IndexVariabelnReference(this, typeDef.Text);
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexVektorDeklaration deklaration)
        {
            if (deklaration.Type == MethodeType.VektorStatic) klasse.StaticMethods.Add(deklaration);
            else klasse.Methods.Add(deklaration);

            return true;
        }

        public bool CompileSetMethode(Compiler.Compiler compiler, string mode = "default")
        {
            if (this.SetStatement is null) return false;

            Container? c = this.SetStatement.Statement;
            if (c is null) return false;
            if (c.IndexContainer is null) return false;

            CompileContainer compileContainer = new CompileContainer();
            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();

            compiler.BeginNewMethode( this.SetRegisterInUse, compileContainer, c.IndexContainer.ThisUses );

            CompileFunktionsDeklaration dek = new CompileFunktionsDeklaration();

            dek.Compile(compiler, this, "set");

            return this.CompileNormalFunktionSet(compiler, compileContainer);
        }

        private void CompileReturnType(CompileContainer compileContainer, IParent? deklaration, bool isBorrowing, IndexVariabelnReference varref)
        {
            if (deklaration is not IndexKlassenDeklaration dk) return;

            SSAVariableMap.VariableType kind = SSAVariableMap.VariableType.StackValue;
            IndexVariabelnDeklaration vardek = new IndexVariabelnDeklaration(this, dk.Name, varref);
            if (dk.MemberModifier == ClassMemberModifiers.None)
            {
                kind = isBorrowing ? SSAVariableMap.VariableType.BorrowingReference : SSAVariableMap.VariableType.OwnerReference;

                vardek.IsReference = true;
            }

            SSAVariableMap map = new SSAVariableMap(dk.Name, kind, vardek);

            compileContainer.ReturnType = map;
        }

        public bool CompileGetMethode(Compiler.Compiler compiler, string mode = "default")
        {
            if (this.GetStatement is null) return false;
            if (this.Deklaration is null) return false;

            Container? c = this.GetStatement.Statement;
            if (c is null) return false;
            if (c.IndexContainer is null) return false;

            CompileContainer compileContainer = new CompileContainer();
            compileContainer.Begin = new CompileSprungPunkt();
            compileContainer.Ende = new CompileSprungPunkt();

            this.CompileReturnType(compileContainer, this.Deklaration.ReturnValue.Deklaration, this.BorrowingToken is not null, this.Deklaration.ReturnValue );

            compiler.BeginNewMethode( this.GetRegisterInUse, compileContainer, c.IndexContainer.ThisUses );

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
            if (this.Deklaration is null) return false;
            if (this.Deklaration.Name == "main") return true;

            bool isused = this.Deklaration.IsInUse(depth);
            if (isused) return true;
            if (this.Deklaration.Klasse is null) return false;
            if (this.Deklaration.Klasse.InheritanceBase is null) return false;
            if (this.Deklaration.Klasse.InheritanceBase.Deklaration is not IndexKlassenDeklaration dek) return false;

            IMethode? parentMethods = dek.Methods.FirstOrDefault(u=>u.Name == this.Deklaration.Name);
            if (parentMethods is null) return false;
            if (parentMethods.Use is not VektorDeclaration t) return false;
            if (t.Equals(this)) return false;

            return t.CanCompile();
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (!this.CanCompile(request.Compiler)) return true;

            this.CompileGetMethode(request.Compiler, request.Mode);

            this.CompileSetMethode(request.Compiler, request.Mode);

            return true;
        }

        private bool CompileNormalFunktionSet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            if (this.Deklaration is null) return false;
            if (this.SetStatement is null) return false;

            int count = 0;
            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                IParent? tp = this.Deklaration.SetUses.Deklarationen.FirstOrDefault(t=>t.Name == node.Name);
                IndexVariabelnDeklaration target = node;
                if (tp is IndexVariabelnDeklaration u) target = u;

                CompilePopResult compilePopResult = new CompilePopResult();
                compilePopResult.Position = count;
                compilePopResult.Compile(compiler, target, "default");

                count++;
            }

            compiler.Definition.ParaClean();

            if (compileContainer.Begin is null) return false;

            compileContainer.Begin.Compile(compiler, this, "default");

            this.SetStatement.Compile(new RequestParserTreeCompile(compiler, "default"));

            if (compileContainer.Ende is null) return false;

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();
            ende.ArgsCount = count;
            ende.Compile(compiler, this, "set");

            this.SetVariabelCounter = compiler.Definition.VariabelCounter;

            return true;
        }

        private bool CompileNormalFunktionGet(Compiler.Compiler compiler, CompileContainer compileContainer)
        {
            if (this.Deklaration is null) return false;
            if (this.GetStatement is null) return false;

            int count = 0;
            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                if (node.Name == this.InValue) continue;

                CompilePopResult compilePopResult = new CompilePopResult();
                compilePopResult.Position = count;
                compilePopResult.Compile(compiler, node, "default");

                count++;
            }

            compiler.Definition.ParaClean();

            if (compileContainer.Begin is null) return false;

            compileContainer.Begin.Compile(compiler, this, "default");

            this.GetStatement.Compile(new RequestParserTreeCompile(compiler, "default"));

            if (compileContainer.Ende is null) return false;

            compileContainer.Ende.Compile(compiler, this, "default");

            CompileFunktionsEnde ende = new CompileFunktionsEnde();
            ende.ArgsCount = count;
            ende.Compile(compiler, this, "get");

            this.GetVariabelCounter = compiler.Definition.VariabelCounter;

            return true;
        }

        #endregion methods
    }
}