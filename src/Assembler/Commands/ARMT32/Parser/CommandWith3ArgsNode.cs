using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWith3ArgsNode : IParseTreeNode
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

        public IParseTreeNode Argument2
        {
            get;
            set;
        }

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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public CommandWith3ArgsNode()
        {
            this.AllTokens = new List<IdentifierToken> ();
        }

        public CommandWith3ArgsNode(ParserLayer argumentLayer)
        {
            this.argumentLayer = argumentLayer;
        }

        #endregion


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
            deklaration.AllTokens.Add(request.Token);

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
            deklaration.AllTokens.Add(token);

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
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);

            request.Parser.ActivateLayer(this.argumentLayer);

            deklaration.Argument2 = request.Parser.ParseCleanToken(token);

            request.Parser.VorherigesLayer();
            if (!(deklaration.Argument2 is IContainer ic)) return null;

            return deklaration;
        }
    }
}