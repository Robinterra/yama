using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator1Child : IParseTreeNode, IPriority
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

        #endregion get/set

        #region ctor

        public Operator1Child ( int prio )
        {
            this.Prio = prio;
        }

        public Operator1Child ( List<string> validOperators, int prio )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
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

            SyntaxToken lexerLeft = parser.Peek ( token, -1 );
            SyntaxToken lexerRight = parser.Peek ( token, 1 );

            Operator1Child node = new Operator1Child ( this.Prio );
            node.Token = token;
            token.Node = node;

            if ( lexerLeft.Kind == SyntaxKind.OpenKlammer ) node.ChildNode = parser.ParseCleanToken ( lexerRight );
            if ( lexerRight.Kind == SyntaxKind.OpenKlammer ) node.ChildNode = parser.ParseCleanToken ( lexerLeft );
            if ( node.ChildNode == null ) return null;

            if ( node.ChildNode is IPriority t && t.Prio < this.Prio ) node.ChildNode = t.SwapChild ( node );

            node.ChildNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}