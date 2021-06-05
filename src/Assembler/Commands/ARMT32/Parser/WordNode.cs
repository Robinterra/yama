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

        public List<IdentifierToken> SupportTokens
        {
            get;
            set;
        } = new List<IdentifierToken>();

        public IdentifierToken Data
        {
            get;
            set;
        }

        public List<IdentifierToken> Arguments
        {
            get;
            set;
        } = new List<IdentifierToken>();

        public IdentifierToken AdditionNumberToken
        {
            get;
            set;
        }

        #endregion get/set

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
            node.SupportTokens.Add(request.Token);
            node.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token == null) return null;
            if (token.Kind == IdentifierKind.NumberToken) node.Data = token;
            if (token.Kind == IdentifierKind.Word) node.Data = token;

            if (node.Data == null) return null;

            token = request.Parser.Peek(token, 1);
            if (token == null) return this.CleanUp(node);
            if (token.Kind != IdentifierKind.PlusToken) return this.CleanUp(node);

            node.SupportTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token.Kind != IdentifierKind.NumberToken) return this.CleanUp(node);

            node.AdditionNumberToken = token;

            return this.CleanUp(node);
        }

        private IParseTreeNode CleanUp(WordNode node)
        {
            node.Token.Node = node;
            if (node.Data != null) node.Data.Node = node;
            if (node.AdditionNumberToken != null) node.AdditionNumberToken.Node = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            foreach (IdentifierToken token in node.Arguments)
            {
                token.Node = node;
            }

            return node;
        }

        #endregion methods
    }
}