using System.Collections.Generic;

namespace Yama.CommandLines
{
    public interface ICommandParent
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        List<ICommandLine> Childs
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        bool Execute(RequestExecuteArgs request);

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --