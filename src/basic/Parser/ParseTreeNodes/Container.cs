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

        public SyntaxToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return this.Statements;
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.BeginContainer ) return null;

            return null;
        }
    }
}