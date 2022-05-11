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

            if (args[0] == "time") return Imperialstandard();

            return true;
        }

        // -----------------------------------------------

        private const string imperialStandardPattern = "{6}-{5}.{4} {3} 0 {0} {1}.M{2}";
        private static bool Imperialstandard()
        {
            DateTime jetzt = DateTime.UtcNow;
            long isSchaltjahr = DateTime.IsLeapYear(jetzt.Year) ? 1 : 0;
            long maxstunden = 8760 + 24 * isSchaltjahr;
            long maxminuten = maxstunden * 60;
            long maxsekunden = maxminuten * 60 * 1000;
            long vergangeneStunden = (jetzt.DayOfYear - 1 * isSchaltjahr) * 24 + jetzt.Hour;
            long currentState = (((vergangeneStunden * 60 + jetzt.Minute) * 60 + jetzt.Second) * 1000 + jetzt.Millisecond) * 1000000 * 100 / maxsekunden;
            long Jahrestausendstel = currentState / 100 / 100 / 10;
            //vergangeneStunden * 1000 / maxstunden;
            // ein Jahrtausendstel sind 8,76 Stunden
            long ZehntelDrittelTag = currentState / 100 / 100 % 10;
            //ein ZehntelDrittelTag sind 52,56 Minuten
            long ZehntelDritteMinute = currentState / 100 % 100;
            //eine ZehntelDritteMinute sind 31,536 Sekunden
            long ZehntelDritteSekunde = currentState % 100;
            //eine ZehntelDritteSekunde sind 0,31536 Sekunden
            long jahr = (jetzt.Year % 1000);
            long millenium = (jetzt.Year / 1000) + 1;
            string output = string.Format(imperialStandardPattern, Jahrestausendstel, jahr, millenium, jetzt.DayOfWeek.ToString(), ZehntelDrittelTag, ZehntelDritteMinute, ZehntelDritteSekunde);

            Console.WriteLine(output);

            return false;
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