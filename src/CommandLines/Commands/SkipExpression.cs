namespace LearnCsStuf.CommandLines.Commands
{
    public class SkipExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "skip";
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
                return string.Format (CommandLines.Help.HilfePattern, this.Key, "<value>", "The Skip value at top from binary code (Hex Format)" );
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

            return new SkipExpression (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --