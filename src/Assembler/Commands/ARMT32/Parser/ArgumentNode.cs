using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;
using Yama.Parser.Request;

namespace Yama.Assembler.ARMT32
{
    public class ArgumentNode : IParseTreeNode, IContainer
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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ArgumentNode ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion

        public bool Compile(RequestParserTreeCompile request)
        {
            return true;
        }

        public bool Indezieren(RequestParserTreeIndezieren request)
        {
            return true;
        }

        public IParseTreeNode? Parse(RequestParserTreeParser request)
        {
            ArgumentNode deklaration = new ArgumentNode();

            if (request.Token.Kind == IdentifierKind.Word)
            {
                deklaration.Token = request.Token;
                deklaration.AllTokens.Add(request.Token);

                deklaration.ende = request.Token;

                return deklaration;
            }

            if (request.Token.Kind == IdentifierKind.Gleich || request.Token.Kind == IdentifierKind.Hash)
            {
                deklaration.AllTokens.Add(request.Token);
                IdentifierToken? token = request.Parser.Peek(request.Token, 1);
                if (token is null) return null;
                if (token.Kind != IdentifierKind.NumberToken && token.Kind != IdentifierKind.Word) return null;

                deklaration.Token = token;
                deklaration.AllTokens.Add(token);

                deklaration.ende = token;

                return deklaration;
            }

            return null;
        }
    }
}