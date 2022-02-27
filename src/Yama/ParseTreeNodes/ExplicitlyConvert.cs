using System.Collections.Generic;
using Yama.Lexer;
using Yama.Index;
using Yama.Compiler;
using System.Linq;

namespace Yama.Parser
{
    public class ExplicitlyConvert : IParseTreeNode
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

        #endregion get/set

        #region ctor

        public ExplicitlyConvert ( int prio )
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Prio = prio;
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

            IdentifierToken? token = request.Parser.Peek ( request.Token, -1 );
            if (token is null) return null;

            node.LeftNode = request.Parser.ParseCleanToken ( token );

            node.RightToken = request.Parser.Peek ( request.Token, 1 );
            if (node.RightToken is null) return null;
            if ( !this.CheckHashValidTypeDefinition ( node.RightToken ) ) return null;

            node.AllTokens.Add(node.RightToken);

            return node;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.LeftNode is null) return request.Index.CreateError(this);
            if (this.RightToken is null) return request.Index.CreateError(this);

            this.LeftNode.Indezieren(request);

            IndexVariabelnReference type = new IndexVariabelnReference (this, this.RightToken.Text);
            container.VariabelnReferences.Add(type);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.LeftNode is null) return false;

            return this.LeftNode.Compile(request);
        }

        #endregion methods
    }
}