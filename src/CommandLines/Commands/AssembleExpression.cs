using System.Collections.Generic;

namespace Yama.CommandLines.Commands
{
    public class AssembleExpression : ICommandLine, ICommandParent
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public string Key
        {
            get
            {
                return "assemble";
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, string.Empty, "Assemble a Assembler file to Binary" );
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

        public AssembleExpression ( List<ICommandLine> commands )
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

            return new AssembleExpression ( this.Childs );
        }

        // -----------------------------------------------

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