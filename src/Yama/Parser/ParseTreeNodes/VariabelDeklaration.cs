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

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Word )
                if ( !this.CheckHashValidOperator ( token ) ) return null;

            IdentifierToken lexerLeft = parser.Peek ( token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            IdentifierToken lexerRight = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            VariabelDeklaration node = new VariabelDeklaration ( this.Prio );
            node.Token = lexerRight;
            lexerRight.Node = node;

            IParseTreeNode callRule = parser.GetRule<ReferenceCall>();

            node.TypeDefinition = callRule.Parse(parser, token);

            if ( node.TypeDefinition == null ) return null;

            node.TypeDefinition.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnDeklaration reference = this.Deklaration;

            if (reference == null)
            {
                reference = new IndexVariabelnDeklaration();
                reference.Use = this;
                reference.Name = this.Token.Text;

                this.TypeDefinition.Indezieren(index, parent);
                IndexVariabelnReference type = container.VariabelnReferences.Last();
                reference.Type = type;

                this.Deklaration = reference;
            }

            container.VariabelnDeklarations.Add(reference);

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (mode == "set")
            {
                IndexVariabelnReference varref = new IndexVariabelnReference();
                varref.Deklaration = this.Deklaration;
                varref.Use = this;
                varref.ParentUsesSet = this.Deklaration.ParentUsesSet;

                this.Compilen.Compile(compiler, varref, mode);
            }

            return true;
        }

        #endregion methods
    }
}