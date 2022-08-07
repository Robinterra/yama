using System.Collections.Generic;
using Yama.Index;

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

        public SSAVariableMap? Variable
        {
            get;
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

        public IParent? IndexRef
        {
            get;
            set;
        }

        public List<SSACompileLine> Calls
        {
            get;
        }

        #endregion get/set

        #region ctor

        public SSACompileArgument(SSACompileArgumentMode mode)
        {
            this.Mode = mode;
            this.Calls = new List<SSACompileLine>();
        }

        public SSACompileArgument(SSACompileLine line, SSAVariableMap? map = null)
        {
            this.Reference = line;
            this.Mode = SSACompileArgumentMode.Reference;
            this.Root = line.Owner;
            if (map is not null) this.Variable = new SSAVariableMap(map);
            this.Calls = new List<SSACompileLine>();
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