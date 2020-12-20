using System;
using System.Collections.Generic;
using System.Text;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Debug
{
    public class WindowsFormsMapper
    {

        #region vars

        private static WindowsFormsMapper instance;

        #endregion vars

        #region get/set

        public static WindowsFormsMapper Instance
        {
            get
            {
                if (WindowsFormsMapper.instance != null) return WindowsFormsMapper.instance;

                WindowsFormsMapper.instance = new WindowsFormsMapper();

                return WindowsFormsMapper.instance;
            }
        }

        #endregion get/set

        #region methods

        public bool Execute (Runtime runtime)
        {
            return true;
        }

        #endregion methods

    }
}