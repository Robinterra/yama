using System;
using System.Collections.Generic;

using LearnCsStuf.CommandLines;
using LearnCsStuf.Basic;
using LearnCsStuf.CommandLines.Commands;
using System.IO;

namespace LearnCsStuf
{
    public static class Program
    {

        // -----------------------------------------------

        public static List<ICommandLine> EnabledCommandLines
        {
            get;
            set;
        }

        // -----------------------------------------------

        public static int Main ( string[] args )
        {
            Program.Init (  );

            return Program.NormalStart ( args );
        }

        // -----------------------------------------------

        private static int NormalStart ( string[] args )
        {
            if ( !ParseCommandLine.CheckArgs ( args ) ) return HilfeP (  );

            ParseCommandLine pcl = new ParseCommandLine { CommandLines = EnabledCommandLines };

            foreach ( string arg in args )
            {
                pcl.ArgumentAuswerten ( arg );
            }

            Program.Execute ( pcl.Result );

            return 0;
        }

        // -----------------------------------------------

        private static bool Execute ( List<ICommandLine> commands )
        {
            List<string> parseFiles = new List<string>();
            List<string> parsecsvFiles = new List<string>();

            foreach ( ICommandLine command in commands )
            {
                if (command.Key == "help") return Program.HilfeP (  ) == 1;
                if (command.Key == "print") Console.WriteLine ( command.Value );
                if (command.Key == "printn") Console.WriteLine ( command.Value );
                if (command.Key == "basic") parseFiles.Add ( command.Value );
                if (command.Key == "csv") parsecsvFiles.Add ( command.Value );
            }

            foreach (string value in parseFiles)
            {
                BasicExpressionEvaluator basic = new BasicExpressionEvaluator();

                basic.File = value;

                basic.Compile();
            }

            foreach (string value in parsecsvFiles)
            {
                Console.WriteLine ( value );
                Lexer l = new Lexer(File.OpenRead ( value ));
                l.LexerTokens.Add(
                    new Splitter
                    (
                        new List<ZeichenKette> { new ZeichenKette(";"), new ZeichenKette("\n") },
                        new Escaper
                        (
                            new ZeichenKette("\\"),
                            new List<Replacer>
                            {
                                new Replacer(new ZeichenKette(";"), ";"),
                                new Replacer( new ZeichenKette (":)"), "😊")
                            }
                        )
                    )
                );
                foreach ( SyntaxToken token in l )
                {
                    Console.WriteLine ( token.Value );
                }
            }

            return true;
        }

        // -----------------------------------------------

        private static int HilfeP (  )
        {
            Hilfe hilfe = new Hilfe { CommandLines = Program.EnabledCommandLines };

            hilfe.Print (  );

            return 1;
        }

        // -----------------------------------------------

        private static bool Init (  )
        {
            Program.InitCommandLines (  );

            return true;
        }

        // -----------------------------------------------

        private static bool InitCommandLines (  )
        {
            Program.EnabledCommandLines = new List<ICommandLine> (  );

            Program.EnabledCommandLines.Add ( new Print (  ) );
            Program.EnabledCommandLines.Add ( new PrintN (  ) );
            Program.EnabledCommandLines.Add ( new BasicExpression (  ) );
            Program.EnabledCommandLines.Add ( new CsvExpression (  ) );
            Program.EnabledCommandLines.Add ( new Help (  ) );

            return true;
        }

        // -----------------------------------------------

    }

}

// -- [EOF] --