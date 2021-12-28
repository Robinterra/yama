using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public class CommandCompilerMap
    {

        #region get/set

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

        public AssemblerCompilerMap? Map
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public uint Skip
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CommandCompilerMap(ICommand command, IParseTreeNode node, int size, uint skip)
        {
            this.Command = command;
            this.Node = node;
            this.Size = size;
            this.Skip = skip;
        }

        public CommandCompilerMap(ICommand command, IParseTreeNode node, AssemblerCompilerMap map, int size, uint skip)
        {
            this.Command = command;
            this.Node = node;
            this.Map = map;
            this.Size = size;
            this.Skip = skip;
        }

        #endregion ctor
    }
}