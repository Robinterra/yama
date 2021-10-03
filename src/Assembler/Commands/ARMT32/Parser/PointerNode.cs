using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class PointerNode : IParseTreeNode, IContainer
    {

        #region get/set

        public IdentifierToken Token
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

        #endregion get/set

        #region ctor

        public PointerNode ()
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
            if (request.Token.Kind != IdentifierKind.StarToken) return null;

            PointerNode deklaration = new PointerNode();

            deklaration.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);

            if (token.Kind != IdentifierKind.Word) return null;

            deklaration.Token = token;
            deklaration.AllTokens.Add(token);

            deklaration.Ende = token;

            return deklaration;
        }

    }
}