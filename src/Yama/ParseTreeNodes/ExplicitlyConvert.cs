using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System.Linq;

namespace Yama.Parser
{
    public class ExplicitlyConvert : IParseTreeNode, IIndexNode, ICompileNode, IParentNode, IContainer
    {

        #region get/set

        public IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        public IdentifierToken? RightToken
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public int Prio
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.LeftNode is not null) result.Add ( this.LeftNode );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Ende
        {
            get;
            private set;
        }

        #endregion get/set

        #region ctor

        public ExplicitlyConvert ( int prio )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
            this.Ende = this.Token;
        }

        #endregion ctor

        #region methods

        private bool CheckHashValidTypeDefinition ( IdentifierToken token )
        {
            if (token.Kind == IdentifierKind.Word) return true;

            return false;
        }

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.As ) return null;

            ExplicitlyConvert node = new ExplicitlyConvert ( this.Prio );
            node.Token = request.Token;
            node.AllTokens.Add(request.Token);

            node.RightToken = request.Parser.Peek ( request.Token, 1 );
            if (node.RightToken is null) return new ParserError(request.Token, $"Expectet a word after the as keyword");
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return new ParserError(request.Token, $"Expectet a word after the as keyword and not a '{node.RightToken.Text}'", node.RightToken);

            node.AllTokens.Add(node.RightToken);
            node.Ende = node.RightToken;

            return node;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is not IIndexNode leftNode) return request.Index.CreateError(this);
            if (this.RightToken is null) return request.Index.CreateError(this);

            leftNode.Indezieren(request);

            IndexVariabelnReference type = new IndexVariabelnReference (this, this.RightToken.Text);
            container.VariabelnReferences.Add(type);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.LeftNode is not ICompileNode leftNode) return false;

            return leftNode.Compile(request);
        }

        #endregion methods
    }
}