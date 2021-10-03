using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class WordNode : IParseTreeNode
    {

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
            set;
        }

        public IdentifierToken AdditionNumberToken
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public WordNode ()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public bool Compile(Parser.Request.RequestParserTreeCompile request)
        {
            return true;
        }

        public bool Indezieren(Parser.Request.RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Base) return null;
            if (request.Token.Text != ".word") return null;

            WordNode node = new WordNode();
            node.AllTokens.Add(request.Token);
            node.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token == null) return null;
            if (token.Kind == IdentifierKind.NumberToken) node.Data = token;
            if (token.Kind == IdentifierKind.Word) node.Data = token;

            if (node.Data == null) return null;
            node.AllTokens.Add(node.Data);

            token = request.Parser.Peek(token, 1);
            if (token == null) return node;
            if (token.Kind != IdentifierKind.PlusToken) return node;

            node.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token.Kind != IdentifierKind.NumberToken) return node;

            node.AdditionNumberToken = token;
            node.AllTokens.Add(token);

            return node;
        }

        #endregion methods
    }
}