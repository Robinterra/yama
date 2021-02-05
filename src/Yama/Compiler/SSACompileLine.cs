using System.Collections.Generic;

namespace Yama.Compiler
{
    public class SSACompileLine
    {

        public SSACompileLine ReplaceLine
        {
            get;
            set;
        }

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public ICompileRoot Owner
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        public List<SSACompileArgument> Arguments
        {
            get;
            set;
        } = new List<SSACompileArgument>();

    }
}