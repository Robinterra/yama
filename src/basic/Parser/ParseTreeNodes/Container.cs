using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class Container : IParseTreeNode
    {

        #region get/set

        public List<IParseTreeNode> Statements
        {
            get;
            set;
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser )
        {
            if ( parser.Current.Kind != SyntaxKind.BeginContainer ) return null;

            return null;
        }
    }
}