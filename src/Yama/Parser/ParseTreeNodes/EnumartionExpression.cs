using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public class EnumartionExpression : IParseTreeNode, IEndExpression
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

        public int Prio
        {
            get
            {
                return 0;
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            //SyntaxToken kind = parser.FindAToken ( token, SyntaxKind.EndOfCommand );
            
            //if ( kind == null ) return null;
            //if ( kind.Node != null ) return null;
            if ( token.Kind != SyntaxKind.Comma ) return null;

            SyntaxToken left = parser.Peek ( token, -1 );

            NormalExpression expression = new NormalExpression (  );

            expression.Token = token;

            token.Node = expression;

            if (left == null) return expression;

            List<IParseTreeNode> nodes = parser.ParseCleanTokens (left, parser.Start, token.Position );

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