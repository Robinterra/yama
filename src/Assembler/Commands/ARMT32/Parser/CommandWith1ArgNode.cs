using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith1ArgNode : IParseTreeNode
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        public List<IParseTreeNode> GetAllChilds
        {
            get
            {
                List<IParseTreeNode> nodes = new List<IParseTreeNode>();

                if (this.Argument0 is not null) nodes.Add(this.Argument0);

                return nodes;
            }
        }

        #endregion get/set

        #region ctor

        public CommandWith1ArgNode(ParserLayer argumentLayer)
        {
            this.Argument0 = new WordNode();
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
            this.argumentLayer = argumentLayer;
        }

        #endregion ctor

        public IParseTreeNode? Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;

            CommandWith1ArgNode deklaration = new CommandWith1ArgNode(this.argumentLayer);
            deklaration.Token = request.Token;
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token is null) return null;

            request.Parser.ActivateLayer(this.argumentLayer);

            IParseTreeNode? arg0 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (arg0 is null) return null;

            deklaration.Argument0 = arg0;

            return deklaration;
        }

    }
}