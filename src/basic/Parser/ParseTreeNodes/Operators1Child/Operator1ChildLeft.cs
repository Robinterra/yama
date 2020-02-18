using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator1ChildLeft : IParseTreeNode, IPriority
    {

        #region get/set

        public IParseTreeNode ChildNode
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

                result.Add ( this.ChildNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
        }

        public List<SyntaxKind> ValidChilds
        {
            get;
        }

        #endregion get/set

        #region ctor

        public Operator1ChildLeft ( int prio )
        {
            this.Prio = prio;
        }

        public Operator1ChildLeft ( List<string> validOperators, int prio, List<SyntaxKind> validChilds )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.ValidChilds = validChilds;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode SwapChild ( IPriority node )
        {
            IParseTreeNode result = this.ChildNode;

            if ( result is IPriority t && t.Prio < node.Prio ) return t.SwapChild ( node );

            this.ChildNode = (IParseTreeNode)node;
            this.ChildNode.Token.ParentNode = this;

            return result;
        }

        private bool CheckHashValidChild ( SyntaxToken token )
        {
            if (token == null) return false;

            foreach ( SyntaxKind op in this.ValidChilds )
            {
                if ( op == token.Kind ) return true;
            }

            return false;
        }

        private bool CheckHashValidOperator ( SyntaxToken token )
        {
            foreach ( string op in this.ValidOperators )
            {
                if ( op == token.Text ) return true;
            }

            return false;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Operator ) return null;
            if ( !this.CheckHashValidOperator ( token ) ) return null;

            SyntaxToken lexerRight = parser.Peek ( token, 1 );

            if ( this.CheckHashValidChild ( lexerRight ) ) return null;

            SyntaxToken lexerLeft = parser.Peek ( token, -1 );

            if ( !this.CheckHashValidChild ( lexerLeft ) ) return null;

            Operator1ChildLeft node = new Operator1ChildLeft ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.ChildNode = lexerLeft.Node;

            if ( node.ChildNode == null ) return null;

            if ( node.ChildNode is IPriority t && t.Prio < this.Prio ) node.ChildNode = t.SwapChild ( node );

            if ( node.ChildNode.Token.ParentNode != null )
            {
                node.Token.ParentNode = node.ChildNode.Token.ParentNode;
                //node.Token.ParentNode swap child from parent with this
            }
            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}