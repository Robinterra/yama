using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public interface ICompileRoot
    {

        List<string> AssemblyCommands
        {
            get;
            set;
        }

        IParseTreeNode Node
        {
            get;
            set;
        }

        bool IsUsed
        {
            get;
        }

        CompileAlgo Algo
        {
            get;
            set;
        }

        bool InFileCompilen(Compiler compiler);
    }

}