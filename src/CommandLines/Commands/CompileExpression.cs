using System.Collections.Generic;

namespace LearnCsStuf.CommandLines.Commands
{
    public class CompileExpression : ICommandLine, ICommandParent
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
                return string.Format (CommandLines.HelpController.HilfePattern, this.Key, string.Empty, "Build a Yama Programm" );
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

        public CompileExpression ( List<ICommandLine> commands )
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

            return new CompileExpression ( this.Childs );
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