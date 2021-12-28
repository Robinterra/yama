using System.Collections.Generic;
using Yama.Compiler;
using Yama.Parser;

namespace Yama.Assembler
{
    public class AssemblerCompilerMap
    {

        #region get/set

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

        #endregion get/set

        #region ctor

        public AssemblerCompilerMap(ICompileRoot root, List<IParseTreeNode> nodes)
        {
            this.Root = root;
            this.Nodes = nodes;
        }

        #endregion ctor
    }
}