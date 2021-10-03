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

                nodes.Add(this.Argument0);

                return nodes;
            }
        }

        #endregion get/set

        #region ctor

        public CommandWith1ArgNode()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public CommandWith1ArgNode(ParserLayer argumentLayer)
        {
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

        public IParseTreeNode Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Word) return null;

            CommandWith1ArgNode deklaration = new CommandWith1ArgNode();
            deklaration.Token = request.Token;
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument0 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (deklaration.Argument0 == null) return null;

            return deklaration;
        }

    }
}