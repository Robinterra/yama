using System.Collections.Generic;
using Yama.Index;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ParserError : IParseTreeNode
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
                return null;
            }
        }

        #endregion get/set

        public IParseTreeNode Parse ( Request.RequestParserTreeParser request )
        {
            ParserError result = new ParserError ();

            result.Token = request.Token;

            result.Token.Node = result;

            return result;
        }

        public bool Indezieren ( Request.RequestParserTreeIndezieren request )
        {
            return request.Index.CreateError(this);
        }

        public bool Compile ( Request.RequestParserTreeCompile request)
        {
            return false;
        }
    }
}