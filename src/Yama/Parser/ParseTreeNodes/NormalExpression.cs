using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NormalExpression : IParseTreeNode, IEndExpression
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
            //SyntaxToken kind = parser.FindAToken ( token, SyntaxKind.EndOfCommand );
            
            //if ( kind == null ) return null;
            //if ( kind.Node != null ) return null;
            if ( request.Token.Kind != IdentifierKind.EndOfCommand ) return null;

            IdentifierToken left = request.Parser.Peek ( request.Token, -1 );

            NormalExpression expression = new NormalExpression (  );

            expression.Token = request.Token;

            request.Token.Node = expression;

            if (left == null) return expression;

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens ( request.Parser.Start, request.Token.Position );

            IParseTreeNode node = null;

            if ( nodes == null ) return null;
            //if ( nodes.Count > 1 ) return null;
//            if ( nodes.Count == 1 ) node = nodes[0];
            node = nodes[nodes.Count - 1];

            expression.ExpressionParent = node;

            if ( node != null ) node.Token.ParentNode = expression;

            return expression;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            //if (!(parent is IndexContainer container)) return index.CreateError(this);

            if (this.ExpressionParent == null) return true;

            return this.ExpressionParent.Indezieren(request);
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            this.ExpressionParent.Compile(request);

            return true;
        }
    }
}