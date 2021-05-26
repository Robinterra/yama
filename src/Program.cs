using System;
using System.IO;
using System.Collections.Generic;

using Yama.Compiler;
using Yama.Compiler.Definition;

using LearnCsStuf.Automaten;
using LearnCsStuf.CommandLines;
using LearnCsStuf.CommandLines.Commands;
using Yama.Assembler;
using Yama.Debug;

namespace Yama
{
    public static class Program
    {

        // -----------------------------------------------

        private static LanguageDefinition yama;

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

            if (firstCommand is FileExpression) return Program.RunSofort ( firstCommand.Value, commands );
            System.Console.WriteLine("Remember Yama, old friend?");
            if (firstCommand is CompileExpression) return Program.Build ( commands, defs );
            if (firstCommand is DebugExpression) return Program.Debug ( commands, defs );
            if (firstCommand is AssembleExpression) return Program.Assemble ( commands );
            if (firstCommand is RunExpression) return Program.Run ( commands );
            if (firstCommand is LearnCsStuf.CommandLines.Commands.Help) return Program.HelpPrinten (  ) == 1;
            if (firstCommand is AutoExpression) return Program.RunAuto ( firstCommand );
            if (firstCommand is PrintDefinitionsExpression) return defs.PrintAllDefinitions (  );

            Console.Error.WriteLine ( "please enter a allowd argument first" );

            return false;
        }

        // -----------------------------------------------

        private static bool RunSofort(string value, List<ICommandLine> commands)
        {
            FileInfo file = new FileInfo(value);
            if (file.Extension != ".yexe") return false;
            if (!file.Exists) return false;

            Runtime runtime = new Runtime();
            runtime.IsDebug = false;
            runtime.MemorySize = 0x2FAF080;
            runtime.Input = file;

            bool isfirst = true;
            foreach ( ICommandLine command in commands )
            {
                if (command is SizeExpression) runtime.MemorySize = (uint) int.Parse(command.Value.Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber);
                if (command is FileExpression && !isfirst) runtime.Arguments.Add(command.Value);
                if (command is FileExpression && isfirst) isfirst = false;
            }

            return runtime.Execute();
        }

        // -----------------------------------------------

        private static bool Debug(List<ICommandLine> commands, DefinitionManager defs)
        {
            Program.yama = new LanguageDefinition();

            if (!Program.Assemble(commands)) return false;

            Runtime runtime = new Runtime();
            runtime.Sequence = Program.yama.Sequence;
            runtime.Input = new FileInfo(Program.yama.OutputFile);

            foreach ( ICommandLine command in commands )
            {
                if (command is SizeExpression) runtime.MemorySize = (uint) int.Parse(command.Value.Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber);
                if (command is FileExpression) runtime.Arguments.Add(command.Value);
            }

            return runtime.Execute();
        }

        // -----------------------------------------------

        private static bool Run(List<ICommandLine> commands)
        {
            Runtime runtime = new Runtime();

            bool isfirst = true;
            foreach ( ICommandLine command in commands )
            {
                if (command is FileExpression) runtime.Input = new FileInfo ( command.Value );
                if (command is SizeExpression) runtime.MemorySize = Program.ParseSkipExpressionHex(command.Value);
                if (command is FileExpression && !isfirst) runtime.Arguments.Add(command.Value);
                if (command is FileExpression && isfirst) isfirst = false;
            }

            if (runtime.Input == null) return false;

            return runtime.Execute();
        }

        // -----------------------------------------------

        private static bool Assemble ( List<ICommandLine> commands )
        {
            Definitionen def = new Definitionen();
            Assembler.Assembler assembler = new Assembler.Assembler();
            RequestAssemble request = new RequestAssemble();
            if (Program.yama == null) Program.yama = new LanguageDefinition();

            foreach ( ICommandLine command in commands )
            {
                if (command is DefinitionExpression) assembler = def.GenerateAssembler ( assembler, command.Value );
                if (command is FileExpression) request.InputFile = new FileInfo ( command.Value );
                if (command is SkipExpression) assembler.Position = Program.ParseSkipExpressionHex(command.Value);
                if (command is OutputFileExpression)
                {
                    FileInfo file = new FileInfo ( command.Value );
                    if (file.Exists) file.Delete();
                    request.Stream = file.OpenWrite();
                }
            }

            if (request.InputFile == null) return false;
            if (request.Stream == null)
            {
                FileInfo file = new FileInfo ( "out.bin" );
                if (file.Exists) file.Delete();
                request.Stream = file.OpenWrite();
            }
            if (assembler.Definition == null) assembler = def.GenerateAssembler ( assembler, "runtime" );

            bool isok = assembler.Assemble(request);

            request.Stream.Close();

            if (isok) Program.yama.Sequence = assembler.Sequence;

            return isok;
        }

        // -----------------------------------------------

