using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yama.Assembler.ARMT32;
using Yama.Compiler;
using Yama.Lexer;
using Yama.Parser;

namespace Yama.Assembler
{
    public class Assembler
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public AssemblerDefinition Definition
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Parser.Parser Parser
        {
            get;
            set;
        }

        // -----------------------------------------------

        public Stream Stream
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

        #endregion get/set

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        public IFormat GetFormat(string key)
        {
            return this.Definition.Formats.FirstOrDefault(t=>t.Name == key);
        }

        // -----------------------------------------------

        public bool Assemble(RequestAssemble request)
        {
            this.Stream = request.Stream;

            Definitionen definition = new Definitionen();

            this.Parser = definition.GetParser(request.InputFile);

            ParserLayer startlayer = this.Parser.ParserLayers.FirstOrDefault(t=>t.Name == "main");

            if (!this.Parse(startlayer, request)) return this.PrintParserErrors();

            if (request.IsSkipper) this.Skipper();

            if (!this.IdentifyAndAssemble()) return this.PrintErrors();

            return true;
        }

        private bool Parse(ParserLayer startlayer, RequestAssemble request)
        {
            if (request.InputFile != null) return this.Parser.Parse(startlayer);
            if (request.Roots == null) return false;

            foreach (ICompileRoot root in request.Roots)
            {
                if (this.ParseRoot(startlayer, root)) continue;
            }

            return this.Errors.Count == 0;
        }

        public bool AddError(IParseTreeNode t, string v)
        {
            this.Parser.PrintSyntaxError(t.Token, v, "Assembler error");

            return false;
        }

        private bool ParseRoot(ParserLayer startlayer, ICompileRoot root)
        {
            StringBuilder builder = new StringBuilder();
            foreach(string entity in root.AssemblyCommands)
            {
                builder.Append(entity);
                builder.Append("\n");
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));

            this.Parser.NewParse();

            if (!this.Parser.Parse(startlayer, stream))
            {
                this.Errors.AddRange(this.Parser.ParserErrors);

                return false;
            }

            AssemblerCompilerMap map = new AssemblerCompilerMap();
            map.Root = root;
            map.Nodes = this.Parser.ParentContainer.Statements;

            this.AsmCompilerMap.Add(map);

            return true;
        }

        // -----------------------------------------------

        public uint BuildJumpSkipper(uint position, uint adresse, uint size, bool isnormal = false)
        {
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

        public JumpPointMapper GetJumpPoint(string key)
        {
            JumpPointMapper map = this.Mapper.FirstOrDefault(t=>t.Key == key);
            if (map == null) return null;

            //if (map.Key == "__start") map.Adresse = map.Adresse | 0x1;

            return map;
        }

        // -----------------------------------------------

        private bool PrintErrors()
        {
            foreach (IParseTreeNode node in this.Errors)
            {
                this.Parser.PrintSyntaxError(node.Token, "Assembler error", "Assembler error");
            }

            return false;
        }

        // -----------------------------------------------

        
        private bool PrintParserErrors()
        {
            foreach (IParseTreeNode node in this.Parser.ParserErrors)
            {
                this.Parser.PrintSyntaxError(node.Token, "Parser error", "Assembler error");
            }

            return false;
        }

        // -----------------------------------------------

        private bool Skipper()
        {
            byte[] result = new byte[this.Position];

            this.Stream.Write(result);

            return true;
        }

        // -----------------------------------------------

        private bool IdentifyAndAssemble()
        {
            uint startposition = this.Position;

            List<CommandCompilerMap> maptranslate = new List<CommandCompilerMap>();

            if (this.AsmCompilerMap.Count == 0) this.IdentifyCommandFromNodes(this.Parser.ParentContainer.Statements, maptranslate);
            else
            {
                foreach (AssemblerCompilerMap map in this.AsmCompilerMap)
                {
                    this.IdentifyCommandFromNodes(map, maptranslate);
                }
            }

            if (this.Errors.Count != 0) return false;

            foreach (CommandCompilerMap assmblepair in maptranslate)
            {
                startposition += assmblepair.Skip;

                if (!this.AssembleStep(assmblepair, startposition)) return false;

                startposition += (uint)assmblepair.Size;
            }

            return this.Errors.Count == 0;
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

                ICommand command = this.Identify(request);
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

                ICommand command = this.Identify(request);
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

        private ICommand Identify(RequestIdentify request)
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
            RequestAssembleCommand request = new RequestAssembleCommand();
            request.Node = assmblepair.Node;
            request.Assembler = this;
            request.Stream = this.Stream;
            request.WithMapper = true;
            request.Position = position;

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
            Register register = this.Definition.Registers.FirstOrDefault(t=>t.Name == text);
            if (register == null) return 0; // todo print error

            return register.BinaryId;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}