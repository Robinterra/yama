using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yama.Assembler.ARMT32;
using Yama.Assembler.Definitions;
using Yama.Compiler;
using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Lexer;
using Yama.Parser;
using Yama.ProjectConfig;

namespace Yama.Assembler
{
    public class Assembler
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public AssemblerDefinition? Definition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Parser.Parser? Parser
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Stream? Stream
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<JumpPointMapper> Mapper
        {
            get;
            set;
        } = new List<JumpPointMapper>();

        // -----------------------------------------------

        public uint Position
        {
            get;
            set;
        }

        // -----------------------------------------------

        public List<IParseTreeNode> Errors
        {
            get;
            set;
        } = new List<IParseTreeNode>();

        // -----------------------------------------------

        public List<AssemblerCompilerMap> AsmCompilerMap
        {
            get;
            set;
        } = new List<AssemblerCompilerMap>();

        // -----------------------------------------------

        public uint DataAddition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public bool IsOptimizeActive
        {
            get;
            set;
        } = true;

        // -----------------------------------------------

        public List<ICommand> Sequence
        {
            get;
            set;
        } = new List<ICommand>();

        // -----------------------------------------------

        public OutputController Output
        {
            get;
        }

        public Project.OSHeader osHeader;

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public Assembler(OutputController outputController, ProjectConfig.Project.OSHeader oSHeader)
        {
            this.Output = outputController;
            this.osHeader = oSHeader;
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public IFormat? GetFormat(string key)
        {
            return this.Definition?.Formats.FirstOrDefault(t=>t.Name == key);
        }

        // -----------------------------------------------

        public bool Assemble(RequestAssemble request)
        {
            this.Stream = request.Stream;

            Definitionen definition = new Definitionen();

            ParserInputData inputData = new ParserInputData("stream", new MemoryStream());
            if (request.InputFile is not null)
            {
                if (request.InputFile.Exists) inputData = new ParserInputData(request.InputFile.FullName, request.InputFile.OpenRead());
            }

            this.Parser = definition.GetParser(inputData);

            ParserLayer? startlayer = this.Parser.ParserLayers.Find(t=>t.Name == "main");
            if (startlayer is null) return false;

            if (!this.Parse(startlayer, request, definition)) return this.PrintingErrors(this.Parser);

            if (request.IsSkipper) this.Skipper();

            if (this.Definition?.Arm2x86 is not null) return this.TranslateArm2x86();
            if (!this.IdentifyAndAssemble()) return this.PrintErrors();

            return true;
        }

        private bool TranslateArm2x86()
        {
            PushPopArm2x86 pushPop = new PushPopArm2x86(this.Definition!.Arm2x86!);
            ArimeticsArm2x86 arimetics = new ArimeticsArm2x86(this.Definition!.Arm2x86!);
            MemoryAccessArm2x86 memory = new MemoryAccessArm2x86(this.Definition!.Arm2x86!);
            BranchesArm2x86 branches = new BranchesArm2x86(this.Definition!.Arm2x86!);

            using StreamWriter writer = new StreamWriter(this.Stream!);

            foreach (IParseTreeNode node in this.Parser!.ParentContainer!.Statements)
            {
                if (pushPop.Translate(writer, node)) continue;
                if (arimetics.Translate(writer, node)) continue;
                if (memory.Translate(writer, node)) continue;
                if (branches.Translate(writer, node)) continue;
                if (node is JumpPointMarker jmp) writer.WriteLine($"{jmp.Token.Text}:");
                if (node is WordNode word) writer.WriteLine($"dd {Convert.ToInt32(word.Data!.Value)}");
                if (node is CommandWith1ArgNode cmd) this.TranslateSpecial(writer, cmd);
            }

            return true;
        }

        private void TranslateSpecial(StreamWriter writer, CommandWith1ArgNode cmd)
        {
            switch (cmd.Token.Text.ToLower())
            {
                case "section":
                    writer.WriteLine($"section .{cmd.Argument0.Token.Text}");
                    break;
                case "global":
                    writer.WriteLine($"global {cmd.Argument0.Token.Text}");
                    break;
                default:break;
            }
        }

        private bool Parse(ParserLayer startlayer, RequestAssemble request, Definitionen definition)
        {
            if (this.Parser is null) return false;
            if (request.InputFile != null) return this.Parser.Parse(startlayer);
            if (request.Roots == null) return false;

            bool isfailed = false;

            foreach (ICompileRoot root in request.Roots)
            {
                if (this.ParseRoot(startlayer, root, definition)) continue;

                isfailed = true;
            }

            return !isfailed;
        }

        public bool AddError(IParseTreeNode t, string msg)
        {
            this.Output.Print(new ParserSyntaxError(msg, t.Token));

            return false;
        }

        private bool ParseRoot(ParserLayer startlayer, ICompileRoot root, Definitionen definition)
        {
            if (this.Parser is null) return false;
            if (root.AssemblyCommands.Count == 0) return true;

            StringBuilder builder = new StringBuilder();
            foreach(string entity in root.AssemblyCommands)
            {
                builder.Append(entity);
                builder.Append("\n");
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));

            IEnumerable<IdentifierToken> tokens = definition.GetBasicLexer(stream);
            this.Parser.NewParse(tokens, new ParserInputData("assemblerStream", stream));

            if (!this.Parser.Parse(startlayer))
            {
                this.PrintingErrors(this.Parser);

                return false;
            }
            if (this.Parser.ParentContainer is null) return true;

            AssemblerCompilerMap map = new AssemblerCompilerMap(root, this.Parser.ParentContainer.Statements);

            this.AsmCompilerMap.Add(map);

            return true;
        }

