using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith1ArgNode : IParseTreeNode
    {
        private ParserLayer argumentLayer;

        public CommandWith1ArgNode()
        {
        }
        public CommandWith1ArgNode(ParserLayer argumentLayer)
        {
            this.argumentLayer = argumentLayer;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IParseTreeNode Argument0
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
                List<IParseTreeNode> nodes = new List<IParseTreeNode>();

                nodes.Add(this.Argument0);

                return nodes;
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

            CommandWith1ArgNode deklaration = new CommandWith1ArgNode();
            deklaration.Token = token;

            token = parser.Peek(token, 1);

            parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument0 = parser.ParseCleanToken(token);

            parser.VorherigesLayer();
            if (deklaration.Argument0 == null) return null;

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(CommandWith1ArgNode node)
        {
            node.Token.Node = node;
            node.Argument0.Token.ParentNode = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}