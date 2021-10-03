using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class JumpPointMarker : IParseTreeNode
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
        
        #endregion get/set

        #region ctor

        public JumpPointMarker ()
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

            JumpPointMarker deklaration = new JumpPointMarker();
            deklaration.Token = request.Token;
            deklaration.AllTokens.Add(request.Token);

            IdentifierToken token = request.Parser.Peek(request.Token, 1);
            if (token.Kind != IdentifierKind.DoublePoint) return null;
            deklaration.AllTokens.Add(token);

            return deklaration;
        }

    }
}