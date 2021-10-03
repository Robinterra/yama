using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class SquareArgumentNode : IParseTreeNode, IContainer
    {
        public IdentifierToken Token
        {
            get;
            set;
        }

        public IdentifierToken Number
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
            get;
            set;
        }

        public SquareArgumentNode ()
        {
            this.AllTokens = new List<IdentifierToken> ();
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
            if (request.Token.Kind != IdentifierKind.OpenSquareBracket) return null;

            SquareArgumentNode deklaration = new SquareArgumentNode();
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);

            if (token.Kind != IdentifierKind.Word) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            token = request.Parser.Peek(token, 1);

            if (token.Kind == IdentifierKind.Comma)
            {
                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.Hash) return null;

                deklaration.AllTokens.Add(token);
                token = request.Parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.NumberToken) return null;
                deklaration.Number = token;
                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);
            }

            if (token.Kind != IdentifierKind.CloseSquareBracket) return null;

            deklaration.AllTokens.Add(token);
            deklaration.Ende = token;

            return deklaration;
        }

    }
}