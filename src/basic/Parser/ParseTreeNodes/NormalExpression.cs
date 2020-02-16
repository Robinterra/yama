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
            SyntaxToken kind = token;

            for ( int i = 1; kind.Kind != SyntaxKind.EndOfCommand; i++ )
            {
                kind = parser.Peek ( token, i );

                if ( kind == null ) return null;
            }

            if ( kind.Node != null ) return null;

            NormalExpression expression = new NormalExpression (  );

            expression.Token = kind;

            kind.Node = expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens ( token.Position, kind.Position );

            if ( nodes.Count > 1 ) return null;

            expression.ExpressionParent = nodes[0];

            nodes[0].Token.ParentNode = expression;

            return expression;
        }
    }
}