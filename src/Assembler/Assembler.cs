using System;
using System.IO;
using System.Linq;
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
                System.Console.WriteLine("Error");

                foreach (IParseTreeNode node in this.Parser.ParserErrors)
                {
                    this.Parser.PrintSyntaxError(node.Token,"Error");
                }

                return false;
            }

            foreach (IParseTreeNode node in this.Parser.ParentContainer.Statements)
            {
                this.AssembleStep(node);
            }

            return true;
        }

        private bool AssembleStep(IParseTreeNode node)
        {
            RequestAssembleCommand request = new RequestAssembleCommand();
            request.Node = node;
            request.Assembler = this;
            request.Stream = this.Stream;

            foreach (ICommand command in this.Definition.Commands)
            {
                if (!command.Assemble(request)) continue;

                return true;
            }

            return false;
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