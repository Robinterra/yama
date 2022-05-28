namespace Yama.CommandLines.Commands
{
    public class AssemblerOutputFileExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "asmoutput";
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, "<file>", "The output Filename (Default:No assembler output)" );
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
            if (string.Format ( "{0}", this.Key ) == command) return new AssemblerOutputFileExpression (  );
            if (string.Format ( "ao", this.Key ) == command) return new AssemblerOutputFileExpression (  );

            return null;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --