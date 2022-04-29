using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ElseKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
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
            if (statementchild is null) return new ParserError(request.Token, $"Can not find a Statement after a else");

            key.Statement = request.Parser.ParseCleanToken(statementchild);
            if (key.Statement == null) return new ParserError(statementchild, $"else statement can not be parse", request.Token);

            return key;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is null) return request.Index.CreateError(this);
            if (this.Statement is not IIndexNode indexNode) return true;

            indexNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is not ICompileNode compileNode) return true;

            compileNode.Compile(request);

            return true;
        }

        #endregion methods

    }
}