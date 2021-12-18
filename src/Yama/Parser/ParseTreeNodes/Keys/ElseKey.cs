using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ElseKey : IParseTreeNode, IContainer
    {

        #region get/set

        public IParseTreeNode? Statement
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.Statement is null) return this.Token;

                return (this.Statement is IContainer t) ? t.Ende : this.Statement.Token;
            }
            set
            {

            }
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ElseKey ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Else ) return null;

            ElseKey key = new ElseKey (  );
            key.Token = request.Token;
            key.AllTokens.Add(request.Token);

            IdentifierToken? statementchild = request.Parser.Peek ( request.Token, 1);
            if (statementchild is null) return null;

            key.Statement = request.Parser.ParseCleanToken(statementchild);
            if (key.Statement == null) return null;

            return key;
        }

        public bool Indezieren(Request.RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is null) return request.Index.CreateError(this);

            this.Statement.Indezieren(request);

            return true;
        }

        public bool Compile(Request.RequestParserTreeCompile request)
        {
            if (this.Statement is null) return false;

            this.Statement.Compile(request);

            return true;
        }

        #endregion methods

    }
}