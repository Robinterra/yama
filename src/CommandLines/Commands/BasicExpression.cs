namespace LearnCsStuf.CommandLines.Commands
{
    public class BasicExpression : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "basic";
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
                return string.Format ( Hilfe.HilfePattern, this.Key, "<file>", "Parse File und gibt das Ergebnis wieder" );
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

            return new BasicExpression (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --