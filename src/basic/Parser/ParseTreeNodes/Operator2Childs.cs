using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Operator2Childs : IParseTreeNode
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

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Operator ) return null;

            Operator2Childs node = new Operator2Childs (  );
            node.Token = token;
            token.Node = node;

            node.LeftNode = parser.ParseCleanToken ( parser.Peek ( token, -1 ) );
            node.RightNode = parser.ParseCleanToken ( parser.Peek ( token, 1 ) );

            node.LeftNode.Token.ParentNode = node;
            node.RightNode.Token.ParentNode = node;

            return node;
        }
    }
}