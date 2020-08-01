using System;
using System.Collections.Generic;

using LearnCsStuf.CommandLines;
using Yama;
using LearnCsStuf.CommandLines.Commands;
using System.IO;
using LearnCsStuf.Automaten;
using Yama.Lexer;

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
            System.Console.WriteLine("Erinnerst du dich an Yama, alter Freund?");

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
            string framework = "atmega328p";

            foreach ( ICommandLine command in commands )
            {
                if (command.Key == "help") return Program.HilfeP (  ) == 1;
                if (command.Key == "print") Console.WriteLine ( command.Value );
                if (command.Key == "printn") Console.WriteLine ( command.Value );
                if (command.Key == "yama") parseFiles.Add ( command.Value );
                if (command.Key == "csv") parsecsvFiles.Add ( command.Value );
                if (command.Key == "auto") Program.RunAuto ( command );
                if (command.Key == "framework") framework = command.Value;
            }

            LanguageDefinition basic = new LanguageDefinition();

            basic.Framework = framework;

            basic.Files = parseFiles;

            basic.Compile();

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

        private static void RunAuto(ICommandLine command)
        {
            AutomatenTest test = new AutomatenTest();

            test.Run(command.Value);
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
            Program.EnabledCommandLines.Add ( new YamaExpression (  ) );
            Program.EnabledCommandLines.Add ( new CsvExpression (  ) );
            Program.EnabledCommandLines.Add ( new Help (  ) );
            Program.EnabledCommandLines.Add ( new AutoExpression (  ) );
            return true;
        }

        // -----------------------------------------------

    }

}

// -- [EOF] --