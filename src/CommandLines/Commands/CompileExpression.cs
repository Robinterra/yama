namespace LearnCsStuf.CommandLines.Commands
{
    public class CompileExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "build";
            }
        }

        // -----------------------------------------------

        public bool HasValue
        {
            get
            {
                return false;
            }
        }

        // -----------------------------------------------

        public string HelpLine
        {
            get
            {
                return string.Format (CommandLines.Help.HilfePattern, this.Key, string.Empty, "Build a Yama Programm" );
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
            if (this.Key != command) return null;

            return new CompileExpression (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --