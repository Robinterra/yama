using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class SquareArgumentNode : IParseTreeNode, IContainer
    {
        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Number
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

        public bool Compile(Compiler.Compiler compiler, string mode = "default")
        {
            return true;
        }

        public bool Indezieren(Index.Index index, IParent parent)
        {
            return true;
        }

        public IParseTreeNode Parse(Parser.Parser parser, IdentifierToken token)
        {
            if (token.Kind != IdentifierKind.OpenSquareBracket) return null;

            SquareArgumentNode deklaration = new SquareArgumentNode();
            deklaration.SupportTokens.Add(token);

            token = parser.Peek(token, 1);

            if (token.Kind != IdentifierKind.Word) return null;

            deklaration.Token = token;

            token = parser.Peek(token, 1);

            if (token.Kind == IdentifierKind.Comma)
            {
                deklaration.SupportTokens.Add(token);

                token = parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.Hash) return null;

                deklaration.SupportTokens.Add(token);
                token = parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.NumberToken) return null;
                deklaration.Number = token;

                token = parser.Peek(token, 1);
            }

            if (token.Kind != IdentifierKind.CloseSquareBracket) return null;

            deklaration.SupportTokens.Add(token);
            deklaration.Ende = token;

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(SquareArgumentNode node)
        {
            node.Token.Node = node;
            if (node.Number != null) node.Number.Node = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}