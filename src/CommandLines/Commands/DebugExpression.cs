using System.Collections.Generic;

namespace Yama.CommandLines.Commands
{
    public class DebugExpression : ICommandLine, ICommandParent
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "debug";
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, string.Empty, "Debug a Yama Source File" );
            }
        }

        // -----------------------------------------------

        public string? Value
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<ICommandLine> Childs
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public DebugExpression ( List<ICommandLine> commands )
        {
            this.Childs = commands;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public ICommandLine? Check ( string command )
        {
            if (this.Key != command) return null;

            return new DebugExpression ( this.Childs );
        }

        public bool Execute(RequestExecuteArgs request)
        {
            throw new System.NotImplementedException();
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }

}

// -- [EOF] --