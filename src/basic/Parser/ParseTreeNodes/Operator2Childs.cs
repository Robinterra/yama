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

        public IParseTreeNode Parse ( Parser parser )
        {
            if ( parser.Current.Kind != SyntaxKind.Operator ) return null;


            return null;
        }
    }
}