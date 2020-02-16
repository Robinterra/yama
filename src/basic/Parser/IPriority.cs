using System.Collections.Generic;

namespace LearnCsStuf.Basic
{
    public interface IPriority
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        int Prio
        {
            get;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        IParseTreeNode SwapChild ( IPriority node );

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}

// -- [EOF] --