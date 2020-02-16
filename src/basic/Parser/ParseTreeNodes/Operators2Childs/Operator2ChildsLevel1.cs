using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator2ChildsLevel1 : IParseTreeNode, IPriority
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
            get
            {
                return 1;
            }
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

        #endregion get/set

        public IParseTreeNode SwapChild ( IParseTreeNode node )
        {
            IParseTreeNode result = this.RightNode;

            this.RightNode = node;
            node.Token.ParentNode = node;

            return result;
        }

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Operator ) return null;

            Operator2ChildsLevel1 node = new Operator2ChildsLevel1 (  );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );
            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            if ( node.LeftNode is IPriority t && t.Prio > this.Prio ) node.LeftNode = t.SwapChild ( node );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
/*
            Operator2Childs node = new Operator2Childs (  );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );
            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;*/
        }
    }
}