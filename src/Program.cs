using System;
using System.IO;
using System.Collections.Generic;

using Yama.Compiler;
using Yama.Compiler.Definition;

using LearnCsStuf.Automaten;
using LearnCsStuf.CommandLines;
using LearnCsStuf.CommandLines.Commands;
using Yama.Assembler;

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

            if (!Program.ExecuteArguments ( pcl.Result )) return 1;

            return 0;
        }

        // -----------------------------------------------

        private static bool ExecuteArguments ( List<ICommandLine> commands )
        {
            if (commands.Count == 0) return Program.HelpPrinten() == 1;
            DefinitionManager defs = new DefinitionManager();

            ICommandLine firstCommand = commands[0];

            if (firstCommand is CompileExpression) return Program.Build ( commands, defs );
            if (firstCommand is AssembleExpression) return Program.Assemble ( commands );
            if (firstCommand is LearnCsStuf.CommandLines.Commands.Help) return Program.HelpPrinten (  ) == 1;
            if (firstCommand is AutoExpression) return Program.RunAuto ( firstCommand );
            if (firstCommand is PrintDefinitionsExpression) return defs.PrintAllDefinitions (  );

            Console.Error.WriteLine ( "please enter a allowd argument first" );

            return false;
        }

        // -----------------------------------------------

        private static bool Assemble ( List<ICommandLine> commands )
        {
            Definitionen def = new Definitionen();
            Assembler.Assembler assembler = def.GenerateAssembler();
            RequestAssemble request = new RequestAssemble();

            foreach ( ICommandLine command in commands )
            {
                if (command is FileExpression) request.InputFile = new FileInfo ( command.Value );
                if (command is OutputFileExpression)
                {
                    FileInfo file = new FileInfo ( command.Value );
                    if (file.Exists) file.Delete();
                    request.Stream = file.OpenWrite();
                }
            }

            if (request.InputFile == null) return false;
            if (request.Stream == null) return false;

            assembler.Assemble(request);

            request.Stream.Close();

            return true;
        }

        // -----------------------------------------------

        private static bool Build ( List<ICommandLine> commands, DefinitionManager defs )
        {
            LanguageDefinition yama = new LanguageDefinition();

            foreach ( ICommandLine command in commands )
            {
                if (command is FileExpression) yama.Files.Add ( command.Value );
                if (command is IncludeExpression) yama.Includes.Add ( command.Value );
                if (command is OutputFileExpression) yama.OutputFile = command.Value;
                if (command is DefinitionExpression) yama.Definition = defs.GetDefinition ( command.Value );
                if (command is DefinesExpression) yama.Defines.Add(command.Value);
                if (command is Print t) Program.CheckPrint ( yama, t );
                if (command is StartNamespace) yama.StartNamespace = command.Value;
            }

            DirectoryInfo systemLibrary = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System"));
            if (systemLibrary.Exists) yama.Includes.Add ( systemLibrary.FullName );

            return yama.Compile();
        }

        private static bool CheckPrint(LanguageDefinition yama, Print t)
        {
            if (t.Value == "tree") yama.PrintParserTree = true;

            return true;
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

            Program.EnabledCommandLines.Add ( new CompileExpression (  ) );
            Program.EnabledCommandLines.Add ( new AssembleExpression (  ) );
            Program.EnabledCommandLines.Add ( new PrintDefinitionsExpression (  ) );
            Program.EnabledCommandLines.Add ( new AutoExpression (  ) );
            Program.EnabledCommandLines.Add ( new IncludeExpression (  ) );
            Program.EnabledCommandLines.Add ( new DefinitionExpression (  ) );
            Program.EnabledCommandLines.Add ( new OutputFileExpression (  ) );
            Program.EnabledCommandLines.Add ( new StartNamespace (  ) );
            Program.EnabledCommandLines.Add ( new DefinesExpression (  ) );
            Program.EnabledCommandLines.Add ( new Print (  ) );
            Program.EnabledCommandLines.Add ( new FileExpression (  ) );
            Program.EnabledCommandLines.Add ( new LearnCsStuf.CommandLines.Commands.Help (  ) );
            return true;
        }

        // -----------------------------------------------

    }

}

// -- [EOF] --