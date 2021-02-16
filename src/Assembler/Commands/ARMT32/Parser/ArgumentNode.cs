using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

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
            ArgumentNode deklaration = new ArgumentNode();

            if (token.Kind == IdentifierKind.Word)
            {
                deklaration.Token = token;

                deklaration.Ende = token;

                return this.CleanUp(deklaration);
            }

            if (token.Kind == IdentifierKind.Gleich || token.Kind == IdentifierKind.Hash)
            {
                deklaration.SupportTokens.Add(token);
                token = parser.Peek(token, 1);

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