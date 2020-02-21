using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class ReturnKey : IParseTreeNode
    {

        #region get/set

        public IParseTreeNode Statement
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
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                result.Add ( this.Statement );

                return result;
            }
        }

        #endregion get/set

        #region methods

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            if ( token.Kind != SyntaxKind.Return ) return null;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens(token.Position + 1, parser.Max);
            IParseTreeNode node = null;

            if ( nodes == null ) return null;
            if ( nodes.Count > 1 ) return null;
            if ( nodes.Count == 1 ) node = nodes[0];

            ReturnKey result = new ReturnKey();

            result.Statement = node;
            result.Token = token;
            token.Node = result;
            if ( node != null ) node.Token.ParentNode = result;

            return result;
        }

        #endregion methods

    }
}