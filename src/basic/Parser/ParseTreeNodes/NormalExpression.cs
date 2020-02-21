using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public class NormalExpression : IParseTreeNode
    {

        #region get/set

        public SyntaxToken Token
        {
            get;
            set;
        }

        public IParseTreeNode ExpressionParent
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode> { this.ExpressionParent };
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            SyntaxToken kind = parser.FindAToken ( token, SyntaxKind.EndOfCommand );

            if ( kind == null ) return null;
            if ( kind.Node != null ) return null;

            NormalExpression expression = new NormalExpression (  );

            expression.Token = kind;

            kind.Node = expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position, kind.Position );
            IParseTreeNode node = null;

            if ( nodes == null ) return null;
            if ( nodes.Count > 1 ) return null;
            if ( nodes.Count == 1 ) node = nodes[0];

            expression.ExpressionParent = node;

            if ( node != null ) node.Token.ParentNode = expression;

            return expression;
        }
    }
}