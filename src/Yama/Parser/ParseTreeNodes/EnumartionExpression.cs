using System.Collections.Generic;
using System.Linq;
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public EnumartionExpression ()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Comma ) return null;

            IdentifierToken left = request.Parser.Peek ( request.Token, -1 );

            EnumartionExpression expression = new EnumartionExpression (  );

            expression.Token = request.Token;
            expression.AllTokens.Add(request.Token);

            if (left == null) return expression;

            List<IParseTreeNode> nodes = request.Parser.ParseCleanTokens (left, request.Parser.Start, request.Token.Position );

            if ( nodes == null ) return null;
            if ( nodes.Count > 1 ) return null;

            expression.ExpressionParent = nodes.FirstOrDefault();

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