using System;
using System.Collections.Generic;

using LearnCsStuf.CommandLines;

namespace LearnCsStuf
{
    public static class Program
    {

        public static List<ICommandLine> EnabledCommandLines
        {
            get;
            set;
        }

        public static int Main ( string[] args )
        {
            Program.Init (  );

            if ( !ParseCommandLine.CheckArgs ( args ) ) return HilfeP (  );

            ParseCommandLine pcl = new ParseCommandLine { CommandLines = EnabledCommandLines };

            foreach ( string arg in args )
            {
                pcl.ArgumentAuswerten ( arg );
            }

            foreach ( ICommandLine command in pcl.Result )
            {
                if (command.Key == "print") Console.WriteLine (command.Value);
                if (command.Key == "printn") Console.WriteLine (command.Value);
            }

            return 0;
        }

        private static int HilfeP (  )
        {
            Hilfe hilfe = new Hilfe { CommandLines = Program.EnabledCommandLines };

            hilfe.Print (  );

            return 1;
        }

        private static bool Init (  )
        {
            Program.InitCommandLines (  );

            return true;
        }

        private static bool InitCommandLines (  )
        {
            Program.EnabledCommandLines = new List<ICommandLine> (  );

            Program.EnabledCommandLines.Add ( new Print (  ) );
            Program.EnabledCommandLines.Add ( new PrintN (  ) );

            return true;
        }
    }
}