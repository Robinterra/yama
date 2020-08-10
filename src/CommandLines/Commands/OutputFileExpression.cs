namespace LearnCsStuf.CommandLines.Commands
{
    public class OutputFileExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "output";
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
                return string.Format (CommandLines.Help.HilfePattern, this.Key, "<file>", "The output Filename (Default:out.S)" );
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
            if (string.Format ( "{0}", this.Key ) == command) return new OutputFileExpression (  );
            if (string.Format ( "out", this.Key ) == command) return new OutputFileExpression (  );

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --