using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System.Linq;

namespace Yama.Parser
{
    public class TypePatternMatching : IParseTreeNode
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public IndexVariabelnReference BooleascherReturn
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IdentifierToken RightToken
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

                result.Add ( this.LeftNode );

                return result;
            }
        }

        public IdentifierKind ValidKind
        {
            get;
        }

        public List<string> ValidOperators
        {
            get;
        }

        public IdentifierToken ReferenceDeklaration
        {
            get;
            set;
        }

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }

        public IndexVariabelnReference EqualsReference
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public TypePatternMatching ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Is ) return null;

            TypePatternMatching node = new TypePatternMatching ( this.Prio );
            node.Token = request.Token;

            node.LeftNode = request.Parser.ParseCleanToken ( request.Parser.Peek ( request.Token, -1 ) );

            node.RightToken = request.Parser.Peek ( request.Token, 1 );
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return null;

            node.ReferenceDeklaration = request.Parser.Peek ( node.RightToken, 1 );
            if (node.ReferenceDeklaration.Kind != IdentifierKind.Word) return null;

            node.Token.Node = node;
            node.LeftNode.Token.ParentNode = node;
            node.RightToken.Node = node;
            node.ReferenceDeklaration.Node = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            this.LeftNode.Indezieren(request);

            IndexVariabelnReference equalsReference = new IndexVariabelnReference();
            equalsReference.Use = this;
            equalsReference.Name = "==";
            this.EqualsReference = equalsReference;
            IndexVariabelnReference varref = new IndexVariabelnReference();
            varref.ParentCall = equalsReference;
            varref.VariabelnReferences.Add(equalsReference);
            varref.Use = this;
            varref.Name = "int";
            container.VariabelnReferences.Add(varref);

            IndexVariabelnDeklaration reference = new IndexVariabelnDeklaration();
            reference.Use = this;
            reference.Name = this.ReferenceDeklaration.Text;
            container.VariabelnDeklarations.Add(reference);
            IndexVariabelnReference type = new IndexVariabelnReference { Name = this.RightToken.Text, Use = this };
            reference.Type = type;
            container.VariabelnReferences.Add(type);

            this.BooleascherReturn = new IndexVariabelnReference { Name = "bool", Use = this };
            container.VariabelnReferences.Add(this.BooleascherReturn);

            this.Deklaration = reference;

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (!(this.Deklaration.Type.Deklaration is IndexKlassenDeklaration t)) return false;
            if (t.InheritanceBase == null && t.InheritanceChilds.Count == 0) return request.Compiler.AddError("The Is Keyword works only with inheritance.", this);

            this.LeftNode.Compile(request);

            CompileReferenceCall compileReference = new CompileReferenceCall();
            ReferenceCall call = new ReferenceCall();
            call.Token = this.ReferenceDeklaration;
            call.Reference = new IndexVariabelnReference { Deklaration = this.Deklaration, Name = this.Deklaration.Name, Use = this, ParentUsesSet = this.BooleascherReturn.ThisUses };
            compileReference.Compile(request.Compiler, call, "set");

            compileReference = new CompileReferenceCall();
            compileReference.CompilePoint0(request.Compiler);

            CompileReferenceCall referenceCall = new CompileReferenceCall();
            referenceCall.CompileData(request.Compiler, this, t.DataRef.JumpPointName);

            if (!(this.EqualsReference.Deklaration.Use is MethodeDeclarationNode mdn)) return false;

            this.CompileCopy(request.Compiler, request.Mode, mdn);

            return request.Compiler.Definition.ParaClean();
        }

        private bool CompileCopy(Compiler.Compiler compiler, string mode, MethodeDeclarationNode t)
        {
            if (t.AccessDefinition == null) return false;
            if (t.AccessDefinition.Kind != IdentifierKind.Copy) return false;

            t.Statement.Compile(new Request.RequestParserTreeCompile(compiler, "default"));

            return true;
        }

        #endregion methods
    }
}