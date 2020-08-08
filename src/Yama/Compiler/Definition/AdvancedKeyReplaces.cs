using System.Collections.Generic;

namespace Yama.Compiler.Definition
{

    public class AdvancedKeyReplaces
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get;
            set;
        }

        // -----------------------------------------------

        /**
         *
         */
        public string Value
        {
            get;
            set;
        }

        // -----------------------------------------------
        
        /**
         * Defines for conditional compilation
         * if a define is contains in this list then this key can be used
         */
        public List<string> Defines
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

    }
}