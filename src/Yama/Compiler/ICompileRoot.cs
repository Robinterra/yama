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

        List<string> PostAssemblyCommands
        {
            get;
            set;
        }

        IParseTreeNode? Node
        {
            get;
            set;
        }

        bool IsUsed
        {
            get;
        }

        CompileAlgo? Algo
        {
            get;
            set;
        }

        Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        }

        bool InFileCompilen(Compiler compiler);
    }

}