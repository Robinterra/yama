using System.Collections.Generic;
using Yama.Compiler;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ReturnKey : IParseTreeNode, IIndexNode, ICompileNode, IContainer
    {
        private ParserLayer expressionLayer;

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

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> result = new List<IParseTreeNode> (  );

                if (this.Statement != null) result.Add ( this.Statement );

                return result;
            }
        }

        public CompileJumpTo JumpTo
        {
            get;
            set;
        } = new CompileJumpTo() { Point = PointMode.RootEnde };

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

        public ReturnKey (ParserLayer expressionLayer)
        {
            this.expressionLayer = expressionLayer;
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.Ende = this.Token;
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse ( Request.RequestParserTreeParser request )
        {
            if ( request.Token.Kind != IdentifierKind.Return ) return null;

            ReturnKey result = new ReturnKey(this.expressionLayer);
            result.Token = request.Token;
            result.AllTokens.Add ( request.Token );

            //IdentifierToken? ende = request.Parser.FindAToken(request.Token, IdentifierKind.EndOfCommand);
            //if (ende is null) return new ParserError(request.Token, "Expectet a ';' after the return statement");

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token is null) return null;

            IParseTreeNode? node = request.Parser.ParseCleanToken(token, this.expressionLayer, true);
            result.Statement = node;
            if (node is not IContainer con) return null;

            IdentifierToken? semikolon = request.Parser.Peek(con.Ende, 1);
            if (semikolon is null) return null;
            if (semikolon.Kind != IdentifierKind.EndOfCommand) return null;
            result.AllTokens.Add(semikolon);
            result.Ende = semikolon;

            return result;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            if (request.Parent is not IndexContainer container) return request.Index.CreateError(this);
            if (this.Statement is not IIndexNode indexNode) return request.Index.CreateError(this);

            indexNode.Indezieren(request);

            return true;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            if (this.Statement is not ICompileNode compileNode) return false;

            compileNode.Compile(request);

            if (this.Statement is ReferenceCall)
            {
                CompileMovReg movReg = new CompileMovReg();
                movReg.Compile(request.Compiler, this);
            }

            this.JumpTo.Compile(request.Compiler, null, request.Mode);

            return true;
        }

        #endregion methods

    }
}