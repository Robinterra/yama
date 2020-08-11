using System;
using System.IO;
using System.Collections.Generic;

using Yama.Compiler;
using Yama.Compiler.Definition;

using LearnCsStuf.Automaten;
using LearnCsStuf.CommandLines;
using LearnCsStuf.CommandLines.Commands;

namespace Yama
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
            System.Console.WriteLine("Remember Yama, old friend?");

            Program.Init (  );

            return Program.NormalStart ( args );
        }

        // -----------------------------------------------

        private static int NormalStart ( string[] args )
        {
            if ( !ParseCommandLine.CheckArgs ( args ) ) return HelpPrinten (  );

            ParseCommandLine pcl = new ParseCommandLine { CommandLines = EnabledCommandLines, Default = new FileExpression() };

            foreach ( string arg in args )
            {
                pcl.ArgumentAuswerten ( arg );
            }

            if (!Program.Execute ( pcl.Result )) return 1;

            return 0;
        }

        // -----------------------------------------------

        private static bool Execute ( List<ICommandLine> commands )
        {
            LanguageDefinition yama = new LanguageDefinition();
            DefinitionManager defs = new DefinitionManager();

            foreach ( ICommandLine command in commands )
            {
                if (command is LearnCsStuf.CommandLines.Commands.Help) return Program.HelpPrinten (  ) == 1;
                if (command is FileExpression) yama.Files.Add ( command.Value );
                if (command is AutoExpression) return Program.RunAuto ( command );
                if (command is IncludeExpression) yama.Includes.Add ( command.Value );
                if (command is OutputFileExpression) yama.OutputFile = command.Value;
                if (command is DefinitionExpression) yama.Definition = defs.GetDefinition ( command.Value );
                if (command is PrintDefinitionsExpression) return defs.PrintAllDefinitions (  );
                if (command is DefinesExpression) yama.Defines.Add(command.Value);
                if (command is StartNamespace) yama.StartNamespace = command.Value;
            }

            return yama.Compile();
        }

        // -----------------------------------------------

        private static bool RunAuto(ICommandLine command)
        {
            AutomatenTest test = new AutomatenTest();

            return test.Run(command.Value);
        }

        // -----------------------------------------------

        private static int HelpPrinten (  )
        {
            LearnCsStuf.CommandLines.Help hilfe = new LearnCsStuf.CommandLines.Help { CommandLines = Program.EnabledCommandLines };

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

            Program.EnabledCommandLines.Add ( new FileExpression (  ) );
            Program.EnabledCommandLines.Add ( new AutoExpression (  ) );
            Program.EnabledCommandLines.Add ( new IncludeExpression (  ) );
            Program.EnabledCommandLines.Add ( new DefinitionExpression (  ) );
            Program.EnabledCommandLines.Add ( new PrintDefinitionsExpression (  ) );
            Program.EnabledCommandLines.Add ( new OutputFileExpression (  ) );
            Program.EnabledCommandLines.Add ( new StartNamespace (  ) );
            Program.EnabledCommandLines.Add ( new DefinesExpression (  ) );
            Program.EnabledCommandLines.Add ( new LearnCsStuf.CommandLines.Commands.Help (  ) );
            return true;
        }

        // -----------------------------------------------

    }

}

// -- [EOF] --