        private static bool Build ( List<ICommandLine> commands, DefinitionManager defs )
        {
            if (Program.yama == null) Program.yama = new LanguageDefinition();

            foreach ( ICommandLine command in commands )
            {
                if (command is FileExpression) yama.Files.Add ( command.Value );
                if (command is IncludeExpression) yama.Includes.Add ( command.Value );
                if (command is AssemblerOutputFileExpression) yama.OutputAssemblerFile = command.Value;
                if (command is OutputFileExpression) yama.OutputFile = command.Value;
                if (command is OptimizingExpression) yama.OptimizeLevel = Program.GetOptimizeLevel ( command.Value );
                if (command is DefinitionExpression) yama.Definition = defs.GetDefinition ( command.Value );
                if (command is DefinesExpression) yama.Defines.Add(command.Value);
                if (command is Print t) Program.CheckPrint ( yama, t );
                if (command is SkipExpression) yama.StartPosition = Program.ParseSkipExpressionHex(command.Value);
                if (command is StartNamespace) yama.StartNamespace = command.Value;
                if (command is IROutputExpression) yama.IROutputFile = command.Value;
            }

            DirectoryInfo systemLibrary = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System"));
            if (systemLibrary.Exists) yama.Includes.Add ( systemLibrary.FullName );

            return yama.Compile();
        }

        // -----------------------------------------------

        private static uint ParseSkipExpressionHex(string value)
        {
            if (!uint.TryParse(value.Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber, null, out uint result))
            {
                Console.Error.WriteLine("can not parse hex number from arguments");

                Environment.Exit(1);
            }

            return result;
        }

        // -----------------------------------------------

        private static Optimize GetOptimizeLevel(string value)
        {
            if (value == "none") return Optimize.None;
            if (value == "ssa") return Optimize.SSA;

            return Optimize.Level1;
        }

        // -----------------------------------------------

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

            List<ICommandLine> compileArgs = new List<ICommandLine>();
            compileArgs.Add( new FileExpression () );
            compileArgs.Add( new IncludeExpression () );
            compileArgs.Add( new AssemblerOutputFileExpression () );
            compileArgs.Add( new OutputFileExpression () );
            compileArgs.Add( new OptimizingExpression () );
            compileArgs.Add( new DefinitionExpression () );
            compileArgs.Add( new DefinesExpression () );
            compileArgs.Add( new Print () );
            compileArgs.Add( new SkipExpression () );
            compileArgs.Add( new StartNamespace () );
            compileArgs.Add( new IROutputExpression  () );

            List<ICommandLine> debugArgs = new List<ICommandLine>();
            debugArgs.Add(new SizeExpression ());
            debugArgs.Add(new FileExpression ());

            List<ICommandLine> runArgs = new List<ICommandLine>();
            runArgs.Add(new SizeExpression ());
            runArgs.Add(new FileExpression ());

            List<ICommandLine> assembleArgs = new List<ICommandLine>();
            assembleArgs.Add(new SizeExpression ());
            assembleArgs.Add(new FileExpression ());
            assembleArgs.Add( new DefinitionExpression () );
            assembleArgs.Add( new OutputFileExpression () );
            assembleArgs.Add( new SkipExpression () );

            Program.EnabledCommandLines.Add ( new CompileExpression ( compileArgs ) );
            Program.EnabledCommandLines.Add ( new AssembleExpression ( assembleArgs ) );
            Program.EnabledCommandLines.Add ( new RunExpression ( runArgs ) );
            Program.EnabledCommandLines.Add ( new DebugExpression ( debugArgs ) );
            Program.EnabledCommandLines.Add ( new PrintDefinitionsExpression (  ) );
            Program.EnabledCommandLines.Add ( new AssemblerOutputFileExpression (  ) );
            Program.EnabledCommandLines.Add ( new AutoExpression (  ) );
            Program.EnabledCommandLines.Add ( new IncludeExpression (  ) );
            Program.EnabledCommandLines.Add ( new DefinitionExpression (  ) );
            Program.EnabledCommandLines.Add ( new IROutputExpression (  ) );
            Program.EnabledCommandLines.Add ( new OptimizingExpression (  ) );
            Program.EnabledCommandLines.Add ( new OutputFileExpression (  ) );
            Program.EnabledCommandLines.Add ( new StartNamespace (  ) );
            Program.EnabledCommandLines.Add ( new DefinesExpression (  ) );
            Program.EnabledCommandLines.Add ( new SkipExpression (  ) );
            Program.EnabledCommandLines.Add ( new SizeExpression (  ) );
            Program.EnabledCommandLines.Add ( new Print (  ) );
            Program.EnabledCommandLines.Add ( new FileExpression (  ) );
            Program.EnabledCommandLines.Add ( new LearnCsStuf.CommandLines.Commands.Help (  ) );
            return true;
        }

        // -----------------------------------------------

    }

}

// -- [EOF] --