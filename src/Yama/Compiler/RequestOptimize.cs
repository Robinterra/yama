using System.Collections.Generic;

namespace Yama.Compiler
{
    public class RequestOptimize
    {

        #region get/set

        public SSACompileLine Current
        {
            get;
            set;
        }

        public List<SSACompileLine> ToRemove
        {
            get;
            set;
        }

        public List<SSACompileLine> IRCode
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestOptimize(SSACompileLine line, List<SSACompileLine> toremove, List<SSACompileLine> ircode)
        {
            this.Current = line;
            this.ToRemove = toremove;
            this.IRCode = ircode;
        }

        #endregion ctor

    }
}