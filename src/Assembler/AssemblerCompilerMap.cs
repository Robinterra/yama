using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public class AssemblerCompilerMap
    {
        public ICompileRoot Root
        {
            get;
            set;
        }

        public List<IParseTreeNode> Nodes
        {
            get;
            set;
        }
    }
}