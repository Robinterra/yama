using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class NormalStatementNode : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {
        private ParserLayer identifierStatementLayer;
        private ParserLayer statementLayer;

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                if (this.StatementChild is null) return new ();

                return new List<IParseTreeNode> { this.StatementChild };
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        public IParseTreeNode? StatementChild
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public NormalStatementNode (ParserLayer identifierStatementLayer, ParserLayer statementLayer)
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.identifierStatementLayer = identifierStatementLayer;
            this.statementLayer = statementLayer;
            this.Ende = new IdentifierToken();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            NormalStatementNode expression = new NormalStatementNode ( this.identifierStatementLayer, this.statementLayer );

            expression.Token = request.Token;

            IParseTreeNode? node = request.Parser.ParseCleanToken(request.Token, this.identifierStatementLayer);
            if (node is null) return null;

            IdentifierToken? ende = request.Token;
            if (node is IContainer container) ende = container.Ende;

            ende = request.Parser.Peek(ende, 1);
            if (ende is null) return null;

            IParseTreeNode? statementnode = request.Parser.ParseCleanToken(ende, this.statementLayer);
            if (statementnode is not IParentNode parentNode) return null;
            parentNode.LeftNode = node;

            if (statementnode is IContainer statementContainer) ende = statementContainer.Ende;
            expression.Ende = ende;

            expression.StatementChild = statementnode;

            return expression;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            //if (!(parent is IndexContainer container)) return index.CreateError(this);

            if (this.StatementChild is not IIndexNode statementChild) return true;

            return statementChild.Indezieren(request);

        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.StatementChild is not ICompileNode statementChild) return false;

            return statementChild.Compile(request);
        }

        #endregion methods

    }
}