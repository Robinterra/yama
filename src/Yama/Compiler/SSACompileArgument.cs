using System.Collections.Generic;

namespace Yama.Compiler
{

    public class SSACompileArgument
    {

        public SSACompileLine Reference
        {
            get;
            set;
        }

        public SSAVariableMap Variable
        {
            get;
            set;
        }

        public long Const
        {
            get;
            set;
        }

        public SSACompileArgumentMode Mode
        {
            get;
            set;
        }

        public ICompileRoot Root
        {
            get;
            set;
        }

        public SSACompileArgument()
        {

        }

        public SSACompileArgument(SSACompileLine line)
        {
            this.Reference = line;
            this.Mode = SSACompileArgumentMode.Reference;
            this.Root = line.Owner;
        }
    }

    public enum SSACompileArgumentMode
    {
        None,
        Variable,
        Reference,
        Const
    }
}