using System.Collections.Generic;

namespace Yama.Compiler
{

    public class SSACompileArgument
    {

        #region get/set

        public SSACompileLine? Reference
        {
            get;
            set;
        }

        public SSAVariableMap? Variable
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

        public ICompileRoot? Root
        {
            get;
            set;
        }

        public CompileSprungPunkt? CompileReference
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public SSACompileArgument(SSACompileArgumentMode mode)
        {
            this.Mode = mode;
        }

        public SSACompileArgument(SSACompileLine line)
        {
            this.Reference = line;
            this.Mode = SSACompileArgumentMode.Reference;
            this.Root = line.Owner;
        }

        #endregion ctor

    }

    public enum SSACompileArgumentMode
    {
        None,
        Variable,
        Reference,
        Const,
        JumpReference
    }
}