        // -----------------------------------------------

        public uint BuildJumpSkipper(uint position, uint adresse, uint size, bool isnormal = false)
        {
            if (this.Definition is null) return 999999999;

            uint orgpos = position;
            position = position + this.Definition.ProgramCounterIncress;

            if (isnormal)
            {
                if (position > adresse) return (~(position - adresse )) + 1;

                return adresse - position;
            }

            if (position > adresse) return (~((position - adresse) / this.Definition.CommandEntitySize)) + 1;

            uint result = (adresse - position) / this.Definition.CommandEntitySize;
            if ( result == 1 && adresse - (orgpos + size) == 4 ) return 1;

            size = size / this.Definition.CommandEntitySize;
            size = 2 - size;

            if ( result == 0 ) return 0;//(~(result)) + 1;

            if (result - size != 0) return result;

            return result - size;
        }

        // -----------------------------------------------

        public JumpPointMapper? GetJumpPoint(string key)
        {
            JumpPointMapper? map = this.Mapper.FirstOrDefault(t=>t.Key == key);
            if (map == null) return null;

            //if (map.Key == "__start") map.Adresse = map.Adresse | 0x1;

            return map;
        }

        // -----------------------------------------------

        private bool PrintingErrors(Parser.Parser p)
        {
            List<ParserError> removes = new();
            IdentifierToken? previous = null;

            foreach ( ParserError error in p.ParserErrors )
            {
                IdentifierToken token = error.Token;

                if (previous == token) removes.Add(error);

                previous = token;

                if (token.Kind == IdentifierKind.Unknown && error.Token.ParentNode != null) token = error.Token.ParentNode.Token;
            }

            this.Output.Print(p.ParserErrors.Where(q=>!removes.Contains(q)).Select(t=>t.OutputNode));

            return false;
        }

        // -----------------------------------------------

        private bool PrintErrors()
        {
            foreach (IParseTreeNode node in this.Errors)
            {
                this.AddError(node, "Assembler error");
            }

            return false;
        }

        // -----------------------------------------------

        private bool Skipper()
        {
            if (this.Stream is null) return false;

            byte[] result = new byte[this.Position];

            this.Stream.Write(result);

            return true;
        }

        // -----------------------------------------------

        private bool IdentifyAndAssemble()
        {
            LinuxElfHeader.EI_Class eiClass = LinuxElfHeader.EI_Class.ELFCLASS32;
            LinuxElfHeader.E_Machine machine = LinuxElfHeader.E_Machine.EM_ARM;

            LinuxElfHeader linuxElfHeader = new LinuxElfHeader(eiClass, machine);
            ElfProgramHeader programHeader = new ElfProgramHeader();
            linuxElfHeader.ProgramHeaders.Add(programHeader);

            if (this.osHeader == Project.OSHeader.LinuxArm) this.Position += linuxElfHeader.Size;
            uint startposition = this.Position;

            List<CommandCompilerMap> maptranslate = new List<CommandCompilerMap>();

            if (this.AsmCompilerMap.Count == 0)
            {
                if (this.Parser?.ParentContainer is null) return false;

                this.IdentifyCommandFromNodes(this.Parser.ParentContainer.Statements, maptranslate);
            }
            else
            {
                foreach (AssemblerCompilerMap map in this.AsmCompilerMap)
                {
                    this.IdentifyCommandFromNodes(map, maptranslate);
                }
            }

            if (this.Errors.Count != 0) return false;

            this.Mappen("THE_END", this.Position + 8);

            this.CreateOsHeader(linuxElfHeader, this.osHeader, startposition, programHeader);

            foreach (CommandCompilerMap assmblepair in maptranslate)
            {
                startposition += assmblepair.Skip;

                if (!this.AssembleStep(assmblepair, startposition)) return false;

                startposition += (uint)assmblepair.Size;
            }

            return this.Errors.Count == 0;
        }

