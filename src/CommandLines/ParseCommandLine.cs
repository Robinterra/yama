using System.Collections.Generic;

namespace LearnCsStuf.CommandLines
{
    public class ParseCommandLine
    {
        #region get/set

        public List<ICommandLine> CommandLines
        {
            get;
            set;
        }

        public List<ICommandLine> Result
        {
            get;
            set;
        }

        private ICommandLine LastCommand
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public ParseCommandLine()
        {
            this.Result = new List<ICommandLine> (  );
        }

        ~ParseCommandLine (  )
        {

        }

        #endregion ctor

        #region methods

        private bool SetLastCommandValue ( string arg )
        {
                this.LastCommand.Value = arg;
                this.LastCommand = null;
                return true;
        }

        public bool ArgumentAuswerten ( string arg )
        {
            if (string.IsNullOrEmpty(arg)) return true;

            if (this.LastCommand != null) return this.SetLastCommandValue ( arg );

            foreach ( ICommandLine command in CommandLines )
            {
                if (command == null) continue;

                ICommandLine test = command.Check ( arg );

                if ( test == null ) continue;

                this.Result.Add ( test );

                if ( test.HasValue ) this.LastCommand = test;
            }

            return false;
        }

        #endregion methods

        #region static

        public static bool CheckArgs ( string[] args )
        {
            if (args == null) return false;
            if (args.Length == 0) return false;

            return true;
        }

        public static bool TestArg(string arg, string para, ref string Variable)
        {
            string Test = arg.Replace(para, string.Empty);
            if (Test == arg) return false;

            Variable = Test;

            return true;
        }

        #endregion static
    }
}