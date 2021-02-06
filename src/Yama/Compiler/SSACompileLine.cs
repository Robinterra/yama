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

        public List<SSACompileLine> Calls
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public List<SSACompileArgument> Arguments
        {
            get;
            set;
        } = new List<SSACompileArgument>();

        public bool AddArgument(SSACompileArgument arg)
        {
            this.Arguments.Add(arg);

            if (arg.Mode != SSACompileArgumentMode.Reference) return true;

            arg.Reference.Calls.Add(this);

            return true;
        }

    }
}