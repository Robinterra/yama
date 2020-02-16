using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Number : IParseTreeNode
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode> (  );
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.NumberToken ) return null;

            Number node = new Number { Token = token };

            token.Node = node;

            return node;
        }
    }
}