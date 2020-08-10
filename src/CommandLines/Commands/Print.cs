namespace LearnCsStuf.CommandLines.Commands
{
    public class Print : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "print";
            }
        }

        // -----------------------------------------------

        public bool HasValue
        {
            get
            {
                return true;
            }
        }

        // -----------------------------------------------

        public string HelpLine
        {
            get
            {
                return string.Format (CommandLines.Help.HilfePattern, this.Key, "<text>", "Gibt <text> in der Console aus" );
            }
        }

        // -----------------------------------------------

        public string Value
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public ICommandLine Check ( string command )
        {
            if (string.Format ( "{0}", this.Key ) != command) return null;

            return new Print();
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --