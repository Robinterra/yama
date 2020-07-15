using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public interface IParseTreeNode
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        SyntaxToken Token
        {
            get;
            set;
        }

        List<IParseTreeNode> GetAllChilds
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        IParseTreeNode Parse ( Parser parser, SyntaxToken token );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --