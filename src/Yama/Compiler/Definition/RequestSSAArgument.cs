using System.Collections.Generic;

namespace Yama.Compiler.Definition
{
    public class RequestSSAArgument
    {

        public int Count
        {
            get;
            set;
        }

        public SSACompileLine Target
        {
            get;
            set;
        }

        public List<SSACompileArgument> Arguments
        {
            get;
            set;
        } = new List<SSACompileArgument>();

        public RequestSSAArgument(SSACompileLine target)
        {
            this.Target = target;
        }
    }
}