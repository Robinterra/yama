using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class PointerNode : IParseTreeNode, IContainer
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
            if (token.Kind != IdentifierKind.StarToken) return null;

            PointerNode deklaration = new PointerNode();

            deklaration.SupportTokens.Add(token);

            token = parser.Peek(token, 1);

            if (token.Kind != IdentifierKind.Word) return null;

            deklaration.Token = token;

            deklaration.Ende = token;

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(PointerNode node)
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