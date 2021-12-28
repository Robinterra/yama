using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith2ArgsNode : IParseTreeNode
    {

        #region vars

        private ParserLayer argumentLayer;

        #endregion vars

        #region get/set

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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

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

        #endregion get/set

        #region ctor

        public CommandWith2ArgsNode(ParserLayer argumentLayer)
        {
            this.Argument0 = new WordNode();
            this.Argument1 = new WordNode();
            this.AllTokens = new List<IdentifierToken> ();
            this.Token = new();
            this.argumentLayer = argumentLayer;
        }

        #endregion ctor

        public bool Compile(Parser.Request.RequestParserTreeCompile request)
        {
            return true;
        }

        public bool Indezieren(Parser.Request.RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode? Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;

            CommandWith2ArgsNode deklaration = new CommandWith2ArgsNode(this.argumentLayer);
            deklaration.Token = request.Token;
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token is null) return null;

            request.Parser.ActivateLayer(this.argumentLayer);

            IParseTreeNode? arg0 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (arg0 is null) return null;

            deklaration.Argument0 = arg0;
            if (!(deklaration.Argument0 is IContainer ic)) return null;
            token = ic.Ende;

            token = request.Parser.Peek(token, 1);
            if (token == null) return null;
            if (token.Kind != IdentifierKind.Comma) return null;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token is null) return null;

            request.Parser.ActivateLayer(this.argumentLayer);

            IParseTreeNode? arg1 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (arg1 is null) return null;

            deklaration.Argument1 = arg1;

            return deklaration;
        }

    }
}