        private uint CreateOsHeader(LinuxElfHeader linuxElfHeader, Project.OSHeader osHeader, uint startposition, ElfProgramHeader programHeader)
        {
            if (this.Stream is null) return startposition;
            if (this.osHeader != Project.OSHeader.LinuxArm) return startposition;

            uint realStart = startposition - linuxElfHeader.Size;

            programHeader.VAddresse = realStart;
            programHeader.PAddresse = realStart;
            programHeader.MemorySize = this.Position;
            programHeader.FileSize = (uint)this.Position - realStart;
            programHeader.PAlign = realStart;

            uint size = linuxElfHeader.StreamData(this.Stream, startposition, this.Position);

            return size;
        }

        // -----------------------------------------------

        private bool IdentifyCommandFromNodes(List<IParseTreeNode> nodes, List<CommandCompilerMap> maptranslate)
        {
            foreach (IParseTreeNode node in nodes)
            {
                if (node is JumpPointMarker)
                {
                    this.Mappen(node.Token.Text, this.Position);
                    continue;
                }

                if (node is DataNode)
                    this.Mappen(node.Token.Text, this.Position + this.DataAddition);

                RequestIdentify request = new RequestIdentify(node, this);

                ICommand? command = this.Identify(request);
                if (command == null)
                {
                    this.Errors.Add(node);

                    continue;
                }

                uint skip = 0;
                if (request.IsData)
                {
                    uint temp = this.Position & 0x3;
                    if (temp > 0)
                    {
                        this.Position = this.Position + 4;
                        skip = 4 - temp;
                    }
                    this.Position = this.Position ^ temp;
                }

                this.Position += (uint)request.Size;

                maptranslate.Add(new CommandCompilerMap(command, node, request.Size, skip));
            }

            return true;
        }

        // -----------------------------------------------

        private bool IdentifyCommandFromNodes(AssemblerCompilerMap map, List<CommandCompilerMap> maptranslate)
        {
            foreach (IParseTreeNode node in map.Nodes)
            {
                if (node is JumpPointMarker)
                {
                    this.Mappen(node.Token.Text, this.Position);
                    continue;
                }

                if (node is DataNode)
                    this.Mappen(node.Token.Text, this.Position + this.DataAddition);

                RequestIdentify request = new RequestIdentify(node, this);

                ICommand? command = this.Identify(request);
                if (command == null)
                {
                    this.Errors.Add(node);

                    continue;
                }

                uint skip = 0;
                if (request.IsData)
                {
                    uint temp = this.Position & 0x3;
                    if (temp > 0)
                    {
                        this.Position = this.Position + 4;
                        skip = 4 - temp;
                    }
                    this.Position = this.Position ^ temp;
                }

                this.Position += (uint)request.Size;

                maptranslate.Add(new CommandCompilerMap(command, node, map, request.Size, skip));
            }

            return true;
        }

        // -----------------------------------------------

        private bool Mappen (string key, uint position)
        {
            JumpPointMapper map = new JumpPointMapper(position, key);

            this.Mapper.Add(map);

            return true;
        }

        // -----------------------------------------------

        private ICommand? Identify(RequestIdentify request)
        {
            if (this.Definition == null) return null;

            foreach (ICommand command in this.Definition.Commands)
            {
                if (!command.Identify(request)) continue;

                if (request.Size == -1) request.Size = command.Size;

                return command;
            }

            return null;
        }

        // -----------------------------------------------

        private bool AssembleStep(CommandCompilerMap assmblepair, uint position)
        {
            if (this.Stream is null) return false;

            RequestAssembleCommand request = new RequestAssembleCommand(assmblepair.Node, this, this.Stream, true, position);

            if (assmblepair.Skip != 0) this.Stream.Write(new byte[assmblepair.Skip]);

            if (!assmblepair.Command.Assemble(request))
            {
                this.Errors.Add(request.Node);

                return true;
            }

            this.Sequence.AddRange(request.Result);

            return true;
        }

        // -----------------------------------------------

        public uint GetRegister(string text)
        {
            Register? register = this.Definition?.Registers.FirstOrDefault(t=>t.Name == text);
            if (register == null) return 0; // todo print error

            return register.BinaryId;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}