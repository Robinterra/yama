using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using System.Linq;
using Yama.Compiler;

namespace Yama.Parser
{
    public class Operator1ChildLeft : IParseTreeNode, IPriority
    {

        #region get/set

        public IndexVariabelnReference Reference
        {
            get;
            set;
        }

        public IParseTreeNode ChildNode
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

                result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
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

        public List<IdentifierKind> ValidChilds
        {
            get;
        }
        public IndexVariabelnReference VariabelReference { get; private set; }

        #endregion get/set

        #region ctor

        public Operator1ChildLeft ( int prio )
        {
            this.Prio = prio;
        }

        public Operator1ChildLeft ( List<string> validOperators, int prio, List<IdentifierKind> validChilds )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidChild ( IdentifierToken token )
        {
            if (token == null) return false;

            foreach ( IdentifierKind op in this.ValidChilds )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }

        private bool CheckHashValidOperator ( IdentifierToken token )
        {
            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, IdentifierToken token )
        {
            if ( token.Kind != IdentifierKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            IdentifierToken lexerRight = parser.Peek ( token, 1 );

            if ( this.CheckHashValidChild ( lexerRight ) ) return null;

            IdentifierToken lexerLeft = parser.Peek ( token, -1 );

            if ( !this.CheckHashValidChild ( lexerLeft ) ) return null;

            Operator1ChildLeft node = new Operator1ChildLeft ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.ChildNode = parser.ParseCleanToken ( lexerLeft );

            if ( node.ChildNode == null ) return null;

            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            if (!(parent is IndexContainer container)) return index.CreateError(this);

            IndexVariabelnReference reference = new IndexVariabelnReference();
            reference.Use = this;
            reference.Name = this.Token.Text;
            this.ChildNode.Indezieren(index, parent);
            IndexVariabelnReference varref = container.VariabelnReferences.Last();

            this.VariabelReference = reference;
            //container.VariabelnReferences.Add(reference);

            varref.ParentCall = reference;
            varref.VariabelnReferences.Add(reference);
            this.Reference = reference;

            return true;
        }

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            this.ChildNode.Compile(compiler, mode);

            CompilePushResult compilePushResult = new CompilePushResult();
            compilePushResult.Compile(compiler, null, "default");

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            this.FunctionExecute.Compile(compiler, null, mode);

            this.ChildNode.Compile(compiler, "set");

            return true;
        }

        #endregion methods
    }
}