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

        public bool Compile(Parser.Request.RequestParserTreeCompile request)
        {
            return true;
        }

        public bool Indezieren(Parser.Request.RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;

            CommandWith1ArgNode deklaration = new CommandWith1ArgNode();
            deklaration.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(request.Token, 1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument0 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
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