using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator2Childs : IParseTreeNode, IPriority
    {

        #region get/set

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

        public List<string> ValidOperators
        {
            get;
        }

        #endregion get/set

        #region ctor

        public Operator2Childs ( int prio )
        {
            this.Prio = prio;
        }

        public Operator2Childs ( List<string> validOperators, int prio )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode SwapChild ( IPriority node )
        {
            IParseTreeNode result = this.RightNode;

            if ( result is IPriority t && t.Prio < node.Prio ) return t.SwapChild ( node );

            this.RightNode = (IParseTreeNode)node;
            this.RightNode.Token.ParentNode = this;

            return result;
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

            Operator2Childs node = new Operator2Childs ( this.Prio );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );
            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            if ( node.LeftNode is IPriority t && t.Prio < this.Prio ) node.LeftNode = t.SwapChild ( node );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}