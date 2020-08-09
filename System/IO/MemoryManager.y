namespace "System.IO"
{
    using "System";

    public static class MemoryManager
    {

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        /**
         * Dynamic memory allocate
         *
         * @param[in] size (Int) The size to reserved
         *
         * @return (Int) The Adress (If this is null then is failed and not engough space exist)
         */
        public static Int Malloc ( Int size )
        {
            return #defalgo Malloc,default: = size;
        }

        // -----------------------------------------------

        /**
         * Dynamic memory allocate Free
         *
         * @param[in] Adress (Int) The adress to free the block
         *
         * @return (Bool) true is Ok and false is not ok
         */
        public static Bool Free ( Int addresse )
        {
            #defalgo MallocFree,default: = addresse;

            return MemoryManager.Clean ( addresse );
        }

        // -----------------------------------------------

        /**
         * Clean Memory Blocks
         */
        private static Bool Clean ( Int addresse )
        {
            #defalgo Malloc,Init:

            Int currentadress = #defalgo Malloc,CurrentAdress:;

            while ( currentadress < addresse )
            {
                #defalgo MallocClean, default: = currentadress;
            }

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}