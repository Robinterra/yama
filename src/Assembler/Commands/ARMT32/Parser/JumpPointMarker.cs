using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class JumpPointMarker : IParseTreeNode
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

            JumpPointMarker deklaration = new JumpPointMarker();
            deklaration.Token = token;

            token = parser.Peek(token, 1);
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            deklaration.SupportTokens.Add(token);

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(JumpPointMarker node)
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