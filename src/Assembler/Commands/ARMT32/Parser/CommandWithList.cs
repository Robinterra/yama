using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class CommandWithList : IParseTreeNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
            set;
        }

        public List<IdentifierToken> Arguments
        {
            get;
            set;
        } = new List<IdentifierToken>();

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

        #endregion get/set

        #region ctor

        public CommandWithList ()
        {
            this.AllTokens = new List<IdentifierToken> ();
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

            CommandWithList deklaration = new CommandWithList();
            deklaration.Token = request.Token;
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token.Kind != IdentifierKind.BeginContainer) return null;
            deklaration.AllTokens.Add(token);
            token = request.Parser.Peek(token, 1);

            while (token.Kind == IdentifierKind.Word)
            {
                deklaration.Arguments.Add(token);
                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);

                if (token.Kind != IdentifierKind.Comma) continue;

                deklaration.AllTokens.Add(token);

                token = request.Parser.Peek(token, 1);
            }

            if (token.Kind != IdentifierKind.CloseContainer) return null;
            deklaration.AllTokens.Add(token);

            return deklaration;
        }
    }
}