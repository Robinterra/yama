using System.Collections.Generic;
using System.Linq;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class Operator1ChildRight : IParseTreeNode, IPriority
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

        public List<IdentifierKind> ValidChilds
        {
            get;
        }

        public List<IdentifierKind> Ausnahmen
        {
            get;
        }
        public IndexVariabelnReference VariabelReference { get; private set; }

        #endregion get/set

        #region ctor

        public Operator1ChildRight ( int prio )
        {
            this.Prio = prio;
        }

        public Operator1ChildRight ( List<string> validOperators, int prio, List<IdentifierKind> validChilds, List<IdentifierKind> ausnahmen )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
            this.Ausnahmen = ausnahmen;
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

        private bool CheckAusnahmen ( IdentifierToken token )
        {
            if (token == null) return false;

            foreach ( IdentifierKind op in this.Ausnahmen )
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

            IdentifierToken lexerLeft = parser.Peek ( token, -1 );

            if ( this.CheckHashValidChild ( lexerLeft ) && !this.CheckAusnahmen ( lexerLeft ) ) return null;

            IdentifierToken lexerRight = parser.Peek ( token, 1 );

            if ( !this.CheckHashValidChild ( lexerRight ) ) return null;

            Operator1ChildRight node = new Operator1ChildRight ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.ChildNode = parser.ParseCleanToken ( lexerRight );

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

            CompileMovResult movResultLeft = new CompileMovResult();

            movResultLeft.Compile(compiler, null, mode);

            this.OperatorCall.Compile(compiler, this.Reference, "methode");

            CompileUsePara usePara = new CompileUsePara();

            usePara.Compile(compiler);

            this.FunctionExecute.Compile(compiler, null, mode);

            return true;
        }

        #endregion methods
    }
}