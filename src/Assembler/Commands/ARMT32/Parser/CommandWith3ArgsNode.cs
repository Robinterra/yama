using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith3ArgsNode : IParseTreeNode
    {
        private ParserLayer argumentLayer;

        public CommandWith3ArgsNode()
        {
        }
        public CommandWith3ArgsNode(ParserLayer argumentLayer)
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

        public IParseTreeNode Argument2
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
                nodes.Add(this.Argument2);

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

            CommandWith3ArgsNode deklaration = new CommandWith3ArgsNode();
            deklaration.Token = request.Token;

            IdentifierToken token = request.Parser.Peek(request.Token, 1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument0 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();

            if (deklaration.Argument0 == null) return null;
            if (!(deklaration.Argument0 is IContainer b)) return null;
            token = b.Ende;

            token = request.Parser.Peek(token ,1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Comma) return null;
            deklaration.SupportTokens.Add(token);

            token = request.Parser.Peek(token ,1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument1 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (deklaration.Argument1 == null) return null;
            if (!(deklaration.Argument1 is IContainer t)) return null;
            token = t.Ende;

            token = request.Parser.Peek(token ,1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Comma) return null;
            deklaration.SupportTokens.Add(token);

            token = request.Parser.Peek(token, 1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument2 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (!(deklaration.Argument2 is IContainer ic)) return null;

            token = ic.Ende;

            return this.CleanUp(deklaration);
        }

        private IParseTreeNode CleanUp(CommandWith3ArgsNode node)
        {
            node.Token.Node = node;
            node.Argument0.Token.ParentNode = node;
            node.Argument1.Token.ParentNode = node;
            node.Argument2.Token.ParentNode = node;

            foreach (IdentifierToken token in node.SupportTokens)
            {
                token.Node = node;
            }

            return node;
        }
    }
}