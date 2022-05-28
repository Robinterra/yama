namespace Yama.CommandLines.Commands
{
    public class IROutputExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "irout";
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, "<file>", "The output file of the IR Code" );
            }
        }

        // -----------------------------------------------

        public string? Value
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public ICommandLine? Check ( string command )
        {
            if (string.Format ( "{0}", this.Key ) == command) return new IROutputExpression (  );

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --