using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class SpaceNode : IParseTreeNode
    {
        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode>();
            }
        }

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public IdentifierToken Data
        {
            get;
            private set;
        }

        #region ctor

        public SpaceNode ()
        {
            this.Token = new();
            this.Data = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        public IParseTreeNode? Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Base) return null;
            if (request.Token.Text != ".space") return null;

            SpaceNode node = new SpaceNode();
            node.AllTokens.Add(request.Token);
            node.Token = request.Token;

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.NumberToken) return null;

            node.Data = token;
            node.AllTokens.Add(token);

            return node;
        }

    }
}