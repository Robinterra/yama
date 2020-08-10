namespace LearnCsStuf.CommandLines.Commands
{
    public class StartNamespace : ICommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "start";
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
                return string.Format (CommandLines.Help.HilfePattern, this.Key, "<namespace>", "The start namespace that is to compile (default:Program)" );
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

            return new StartNamespace (  );
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --