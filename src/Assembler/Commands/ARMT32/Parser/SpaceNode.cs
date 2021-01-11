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

        public List<IdentifierToken> SupportTokens
        {
            get;
            set;
        } = new List<IdentifierToken>();
        public IdentifierToken Data { get; private set; }
        public List<IdentifierToken> Arguments { get; set; } = new List<IdentifierToken>();

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
            if (token.Kind != IdentifierKind.Base) return null;
            if (token.Text != ".space") return null;

            SpaceNode node = new SpaceNode();
            node.SupportTokens.Add(token);
            node.Token = token;

            token = parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.NumberToken) return null;

            node.Data = token;

            return this.CleanUp(node);
        }

        private IParseTreeNode CleanUp(SpaceNode node)
        {
            node.Token.Node = node;
            if (node.Data != null) node.Data.Node = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}