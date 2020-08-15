using System.Collections.Generic;
using Yama.Lexer;

namespace Yama.Parser
{
    public interface IContainer
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        IdentifierToken Token
        {
            get;
            set;
        }

        IdentifierToken Ende
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