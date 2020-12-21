using System;
using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class DataNode : IParseTreeNode
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

            DataNode node = new DataNode();
            node.SupportTokens.Add(token);

            token = parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Word) return null;

            node.Token = token;
            token = parser.Peek(token, 1);

            if (token.Kind != IdentifierKind.Gleich) return null;
            node.SupportTokens.Add(token);

            token = parser.Peek(token, 1);
            if (token.Kind == IdentifierKind.Text) node.Data = token;
            else if (!this.TryParseList(parser, token, node)) return null;

            return this.CleanUp(node);
        }

        private bool TryParseList(Parser.Parser parser, IdentifierToken token, DataNode deklaration)
        {
            if (token.Kind != IdentifierKind.BeginContainer) return false;
            deklaration.SupportTokens.Add(token);
            token = parser.Peek(token, 1);

            while (token.Kind == IdentifierKind.Word)
            {
                deklaration.Arguments.Add(token);

                token = parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.Comma) continue;

                deklaration.SupportTokens.Add(token);

                token = parser.Peek(token, 1);
            }

            if (token.Kind != IdentifierKind.CloseContainer) return false;
            deklaration.SupportTokens.Add(token);

            return true;
        }

        private IParseTreeNode CleanUp(DataNode node)
        {
            node.Token.Node = node;
            if (node.Data != null) node.Data.Node = node;

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
    }
}