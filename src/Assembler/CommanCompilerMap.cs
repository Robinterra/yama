using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public class CommandCompilerMap
    {
        public CommandCompilerMap(ICommand command, IParseTreeNode node)
        {
            this.Command = command;
            this.Node = node;
        }

        public CommandCompilerMap(ICommand command, IParseTreeNode node, AssemblerCompilerMap map)
        {
            this.Command = command;
            this.Node = node;
            this.Map = map;
        }

        public ICommand Command
        {
            get;
            set;
        }

        public IParseTreeNode Node
        {
            get;
            set;
        }

        public AssemblerCompilerMap Map
        {
            get;
            set;
        }
    }
}