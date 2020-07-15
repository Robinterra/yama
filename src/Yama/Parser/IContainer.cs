using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public interface IContainer
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        SyntaxToken Token
        {
            get;
            set;
        }

        SyntaxToken Ende
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