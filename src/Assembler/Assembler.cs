using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yama.Assembler.ARMT32;
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

            if (!this.Parser.Parse(startlayer))
            {
                this.Errors = this.Parser.ParserErrors;

                return this.PrintErrors();
            }

            this.Skipper();

            if (!this.IdentifyAndAssemble(this.Parser.ParentContainer.Statements)) return this.PrintErrors();

            return true;
        }

        // -----------------------------------------------

        public uint BuildJumpSkipper(uint position, uint adresse, uint size)
        {
            position = position + this.Definition.ProgramCounterIncress;

            if (position > adresse) return (~((position - adresse) / this.Definition.CommandEntitySize)) + 1;

            size = size / this.Definition.CommandEntitySize;

            uint result = (adresse - position) / this.Definition.CommandEntitySize;

            size = 2 - size;

            if ( result < size ) return (~(size - result)) + 1;

             return result - size;
        }

        // -----------------------------------------------

        public JumpPointMapper GetJumpPoint(string key)
        {
            return this.Mapper.FirstOrDefault(t=>t.Key == key);
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

        private bool Skipper()
        {
            byte[] result = new byte[this.Position];

            this.Stream.Write(result);

            return true;
        }

        // -----------------------------------------------

        private bool IdentifyAndAssemble(List<IParseTreeNode> nodes)
        {
            uint startposition = this.Position;

            List<KeyValuePair<ICommand, IParseTreeNode>> maptranslate = new List<KeyValuePair<ICommand, IParseTreeNode>>();

            bool isok = true;
            foreach (IParseTreeNode node in nodes)
            {
                if (node is JumpPointMarker)
                {
                    this.Mappen(node.Token.Text, this.Position);
                    continue;
                }

                ICommand command = this.Identify(node);
                if (command == null)
                {
                    this.Errors.Add(node);
                    isok = false;
                    continue;
                }
                this.Position += (uint)command.Size;

                maptranslate.Add(new KeyValuePair<ICommand, IParseTreeNode>(command, node));
            }

            if (!isok) return false;

            foreach (KeyValuePair<ICommand, IParseTreeNode> assmblepair in maptranslate)
            {
                if (!this.AssembleStep(assmblepair, startposition)) return false;

                startposition += (uint)assmblepair.Key.Size;
            }

            return this.Errors.Count == 0;
        }

        // -----------------------------------------------

        private bool Mappen (string key, uint position)
        {
            JumpPointMapper map = new JumpPointMapper();
            map.Key = key;
            map.Adresse = position;

            this.Mapper.Add(map);

            return true;
        }

        // -----------------------------------------------

        private ICommand Identify(IParseTreeNode node)
        {
            RequestIdentify request = new RequestIdentify();
            request.Node = node;

            foreach (ICommand command in this.Definition.Commands)
            {
                if (!command.Identify(request)) continue;

                return command;
            }

            return null;
        }

        // -----------------------------------------------

        private bool AssembleStep(KeyValuePair<ICommand, IParseTreeNode> assmblepair, uint position)
        {
            RequestAssembleCommand request = new RequestAssembleCommand();
            request.Node = assmblepair.Value;
            request.Assembler = this;
            request.Stream = this.Stream;
            request.Position = position;

            if (assmblepair.Key.Assemble(request)) return true;

            this.Errors.Add(request.Node);

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