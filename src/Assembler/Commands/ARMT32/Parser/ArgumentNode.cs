using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.Assembler.ARMT32
{
    public class ArgumentNode : IParseTreeNode, IContainer
    {
        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IdentifierToken> SupportTokens
        {
            get;
            set;
        } = new List<IdentifierToken>();

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                return new List<IParseTreeNode>();
            }
        }

        public IdentifierToken Ende
        {
            get;
            set;
        }

        public bool Compile(RequestParserTreeCompile request)
        {
            return true;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode Parse(RequestParserTreeParser request)
        {
            ArgumentNode deklaration = new ArgumentNode();

            if (request.Token.Kind == IdentifierKind.Word)
            {
                deklaration.Token = request.Token;

                deklaration.Ende = request.Token;

                return this.CleanUp(deklaration);
            }

            if (request.Token.Kind == IdentifierKind.Gleich || request.Token.Kind == IdentifierKind.Hash)
            {
                deklaration.SupportTokens.Add(request.Token);
                IdentifierToken token = request.Parser.Peek(request.Token, 1);

                if (token == null) return null;
                if (token.Kind != IdentifierKind.NumberToken && token.Kind != IdentifierKind.Word) return null;
                deklaration.Token = token;

                deklaration.Ende = token;

                return this.CleanUp(deklaration);
            }

            return null;
        }

        private IParseTreeNode CleanUp(ArgumentNode node)
        {
            node.Token.Node = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}