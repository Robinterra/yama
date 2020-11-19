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
        }

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

        private bool PrintErrors()
        {
            foreach (IParseTreeNode node in this.Errors)
            {
                this.Parser.PrintSyntaxError(node.Token, "Assembler error", "Assembler error");
            }

            return false;
        }

        private bool Skipper()
        {
            byte[] result = new byte[this.Position];

            this.Stream.Write(result);

            return true;
        }

        private bool IdentifyAndAssemble(List<IParseTreeNode> nodes)
        {
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
                this.AssembleStep(assmblepair);
            }

            return true;
        }

        private bool Mappen (string key, uint position)
        {
            JumpPointMapper map = new JumpPointMapper();
            map.Key = key;
            map.Adresse = position;

            this.Mapper.Add(map);

            return true;
        }

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

        private bool AssembleStep(KeyValuePair<ICommand, IParseTreeNode> assmblepair)
        {
            RequestAssembleCommand request = new RequestAssembleCommand();
            request.Node = assmblepair.Value;
            request.Assembler = this;
            request.Stream = this.Stream;

            assmblepair.Key.Assemble(request);

            return true;
        }

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