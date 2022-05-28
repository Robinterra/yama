namespace Yama.CommandLines.Commands
{
    public class CsvExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "csv";
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, "<file>", "Parse eine csv File und gibt die key aus" );
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
            if (string.Format ( "{0}", this.Key ) != command) return null;

            return new CsvExpression (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --