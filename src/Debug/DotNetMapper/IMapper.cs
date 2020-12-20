using System;
using System.Collections.Generic;
using System.Text;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public interface IMapper
    {

        #region vars

        #endregion vars

        #region get/set

        uint Id
        {
            get;
        }

        #endregion get/set

        #region methods

        bool Execute (Runtime runtime);

        #endregion methods

    }
}