namespace LearnCsStuf.CommandLines
{
    public class PrintN : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "printn";
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
                return string.Format ( Hilfe.HilfePattern, string.Format ( "--{0}:<Text>", this.Key ), string.Empty, "Gibt <text> in der Console aus" );
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
            string value = null;

            if ( !ParseCommandLine.TestArg ( command, string.Format ( "--{0}:", this.Key ), ref value ) ) return null;

            this.Value = value;

            return this;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --