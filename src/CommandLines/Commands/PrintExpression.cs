namespace Yama.CommandLines.Commands
{
    public class PrintExpression : ICommandLine
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, "<subcommand>", "Zum Einstellen des Consolen Output: Um den ParserTree auszugeben 'tree', um die Parsetime für einzelne Dateien auszugeben 'parsetime', um die Zeit der einzlenen Pahsen auszugeben 'phasetime'" );
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

            return new PrintExpression();
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --