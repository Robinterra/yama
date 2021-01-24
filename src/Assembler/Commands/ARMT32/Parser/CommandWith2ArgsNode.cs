using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith2ArgsNode : IParseTreeNode
    {
        private ParserLayer argumentLayer;

        public CommandWith2ArgsNode()
        {

        }
        public CommandWith2ArgsNode(ParserLayer argumentLayer)
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

        public IParseTreeNode Argument1
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
                nodes.Add(this.Argument1);

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

            CommandWith2ArgsNode deklaration = new CommandWith2ArgsNode();
            deklaration.Token = token;

            token = parser.Peek(token ,1);

            parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument0 = parser.ParseCleanToken(token);

            parser.VorherigesLayer();
            if (deklaration.Argument0 == null) return null;
            if (!(deklaration.Argument0 is IContainer ic)) return null;
            token = ic.Ende;

            token = parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Comma) return null;
            deklaration.SupportTokens.Add(token);

            token = parser.Peek(token, 1);

            parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument1 = parser.ParseCleanToken(token);

            parser.VorherigesLayer();
            if (deklaration.Argument1 == null) return null;

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(CommandWith2ArgsNode node)
        {
            node.Token.Node = node;
            node.Argument0.Token.ParentNode = node;
            node.Argument1.Token.ParentNode = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}