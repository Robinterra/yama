using System;
using System.Collections.Generic;
using System.Text;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class GuiMapper : IMapper
    {

        #region vars

        #endregion vars

        #region get/set

        #endregion get/set

        public uint Id
        {
            get;
        } = 7;

        #region methods

        public bool Execute (Runtime runtime)
        {
            return true;
        }

        #endregion methods

    }
}