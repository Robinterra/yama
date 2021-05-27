using System;
using System.Collections.Generic;
using System.Linq;

namespace Yama.Compiler
{
    public class OptimizeIRCode
    {

        #region get/set

        public List<SSACompileLine> IRCode
        {
            get;
            set;
        }

        public List<OptimizeMethod> Methods
        {
            get;
            set;
        }

        #endregion get/set

        #region delegate

        public delegate bool OptimizeMethod(RequestOptimize request);

        #endregion delegate

        #region ctor

        public OptimizeIRCode(List<SSACompileLine> ircode)
        {
            this.IRCode = ircode;

            this.Init();
        }

        #endregion ctor

        #region methods

        private bool Init()
        {
            this.Methods = new List<OptimizeMethod>();
            this.Methods.Add(this.RemoveNotNecessaryJumps);

            return true;
        }

        public bool Run()
        {
            List<SSACompileLine> canBeRemove = new List<SSACompileLine>();

            foreach (SSACompileLine line in this.IRCode)
            {
                RequestOptimize request = new RequestOptimize(line, canBeRemove, this.IRCode);

                this.RunIterationOfOneSSALine(request);
            }

            return this.Clean ( canBeRemove );
        }

        private bool RunIterationOfOneSSALine(RequestOptimize request)
        {
            foreach (OptimizeMethod method in this.Methods)
            {
                if (method(request)) return true;
            }

            return true;
        }

        private bool Clean(List<SSACompileLine> removeList)
        {
            foreach (SSACompileLine line in removeList)
            {
                this.IRCode.Remove(line);
            }

            return true;
        }

        #endregion methods

        #region delagetMethods

        private bool RemoveNotNecessaryJumps(RequestOptimize request)
        {
            SSACompileLine line = request.Current;
            if (!(line.Owner is CompileJumpTo)) return false;

            SSACompileArgument arg = line.Arguments.FirstOrDefault();
            if (arg == null) return false;

            if (arg.Mode != SSACompileArgumentMode.JumpReference) return false;
            if (arg.CompileReference.Line.Order != line.Order + 1) return false;

            request.ToRemove.Add(line);

            return true;
        }

        #endregion delagetMethods

    }
}