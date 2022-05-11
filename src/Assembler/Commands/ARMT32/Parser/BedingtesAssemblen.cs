using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler.ARMT32
{
    public class BedingtesAssemblen : IParseTreeNode
    {

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

        public List<IdentifierToken> AllTokens
        {
            get;
        }

        #endregion get/set

        #region ctor

        public BedingtesAssemblen ()
        {
            this.Token = new();
            this.AllTokens = new List<IdentifierToken> ();
        }

        #endregion ctor

        #region methods

        public IParseTreeNode? Parse(Parser.Request.RequestParserTreeParser request)
        {
            if (request.Token.Kind != IdentifierKind.Point) return null;

            return null;
        }

        #endregion methods

    }
}