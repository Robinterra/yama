using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public interface ICompile<T> : ICompileRoot where T : IParseTreeNode
    {

        string AlgoName
        {
            get;
            set;
        }

        bool Compile ( Compiler compiler, T node, string mode = "default" );
    }

}