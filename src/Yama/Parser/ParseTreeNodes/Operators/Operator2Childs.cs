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

        public CompileExecuteCall FunctionExecute
        {
            get;
            set;
        } = new CompileExecuteCall();

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
                result.Add ( this.RightNode );

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
            this.ValidKind = IdentifierKind.Operator;
            this.ValidOperators = validOperators;
        }

        public Operator2Childs ( IdentifierKind kind, int prio )
            : this ( prio )
        {
            this.ValidKind = kind;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            if (this.ValidKind != IdentifierKind.Operator) return true;

            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
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
            IndexVariabelnReference varref = container.VariabelnReferences.LastOrDefault();


            this.RightNode.Indezieren(index, parent);
            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            if (this.Token.Text == "=") return true;
            if (varref == null) return index.CreateError(this);

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

            CompileMovResult movResultRight = new CompileMovResult();

            movResultRight.Compile(compiler, null, mode);

            this.LeftNode.Compile(compiler, mode);

            CompileMovResult movResultLeft = new CompileMovResult();

            movResultLeft.Compile(compiler, null, mode);

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            CompileUsePara usePara = new CompileUsePara();

            usePara.Compile(compiler);

            usePara = new CompileUsePara();

            usePara.Compile(compiler);

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}