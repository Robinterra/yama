using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NormalStatementNode : IParseTreeNode, IIndexNode, ICompileNode, IEndExpression
    {
        private ParserLayer normalStatementLayer;

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IParseTreeNode? Child
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                if (this.Child is null) return new ();

                return new List<IParseTreeNode> { this.Child };
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public NormalStatementNode (ParserLayer normalStatementLayer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.normalStatementLayer = normalStatementLayer;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            NormalStatementNode expression = new NormalStatementNode ( this.normalStatementLayer );

            expression.Token = request.Token;

            IParseTreeNode? node = request.Parser.ParseCleanToken(request.Token, this.normalStatementLayer);
            if (node is null) return null;

            expression.Child = node;

            return expression;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            //if (!(parent is IndexContainer container)) return index.CreateError(this);

            if (this.Child is not IIndexNode expressionNode) return true;

            return expressionNode.Indezieren(request);
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Child is not ICompileNode expressionParent) return false;

            expressionParent.Compile(request);

            return true;
        }

        #endregion methods

    }
}