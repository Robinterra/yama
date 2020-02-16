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

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser )
        {
            if ( parser.Current.Kind != SyntaxKind.NumberToken ) return null;

            return null;
        }
    }
}