using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class EnumartionExpression : IParseTreeNode, IEndExpression
    {

        #region get/set

        public IdentifierToken Token
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

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Comma ) return null;

            IdentifierToken left = request.Parser.Peek ( request.Token, -1 );

            EnumartionExpression expression = new EnumartionExpression (  );

            expression.Token = request.Token;
            expression.Token.Node = expression;

            if (left == null) return expression;

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens (left, request.Parser.Start, request.Token.Position );

            IParseTreeNode node = null;

            if ( nodes == null ) return expression.Token.Node = null;
            if ( nodes.Count > 1 ) return expression.Token.Node = null;
            if ( nodes.Count == 1 ) node = nodes[0];

            expression.ExpressionParent = node;

            if ( node != null ) node.Token.ParentNode = expression;

            return expression;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (this.ExpressionParent == null) return true;

            return this.ExpressionParent.Indezieren(request);
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            return true;
        }
    }
}