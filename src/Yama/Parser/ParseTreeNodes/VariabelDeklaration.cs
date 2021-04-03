using Yama.Index;
using System.Collections.Generic;
using Yama.Lexer;
using System.Linq;
using Yama.Compiler;

namespace Yama.Parser
{
    public class VariabelDeklaration : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnDeklaration Deklaration
        {
            get;
            set;
        }

        public IParseTreeNode AccessDefinition
        {
            get;
            set;
        }

        public IParseTreeNode TypeDefinition
        {
            get;
            set;
        }

        public IParseTreeNode GenericDefintion
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

                //result.Add ( this.ChildNode );

                return result;
            }
        }

        public CompileReferenceCall Compilen
        {
            get;
            set;
        } = new CompileReferenceCall();

        public List<IdentifierKind> Ausnahmen
        {
            get;
        }

        #endregion get/set

        #region ctor

        public VariabelDeklaration ( int prio )
        {
            this.Prio = prio;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( IdentifierToken token )
        {
            if (token == null) return false;

            if (token.Kind == IdentifierKind.Word) return true;

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
        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;
            if (token.Kind == IdentifierKind.Int32Bit) return true;
            if (token.Kind == IdentifierKind.Boolean) return true;
            if (token.Kind == IdentifierKind.Char) return true;
            if (token.Kind == IdentifierKind.Byte) return true;
            if (token.Kind == IdentifierKind.Int16Bit) return true;
            if (token.Kind == IdentifierKind.Int64Bit) return true;
            if (token.Kind == IdentifierKind.Float32Bit) return true;
            if (token.Kind == IdentifierKind.Void) return true;

            return false;
        }

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Word )
                if ( !this.CheckHashValidOperator ( request.Token ) ) return null;

            IdentifierToken lexerLeft = request.Parser.Peek ( request.Token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            IdentifierToken lexerRight = request.Parser.Peek ( request.Token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            VariabelDeklaration node = new VariabelDeklaration ( this.Prio );
            node.Token = lexerRight;

            IParseTreeNode callRule = request.Parser.GetRule<ReferenceCall>();

            node.TypeDefinition = callRule.Parse(request);

            if ( node.TypeDefinition == null ) return null;

            lexerRight.Node = node;
            node.TypeDefinition.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (!(request.Parent is IndexContainer container)) return request.Index.CreateError(this);

            IndexVariabelnDeklaration reference = this.Deklaration;

            if (reference == null)
            {
                reference = new IndexVariabelnDeklaration();
                reference.Use = this;
                reference.Name = this.Token.Text;

                this.TypeDefinition.Indezieren(request);
                IndexVariabelnReference type = container.VariabelnReferences.LastOrDefault();

                if (type == null) return request.Index.CreateError(this);

                reference.Type = type;

                this.Deklaration = reference;
            }

            container.VariabelnDeklarations.Add(reference);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (request.Mode == "set")
            {
                IndexVariabelnReference varref = new IndexVariabelnReference();
                varref.Deklaration = this.Deklaration;
                varref.Use = this;
                varref.ParentUsesSet = this.Deklaration.ParentUsesSet;

                this.Compilen.Compile(request.Compiler, varref, request.Mode);
            }

            return true;
        }

        #endregion methods
    }
}