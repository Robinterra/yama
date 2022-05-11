using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class SquareArgumentNode : IParseTreeNode, IContainer
    {

        #region vars

        private IdentifierToken? ende;

        #endregion vars

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken? Number
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
                return new List<IParseTreeNode>();
            }
        }

        public IdentifierToken Ende
        {
            get
            {
                if (this.ende is null) return this.Token;

                return this.ende;
            }
        }

        #endregion get/set

        #region ctor

        public SquareArgumentNode ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.OpenSquareBracket) return null;

            SquareArgumentNode deklaration = new SquareArgumentNode();
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken? token = request.Parser.Peek(request.Token, 1);
            if (token is null) return null;
            if (token.Kind != IdentifierKind.Word) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);
            if (token is null) return null;

            if (token.Kind == IdentifierKind.Comma)
            {
                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);
                if (token is null) return null;
                if (token.Kind != IdentifierKind.Hash) return null;

                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);
                if (token is null) return null;
                if (token.Kind != IdentifierKind.NumberToken) return null;

                deklaration.Number = token;
                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);
                if (token is null) return null;
            }

            if (token.Kind != IdentifierKind.CloseSquareBracket) return null;

            deklaration.AllTokens.Add(token);
            deklaration.ende = token;

            return deklaration;
        }

        #endregion methods

    }
}