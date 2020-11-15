using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWithList : IParseTreeNode
    {
        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IdentifierToken> Arguments
        {
            get;
            set;
        } = new List<IdentifierToken>();

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
            if (token.Kind != IdentifierKind.Word) return null;

            CommandWithList deklaration = new CommandWithList();
            deklaration.Token = token;

            token = parser.Peek(token, 1);
            if (token.Kind != IdentifierKind.BeginContainer) return null;
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

            if (token.Kind != IdentifierKind.CloseContainer) return null;
            deklaration.SupportTokens.Add(token);

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(CommandWithList node)
        {
            node.Token.Node = node;
            foreach (IdentifierToken token in node.Arguments)
            {
                token.Node = node;
            }

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}