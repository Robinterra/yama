using System.Collections.Generic;

namespace LearnCsStuf.Basic
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