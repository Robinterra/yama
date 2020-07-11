using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class BedingtesCompilierenParser : IParseTreeNode
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

        #region ctor

        public BedingtesCompilierenParser (  )
        {

        }

        #endregion ctor

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.BedingtesCompilieren ) return null;

            BedingtesCompilierenParser node = new BedingtesCompilierenParser { Token = token };

            token.Node = node;

            return node;
        }
    }
}