using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using System;
using Yama.Compiler;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class MethodeDeclarationNode : IParseTreeNode//, IPriority
    {
        private ParserLayer layer;

        #region get/set

        public IndexMethodDeklaration Deklaration
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

        public IParseTreeNode Statement
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
                result.Add ( this.Statement );

                return result;
            }
        }

        public List<IdentifierKind> Ausnahmen
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

        public IndexVariabelnReference Malloc
        {
            get;set;
        } = new IndexVariabelnReference
            {
                Name = "MemoryManager",
                ParentCall = new IndexVariabelnReference
                {
                    Name = "Malloc"
                }
            };

        public IndexVariabelnReference MallocFree
        {
            get;
            set;
        } = new IndexVariabelnReference
            {
                Name = "MemoryManager",
                ParentCall = new IndexVariabelnReference
                {
                    Name = "Free"
                }
            };

        public List<IParseTreeNode> Tags
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public MethodeDeclarationNode()
        {

        }

        public MethodeDeclarationNode(ParserLayer layer)
        {
            this.layer = layer;
        }

        public MethodeDeclarationNode ( int prio )
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
            if (token.Kind == IdentifierKind.New) return true;
            if (token.Kind == IdentifierKind.Operator && token.Text == "~") return true;
            if (token.Kind == IdentifierKind.Implicit) return true;
            if (token.Kind == IdentifierKind.Explicit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

            return false;
        }

        private bool CheckHashValidAccessDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Public) return true;
            if (token.Kind == IdentifierKind.Private) return true;
            if (token.Kind == IdentifierKind.Copy) return true;
            if (token.Kind == IdentifierKind.Simple) return true;

            return false;
        }

        private bool CheckHashValidZusatzDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Static) return true;
            if (token.Kind == IdentifierKind.OperatorKey) return true;
            if (token.Kind == IdentifierKind.This) return true;

            return false;
        }

        private IdentifierToken MakeAccessValid( Parser parser, IdentifierToken token, MethodeDeclarationNode deklaration)
        {
            if ( !this.CheckHashValidAccessDefinition ( token ) ) return token;

            deklaration.AccessDefinition = token;

            return parser.Peek(token, 1);
        }

        private IdentifierToken MakeZusatzValid( Parser parser, IdentifierToken token, MethodeDeclarationNode deklaration)
        {
            if ( !this.CheckHashValidZusatzDefinition ( token ) ) return token;

            deklaration.ZusatzDefinition = token;

            return parser.Peek(token, 1);
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            MethodeDeclarationNode deklaration = new MethodeDeclarationNode();

            IdentifierToken token = this.MakeAccessValid(request.Parser, request.Token, deklaration);

            token = this.MakeZusatzValid ( request.Parser, token, deklaration );

            if ( !this.CheckHashValidTypeDefinition ( token ) ) return null;

            deklaration.TypeDefinition = token;

            if ( !this.CheckSonderRegleung ( deklaration.TypeDefinition ) ) token = request.Parser.Peek ( token, 1 );

            if ( !this.CheckHashValidName ( token ) ) return null;

            deklaration.Token = token;

            token = request.Parser.Peek ( token, 1 );

            IParseTreeNode rule = new Container(IdentifierKind.OpenBracket, IdentifierKind.CloseBracket);

            if ( token == null ) return null;

            request.Parser.ActivateLayer(this.layer);

            IParseTreeNode klammer = rule.Parse(new Request.RequestParserTreeParser(request.Parser, token));

            request.Parser.VorherigesLayer();

            if (klammer == null) return null;
            if (!(klammer is Container t)) return null;

            t.Token.ParentNode = deklaration;
            deklaration.Parameters = t.Statements;

            IdentifierToken Statementchild = request.Parser.Peek ( t.Ende, 1);

            deklaration.Statement = request.Parser.ParseCleanToken(Statementchild, this.layer);

            if (deklaration.Statement == null) return null;

            deklaration.Tags = request.Parser.PopMethodTag (  );

            return this.CleanUp(deklaration);
        }

        private bool CheckSonderRegleung(IdentifierToken typeDefinition)
        {
            if (typeDefinition == null) return false;

            if ( typeDefinition.Kind == IdentifierKind.New) return true;
            if ( typeDefinition.Kind == IdentifierKind.Operator && typeDefinition.Text == "~") return true;

            return false;
        }

        private MethodeDeclarationNode CleanUp(MethodeDeclarationNode deklaration)
        {
            deklaration.Statement.Token.ParentNode = deklaration;

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
            if (!this.CheckSonderRegleung(deklaration.TypeDefinition)) deklaration.TypeDefinition.ParentNode = deklaration;

            deklaration.TypeDefinition.Node = deklaration;
            deklaration.Token.Node = deklaration;

            return deklaration;
        }

        public MethodeType GetMethodeType()
        {
            MethodeType type = MethodeType.Methode;

            if (this.ZusatzDefinition != null)
            {
                if (this.ZusatzDefinition.Kind == IdentifierKind.Static) type = MethodeType.Static;
                if (this.ZusatzDefinition.Kind == IdentifierKind.OperatorKey)
                {
                    type = MethodeType.Operator;
                    if (this.TypeDefinition.Kind == IdentifierKind.Explicit) type = MethodeType.Explicit;
                    if (this.TypeDefinition.Kind == IdentifierKind.Implicit) type = MethodeType.Implicit;
                }
                if (this.ZusatzDefinition.Kind == IdentifierKind.This)
                {
                    if (this.Token.Kind == IdentifierKind.New) type = MethodeType.Ctor;
                    if (this.Token.Text == "~") type = MethodeType.DeCtor;
                }
            }

            return type;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexKlassenDeklaration klasse)) return request.Index.CreateError(this);

            IndexMethodDeklaration deklaration = new IndexMethodDeklaration();
            this.MakeTags(deklaration);
            deklaration.Use = this;
            deklaration.Name = this.Token.Text;
            deklaration.ReturnValue = this.GetReturnValueIndex(klasse);
            this.Deklaration = deklaration;

            AccessModify access = AccessModify.Private;
            if (this.AccessDefinition != null) if (this.AccessDefinition.Kind == IdentifierKind.Public) access = AccessModify.Public;
            deklaration.AccessModify = access;

            deklaration.Type = this.GetMethodeType();

            IndexContainer container = new IndexContainer();
            deklaration.Container = container;

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

                if (dek == null) { request.Index.CreateError(this, "A Index error by the parameters of this method"); continue; }

                if (!dek.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, container))) continue;

                deklaration.Parameters.Add(dek.Deklaration);
            }

            if (deklaration.Type == MethodeType.Static)
            if (this.Token.Text == "main")
                request.Index.SetMainFunction(this);

            if (deklaration.Type == MethodeType.Ctor) container.VariabelnReferences.Add(this.Malloc);
            if (deklaration.Type == MethodeType.DeCtor) container.VariabelnReferences.Add(this.MallocFree);

            this.AddMethode(klasse, deklaration);

            this.Statement.Indezieren(new Request.RequestParserTreeIndezieren(request.Index, container));

            return true;
        }

        private bool MakeTags(IndexMethodDeklaration deklaration)
        {
            if (this.Tags == null) return true;

            foreach (IParseTreeNode node in this.Tags)
            {
                if (!(node is ConditionalCompilationNode ccn)) continue;

                deklaration.Tags.Add(ccn.Tag);
            }

            return true;
        }

        private bool IndezierenNonStaticDek(IndexMethodDeklaration deklaration)
        {
            bool isok = deklaration.Type == MethodeType.Methode;
            if (!isok) isok = deklaration.Type == MethodeType.Property;
            if (!isok) isok = deklaration.Type == MethodeType.Ctor;
            if (!isok) isok = deklaration.Type == MethodeType.DeCtor;
            if (!isok) return false;

            IndexVariabelnDeklaration thisdek = new IndexVariabelnDeklaration();
            thisdek.Name = "this";
            deklaration.Parameters.Add(thisdek);

            return true;
        }

        private IndexVariabelnReference GetReturnValueIndex(IndexKlassenDeklaration klasse)
        {
            if (this.CheckSonderRegleung(this.TypeDefinition)) return new IndexVariabelnReference { Name = klasse.Name, Use = this };

            return new IndexVariabelnReference { Name = this.TypeDefinition.Text, Use = this };
        }

        private bool AddMethode(IndexKlassenDeklaration klasse, IndexMethodDeklaration deklaration)
        {
            if (deklaration.Type == MethodeType.Ctor) klasse.Ctors.Add(deklaration);
            if (deklaration.Type == MethodeType.DeCtor) klasse.DeCtors.Add(deklaration);
            if (deklaration.Type == MethodeType.Operator) klasse.Operators.Add(deklaration);
            if (deklaration.Type == MethodeType.Methode) klasse.Methods.Add(deklaration);
            if (deklaration.Type == MethodeType.Static) klasse.StaticMethods.Add(deklaration);
            if (deklaration.Type == MethodeType.Implicit) klasse.Operators.Add(deklaration);
            if (deklaration.Type == MethodeType.Explicit) klasse.Operators.Add(deklaration);

            return true;
        }

        public bool CompileCopy(RequestParserTreeCompile request)
        {
            Compiler.Compiler compiler = request.Compiler;

            CompileContainer container = new CompileContainer();
            container.Begin = new CompileSprungPunkt();
            container.Ende = new CompileSprungPunkt();

            compiler.PushContainer(container, this.Deklaration.ThisUses, false);

            container.Begin.Compile(compiler, this, "default");

            foreach (IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                CompileReferenceCall rc = new CompileReferenceCall();
                rc.CompileDek(compiler, node, "set");
            }

            this.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            container.Ende.Compile(compiler, this, "default");

            return compiler.PopContainer();
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (!this.CanCompile(request.Compiler)) return true;

            if (this.AccessDefinition != null)
                if (this.AccessDefinition.Kind == IdentifierKind.Copy) return true;

            this.CompileContainer.Begin = new CompileSprungPunkt();
            this.CompileContainer.Ende = new CompileSprungPunkt();

            request.Compiler.BeginNewMethode(this.RegisterInUse, this.CompileContainer, ((Container)this.Statement).IndexContainer.ThisUses);

            if (this.AccessDefinition != null)
                if (this.AccessDefinition.Kind == IdentifierKind.Simple) return request.Compiler.AddError("simple keyword is not anymore supported!", this);

            this.FunktionsDeklarationCompile.Compile(request.Compiler, this, request.Mode);

            int count = 0;
            foreach(IndexVariabelnDeklaration node in this.Deklaration.Parameters)
            {
                if (!this.MakeOneArgument(node, request.Compiler, count)) continue;

                count++;

                if (node.Name != "this") continue;
                if (this.Deklaration.Klasse.InheritanceBase == null) continue;

                request.Compiler.CurrentThis = node;

                bool v = this.BaseCompile(request.Compiler);
            }

            if (this.Deklaration.Type == MethodeType.Ctor) this.CompileCtor(request.Compiler, request.Mode);
            if (this.Deklaration.Type == MethodeType.DeCtor) this.CompileDeCtor(request.Compiler, request.Mode);

            return this.CompileNormalFunktion(request.Compiler, request.Mode, count);
        }

        private bool MakeOneArgument(IndexVariabelnDeklaration node, Compiler.Compiler compiler, int count)
        {
            if (this.Deklaration.Type == MethodeType.Ctor)
                if (node.Name == "this") return false;

            CompilePopResult compilePopResult = new CompilePopResult();
            compilePopResult.Position = count;
            compilePopResult.Compile(compiler, node, "default");

            return true;
        }

        private bool BaseCompile(Compiler.Compiler compiler)
        {
            if (!this.Deklaration.Klasse.IsMethodsReferenceMode) return true;
            if (!(this.Deklaration.Klasse.BaseVar.Type.Deklaration is IndexKlassenDeklaration t)) return true;

            compiler.CurrentBase = this.Deklaration.Klasse.BaseVar;

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(compiler, this, t.DataRef.JumpPointName);

            CompileReferenceCall compileReference = new CompileReferenceCall();
            compileReference.CompileDek(compiler, this.Deklaration.Klasse.BaseVar, "set");

            return true;
        }

        public bool CanCompile(Compiler.Compiler compiler)
        {
            if (this.AccessDefinition != null)
            {
                if (this.AccessDefinition.Kind == IdentifierKind.Copy) return false;
            }

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
            IMethode parentMethods = dek.Methods.FirstOrDefault(u=>u.KeyName == this.Deklaration.KeyName);
            if (parentMethods == null) return false;
            if (!(parentMethods.Use is MethodeDeclarationNode t)) return false;
            if (t.Equals(this)) return false;

            return t.CanCompile();
        }

        private bool CompileDeCtor(Compiler.Compiler compiler, string mode)
        {
            if (this.Deklaration.Klasse.GetNonStaticPropCount == 0) return true;

            /*CompilePopResult compilePopResult = new CompilePopResult();
            compilePopResult.Position = 0;
            compilePopResult.Compile(compiler, this.Deklaration.Parameters.FirstOrDefault(), "default");*/

            CompileReferenceCall refCall = new CompileReferenceCall();
            refCall.CompileDek(compiler, this.Deklaration.Parameters.FirstOrDefault(), "default");

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            refCall = new CompileReferenceCall();
            refCall.Compile(compiler, this.MallocFree.ParentCall, "methode");

            CompileExecuteCall executeCall = new CompileExecuteCall();
            executeCall.Compile(compiler, (MethodeDeclarationNode)this.MallocFree.ParentCall.Deklaration.Use);

            return compiler.Definition.ParaClean();
        }

        private bool CompileCtor(Compiler.Compiler compiler, string mode)
        {
            if (this.Deklaration.Klasse.GetNonStaticPropCount == 0) return true;

            CompileNumConst num = new CompileNumConst();
            num.Compile(compiler, new Number { Token = new IdentifierToken { Value = this.Deklaration.Klasse.GetNonStaticPropCount * compiler.Definition.AdressBytes } }, mode);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            CompileReferenceCall refCall = new CompileReferenceCall();
            refCall.Compile(compiler, this.Malloc.ParentCall, "methode");

            CompileExecuteCall executeCall = new CompileExecuteCall();
            executeCall.Compile(compiler, (MethodeDeclarationNode)this.Malloc.ParentCall.Deklaration.Use);

            IndexVariabelnDeklaration dek = this.Deklaration.Parameters.FirstOrDefault(t=>t.Name == "this");
            CompileReferenceCall a = new CompileReferenceCall();
            a.CompileDek(compiler, dek, "set");

            if (this.Deklaration.Klasse.IsMethodsReferenceMode)
            {
                CompileReferenceCall referenceCall = new CompileReferenceCall();
                referenceCall.CompileData(compiler, this, this.Deklaration.Klasse.DataRef.JumpPointName);

                CompileReferenceCall compileReference = new CompileReferenceCall();
                compileReference.CompileDek(compiler, dek);

                compileReference = new CompileReferenceCall();
                compileReference.CompilePoint0(compiler, "setpoint");
            }

            return compiler.Definition.ParaClean();
        }

        private bool CompileNormalFunktion(Compiler.Compiler compiler, string mode, int count)
        {
            this.CompileContainer.Begin.Compile(compiler, this, "default");

            this.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            this.CompileContainer.Ende.Compile(compiler, this, "default");

            this.FunktionsEndeCompile.ArgsCount = count;
            this.FunktionsEndeCompile.Compile(compiler, this, mode);

            this.VariabelCounter = compiler.Definition.VariabelCounter;

            return true;
        }

        #endregion methods
    }
}