using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public class ParserError : IParseTreeNode
    {

        #region get/set

        public SyntaxToken Token
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

        public IParseTreeNode Parse ( Parser parser, SyntaxToken token )
        {
            ParserError result = new ParserError ();

            result.Token = token;

            token.Node = result;

            return result;
        }
    }
}