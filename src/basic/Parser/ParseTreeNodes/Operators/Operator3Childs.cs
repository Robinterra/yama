using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator3Childs : IParseTreeNode, IPriority
    {

        #region get/set

        public IParseTreeNode LeftNode
        {
            get;
            set;
        }

        public IParseTreeNode MiddleNode
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
                result.Add ( this.MiddleNode );
                result.Add ( this.RightNode );

                return result;
            }
        }

        public List<string> ValidOperators
        {
            get;
        }

        public SyntaxKind Steuerzeichen
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public Operator3Childs ( int prio )
        {
            this.Prio = prio;
        }

        public Operator3Childs ( List<string> validOperators, SyntaxKind steuerzeichen, int prio )
            : this ( prio )
        {
            this.ValidOperators = validOperators;
            this.Steuerzeichen = steuerzeichen;
        }

        #endregion ctor

        #region methods

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

            Operator3Childs node = new Operator3Childs ( this.Prio );
            node.Token = token;
            token.Node = node;

            SyntaxToken steuerToken = parser.FindAToken ( token, SyntaxKind.DoublePoint );

            if ( steuerToken == null ) return null;

            IParseTreeNode steuerzeichen = parser.ParseCleanToken ( steuerToken );

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );
            node.MiddleNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );
            node.RightNode = parser.ParseCleanToken ( parser.Peek ( steuerzeichen.Token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.MiddleNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }

        #endregion methods
    }
}