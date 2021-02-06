using System;
using System.Collections.Generic;
using Yama.Compiler.Definition;

namespace Yama.Compiler
{
    public class SSACompileLine
    {

        #region get/set

        public SSACompileLine ReplaceLine
        {
            get;
            set;
        }

        public CompileAlgo Algo
        {
            get
            {
                return this.Owner.Algo;
            }
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

        public bool HasReturn
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public SSACompileLine(ICompileRoot compile)
        {
            this.Owner = compile;
        }

        #endregion ctor

        #region methods

        public bool AddArgument(SSACompileArgument arg)
        {
            if (arg.Reference.ReplaceLine != null) arg.Reference = arg.Reference.ReplaceLine;

            this.Arguments.Add(arg);

            if (arg.Mode != SSACompileArgumentMode.Reference) return true;

            arg.Reference.Calls.Add(this);

            return true;
        }

        public bool DoAllocate(Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater, CompileContainer container)
        {
            int counter = 0;
            foreach (SSACompileArgument arg in this.Arguments)
            {
                RegisterMap map = allocater.GetReferenceRegister(arg.Reference, this);
                if (map == null) return compiler.AddError("Register Allocater can not found reference in Register", this.Owner.Node);

                this.Owner.PrimaryKeys.Add(string.Format("[SSAPOP[{0}]]", counter), map.Name);

                counter++;
            }

            if (!this.HasReturn) return true;
            if (this.Calls.Count == 0)
            {
                this.Owner.PrimaryKeys.Add("[SSAPUSH]", genericDefinition.GetRegister(genericDefinition.ResultRegister));
                return true;
            }

            RegisterMap newMap = allocater.GetNextFreeRegister();
            if (newMap == null) return compiler.AddError("Register Allocater failed to get a free Registermap, maybe out of Register and out of possible stack space", this.Owner.Node);

            newMap.Mode = RegisterUseMode.Used;
            newMap.Line = this;

            this.Owner.PrimaryKeys.Add("[SSAPUSH]", newMap.Name);

            if (!container.RegistersUses.Contains(newMap.Name)) container.RegistersUses.Add(newMap.Name);

            return true;
        }

        #endregion methods

    }
}