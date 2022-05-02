using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public interface IParentNode
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        IParseTreeNode? LeftNode
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

    }
}

// -- [EOF] --