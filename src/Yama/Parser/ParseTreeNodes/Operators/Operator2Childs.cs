using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator2Childs : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference VariabelReference
        {
            get;
            set;
        }

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode RightNode
        {
            get;
            set;
        }

        public CompileReferenceCall OperatorCall
        {
            get;
            set;
        } = new CompileReferenceCall();

        public CompileMovResult MovResultCompilen
        {
            get;
            set;
        } = new CompileMovResult();

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

                result.Add ( this.LeftNode );
                result.Add ( this.RightNode );

                return result;
            }
        }

        public SyntaxKind ValidKind
        {
            get;
        }
        public List<string> ValidOperators
        {
            get;
        }
        public IndexVariabelnReference Reference { get; private set; }

        #endregion get/set

        #region ctor

        public Operator2Childs ( int prio )
        {
            this.Prio = prio;
        }

        public Operator2Childs ( List<string> validOperators, int prio )
            : this ( prio )
        {
            this.ValidKind = SyntaxKind.Operator;
            this.ValidOperators = validOperators;
        }

        public Operator2Childs ( SyntaxKind kind, int prio )
            : this ( prio )
        {
            this.ValidKind = kind;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            if (this.ValidKind != SyntaxKind.Operator) return true;

            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != this.ValidKind ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            Operator2Childs node = new Operator2Childs ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );

            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            this.LeftNode.Indezieren(index, parent);
            IndexVariabelnReference varref = container.VariabelnReferences.Last();


            this.RightNode.Indezieren(index, parent);
            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            if (this.Token.Text == "=") return true;

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.RightNode.Compile(compiler, mode);

            if (this.Token.Text == "=")
            {
                mode = "set";

                this.LeftNode.Compile(compiler, mode);

                return true;
            }

            this.MovResultCompilen.Compile(compiler, null, mode);

            this.LeftNode.Compile(compiler, mode);

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            return true;
        }

        #endregion methods
    }
}