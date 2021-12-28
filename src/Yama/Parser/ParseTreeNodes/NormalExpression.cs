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

        public IParseTreeNode? ExpressionParent
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                if (this.ExpressionParent is null) return new ();

                return new List<IParseTreeNode> { this.ExpressionParent };
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public NormalExpression ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.EndOfCommand ) return null;

            IdentifierToken? left = request.Parser.Peek ( request.Token, -1 );

            NormalExpression expression = new NormalExpression (  );

            expression.Token = request.Token;
            expression.AllTokens.Add(request.Token);

            if (left is null) return expression;

            List<IParseTreeNode>? nodes = request.Parser.ParseCleanTokens ( request.Parser.Start, request.Token.Position );
            if (nodes is null) return null;

            IParseTreeNode? node = nodes.LastOrDefault();

            expression.ExpressionParent = node;

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
            if (this.ExpressionParent is null) return false;

            this.ExpressionParent.Compile(request);

            return true;
        }

        #endregion methods

    }
}