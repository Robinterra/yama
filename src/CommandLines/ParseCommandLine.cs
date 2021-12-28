using System.Collections.Generic;

namespace LearnCsStuf.CommandLines
{
    public class ParseCommandLine
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<ICommandLine> CommandLines
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<ICommandLine> Result
        {
            get;
            set;
        }

        // -----------------------------------------------

        public ICommandLine? Default
        {
            get;
            set;
        }

        // -----------------------------------------------

        private ICommandLine? LastCommand
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public ParseCommandLine()
        {
            this.CommandLines = new();
            this.Result = new List<ICommandLine> (  );
        }

        // -----------------------------------------------

        ~ParseCommandLine (  )
        {

        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private bool SetLastCommandValue ( string arg )
        {
            if (this.LastCommand is null) return false;

            this.LastCommand.Value = arg;
            this.LastCommand = null;

            return true;
        }

        // -----------------------------------------------

        public bool ArgumentAuswerten ( string arg )
        {
            if (string.IsNullOrEmpty(arg)) return true;

            if (this.LastCommand != null) return this.SetLastCommandValue ( arg );

            foreach ( ICommandLine command in CommandLines )
            {
                if (command == null) continue;

                ICommandLine? test = command.Check ( arg );

                if ( test == null ) continue;

                this.Result.Add ( test );

                if ( test.HasValue ) this.LastCommand = test;

                return true;
            }

            if (this.Default == null) return false;

            ICommandLine? result = this.Default.Check(this.Default.Key);
            if (result is null) return false;

            result.Value = arg;

            this.Result.Add(result);

            return true;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

        #region static

        // -----------------------------------------------

        public static bool CheckArgs ( string[] args )
        {
            if (args == null) return false;
            if (args.Length == 0) return false;

            return true;
        }

        // -----------------------------------------------

        public static bool TestArg(string arg, string para, ref string? Variable)
        {
            string Test = arg.Replace(para, string.Empty);
            if (Test == arg) return false;

            Variable = Test;

            return true;
        }

        // -----------------------------------------------

        #endregion static

        // -----------------------------------------------

    }

}

// -- [EOF] --