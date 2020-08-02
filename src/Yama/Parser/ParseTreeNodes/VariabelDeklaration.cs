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

                //result.Add ( this.ChildNode );

                return result;
            }
        }

        public CompileReferenceCall Compilen
        {
            get;
            set;
        } = new CompileReferenceCall();

        public List<SyntaxKind> Ausnahmen
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

        private bool CheckHashValidChild ( SyntaxToken token )
        {
            if (token == null) return false;

            if (token.Kind == SyntaxKind.Word) return true;

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
        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            if (token.Kind == SyntaxKind.Word) return true;
            if (token.Kind == SyntaxKind.Int32Bit) return true;
            if (token.Kind == SyntaxKind.Boolean) return true;
            if (token.Kind == SyntaxKind.Char) return true;
            if (token.Kind == SyntaxKind.Byte) return true;
            if (token.Kind == SyntaxKind.Int16Bit) return true;
            if (token.Kind == SyntaxKind.Int64Bit) return true;
            if (token.Kind == SyntaxKind.Float32Bit) return true;
            if (token.Kind == SyntaxKind.Void) return true;

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Word )
                if ( !this.CheckHashValidOperator ( token ) ) return null;

            SyntaxToken lexerLeft = parser.Peek ( token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) /*&& !this.CheckAusnahmen ( lexerLeft )*/ ) return null;

            SyntaxToken lexerRight = parser.Peek ( token, 1 );

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

            IndexVariabelnDeklaration reference = new IndexVariabelnDeklaration();
            reference.Use = this;
            reference.Name = this.Token.Text;
            container.VariabelnDeklarations.Add(reference);
            this.TypeDefinition.Indezieren(index, parent);
            IndexVariabelnReference type = container.VariabelnReferences.Last();
            reference.Type = type;

            this.Deklaration = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            if (mode == "set")
            {
                IndexVariabelnReference varref = new IndexVariabelnReference();
                varref.Deklaration = this.Deklaration;
                varref.ParentUsesSet = this.Deklaration.ParentUsesSet;

                this.Compilen.Compile(compiler, varref, mode);
            }

            return true;
        }

        #endregion methods
    }
}