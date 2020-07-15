using Yama.Parser;

namespace Yama.Compiler
{

    public interface ICompile
    {

        A GetAlgo<A> (  ) where A : ICompileAlgo;

        bool Compile<T> ( Compiler compiler, IParseTreeNode node, T datenCache ) where T : IDatenCache;
    }

}