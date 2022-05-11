using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System.Linq;
using Yama.Parser.Request;

namespace Yama.Parser
{
    public class TypePatternMatching : IParseTreeNode, IIndexNode, ICompileNode, IParentNode, IContainer
    {

        #region get/set

        public IndexVariabelnReference? BooleascherReturn
        {
            get;
            set;
        }

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public IdentifierToken? RightToken
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

                if (this.LeftNode is not null) result.Add ( this.LeftNode );

                return result;
            }
        }

        public IdentifierToken? ReferenceDeklaration
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration? Deklaration
        {
            get;
            set;
        }

        public IndexVariabelnReference? EqualsReference
        {
            get;
            set;
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public bool IsNullChecking
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.ReferenceDeklaration is not null) return this.ReferenceDeklaration;
                if (this.RightToken is not null) return this.RightToken;

                return this.Token;
            }
        }

        #endregion get/set

        #region ctor

        public TypePatternMatching ( int prio )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;
            if ( token.Kind == IdentifierKind.Null ) return true;

            return false;
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Is ) return null;

            TypePatternMatching node = new TypePatternMatching ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            node.RightToken = request.Parser.Peek ( request.Token, 1 );
            if ( node.RightToken == null ) return new ParserError(request.Token, $"Expectet a word after the is keyword");

            node.AllTokens.Add(node.RightToken);
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return new ParserError(request.Token, $"Expectet a word after the is keyword and not a '{node.RightToken.Text}'", node.RightToken);
            if ( node.RightToken.Kind == IdentifierKind.Null ) return this.ParseNullChecking (node);

            node.ReferenceDeklaration = request.Parser.Peek ( node.RightToken, 1 );
            if ( node.ReferenceDeklaration == null ) return null;
            if (node.ReferenceDeklaration.Kind != IdentifierKind.Word) return null;

            node.AllTokens.Add(node.ReferenceDeklaration);

            return node;
        }

        private IParseTreeNode ParseNullChecking ( TypePatternMatching node )
        {
            node.IsNullChecking = true;

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);

            leftNode.Indezieren(request);

            IndexVariabelnReference equalsReference = new IndexVariabelnReference(this, "==");
            this.EqualsReference = equalsReference;

            IndexVariabelnReference varref = new IndexVariabelnReference(this, "int");
            varref.ParentCall = equalsReference;
            varref.VariabelnReferences.Add(equalsReference);
            container.VariabelnReferences.Add(varref);

            this.BooleascherReturn = new IndexVariabelnReference(this, "bool");
            container.VariabelnReferences.Add(this.BooleascherReturn);

            if ( this.IsNullChecking ) return true;
            if (this.ReferenceDeklaration is null) return false;
            if (this.RightToken is null) return false;

            IndexVariabelnReference type = new IndexVariabelnReference(this, this.RightToken.Text);
            IndexVariabelnDeklaration reference = new IndexVariabelnDeklaration(this, this.ReferenceDeklaration.Text, type);
            reference.Use = this;
            reference.Name = this.ReferenceDeklaration.Text;
            container.VariabelnDeklarations.Add(reference);
            container.VariabelnReferences.Add(type);

            this.Deklaration = reference;

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;
            if ( this.IsNullChecking ) return this.CompileNullChecking ( request, leftNode );
            if (this.Deklaration is null) return false;
            if (this.Deklaration.Type.Deklaration is not IndexKlassenDeklaration t) return false;
            if (t.InheritanceBase == null && t.InheritanceChilds.Count == 0) return request.Compiler.AddError("The Is Keyword works only with inheritance.", this);
            if (this.ReferenceDeklaration is null) return false;
            if (this.EqualsReference is null) return false;
            if (this.BooleascherReturn is null) return false;
            if (t.DataRef is null) return false;

            leftNode.Compile(request);

            CompileReferenceCall compileReference = new CompileReferenceCall();
            ReferenceCall call = new ReferenceCall();
            call.Token = this.ReferenceDeklaration;
            call.Reference = new IndexVariabelnReference(this, this.Deklaration.Name) { Deklaration = this.Deklaration, ParentUsesSet = this.BooleascherReturn.ThisUses };
            compileReference.Compile(request.Compiler, call, "set");

            compileReference = new CompileReferenceCall();
            compileReference.CompilePoint0(request.Compiler);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(request.Compiler, this, t.DataRef.JumpPointName!);

            if (this.EqualsReference.Deklaration is null) return false;
            if (this.EqualsReference.Deklaration.Use is not MethodeDeclarationNode mdn) return false;

            this.CompileCopy(request.Compiler, request.Mode, mdn);

            return request.Compiler.Definition.ParaClean();
        }

        private bool CompileNullChecking ( RequestParserTreeCompile request, ICompileNode leftNode )
        {
            if (this.EqualsReference is null) return false;

            this.Token.Value = 0;
            leftNode.Compile(new RequestParserTreeCompile(request.Compiler, "nullChecking"));

            CompileNumConst compileNumConst = new CompileNumConst ();
            compileNumConst.Compile(request.Compiler, new Number { Token = this.Token }, request.Mode);

            if (this.EqualsReference.Deklaration is null) return false;
            if (this.EqualsReference.Deklaration.Use is not MethodeDeclarationNode mdn) return false;

            this.CompileCopy(request.Compiler, request.Mode, mdn);

            return request.Compiler.Definition.ParaClean();
        }

        private bool CompileCopy(Compiler.Compiler compiler, string mode, MethodeDeclarationNode t)
        {
            if (t.Statement is not ICompileNode statementNode) return false;
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;

            statementNode.Compile(new RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        #endregion methods
    }
}