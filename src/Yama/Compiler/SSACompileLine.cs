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

        public int Order
        {
            get;
            set;
        }

        public ICompileRoot Owner
        {
            get;
            set;
        }

        public List<SSACompileLine> PhiMap
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public List<SSACompileLine> Calls
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public bool IsPrimary
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get
            {
                if (this.IsPrimary) return true;
                if (this.IsReturn) return true;

                return this.Calls.Count != 0;
            }
        }

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

        public bool IsReturn
        {
            get;
            set;
        }

        public CompileContainer LoopContainer
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public SSACompileLine(ICompileRoot compile)
        {
            this.Owner = compile;
            this.PhiMap.Add(this);
        }

        public SSACompileLine(ICompileRoot compile, bool isprimary) : this(compile)
        {
            this.IsPrimary = isprimary;
        }

        #endregion ctor

        #region methods

        public bool AddArgument(SSACompileArgument arg)
        {
            //if (arg.Reference.ReplaceLine != null) arg.Reference = arg.Reference.ReplaceLine;

            this.Arguments.Add(arg);

            if (arg.Mode != SSACompileArgumentMode.Reference) return true;

            arg.Reference.Calls.Add(this);

            return true;
        }

        public bool DoAllocate(Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater, CompileContainer container)
        {
            if (this.SpecialRules(allocater, container, genericDefinition)) return true;

            this.DoAllocateArguments(compiler, genericDefinition, allocater);

            if (!this.HasReturn) return true;

            if (allocater.ExistAllocation(this)) return this.DoAllocateExist(compiler, genericDefinition, allocater, container);

            if (this.Calls.Count == 0)
            {
                this.Owner.PrimaryKeys.Add("[SSAPUSH]", genericDefinition.GetRegister(genericDefinition.ResultRegister));

                return true;
            }

            return this.DoAllocateByMultibleCalls(compiler, genericDefinition, allocater, container);
        }

        private bool DoAllocateByMultibleCalls(Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater, CompileContainer container)
        {
            RegisterMap newMap = allocater.GetNextFreeRegister(this);
            if (newMap == null) return compiler.AddError("Register Allocater failed to get a free Registermap, maybe out of Register and out of possible stack space", this.Owner.Node);

            newMap.Mode = RegisterUseMode.Used;
            newMap.Line = this;
            //newMap.Line.MakeAllRefs();

            this.Owner.PrimaryKeys.Add("[SSAPUSH]", newMap.Name);

            if (newMap.Type == RegisterType.Stack) return this.HandleVirtuellSetRegister(container, genericDefinition, compiler, newMap);

            if (!container.RegistersUses.Contains(newMap.Name)) container.RegistersUses.Add(newMap.Name);

            return true;
        }

        private bool DoAllocateExist(Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater, CompileContainer container)
        {
            RegisterMap map = allocater.GetReferenceRegister(this, this);

            this.Owner.PrimaryKeys.Add("[SSAPUSH]", map.Name);

            if (map.Type == RegisterType.Stack) this.HandleVirtuellSetRegister(container, genericDefinition, compiler, map);

            return true;
        }

        private bool DoAllocateArguments(Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater)
        {
            int counter = 0;

            foreach (SSACompileArgument arg in this.Arguments)
            {
                this.DoAllocateArgumentIteration(arg, counter, compiler, genericDefinition, allocater);

                counter = counter + 1;
            }

            return true;
        }

        private bool DoAllocateArgumentIteration(SSACompileArgument arg, int counter, Compiler compiler, GenericDefinition genericDefinition, RegisterAllocater allocater)
        {
            if (arg.Mode == SSACompileArgumentMode.Const) return true;
            if (arg.Mode == SSACompileArgumentMode.JumpReference) return true;

            RegisterMap map = allocater.GetReferenceRegister(arg.Reference, this);
            if (map == null) return compiler.AddError("Register Allocater can not found reference in Register", this.Owner.Node);

            this.Owner.PrimaryKeys.Add(string.Format("[SSAPOP[{0}]]", counter), map.Name);

            if (map.Mode == RegisterUseMode.Free && this.Owner is CompileReferenceCall)
            {
                map.Line.Calls.AddRange(this.Calls);
                map.Mode = RegisterUseMode.Used;
            }
            if (map.Type != RegisterType.Stack) return true;

            CompileAlgo algo = compiler.GetAlgo("RefCallStack", "Get");

            int subtraction = 0;
            if (genericDefinition.Name == "arm-t32") subtraction = 1;

            int stackPosition = (compiler.Definition.CalculationBytes * (map.RegisterId - subtraction));

            string result = algo.AssemblyCommands[0].Replace("[STACKVAR]", stackPosition.ToString());
            result = result.Replace("[NAME]", map.Name);

            this.Owner.AssemblyCommands.Add(result);

            return true;
        }

        private bool SpecialRules(RegisterAllocater allocater, CompileContainer container, GenericDefinition gd)
        {
            if (this.Owner is CompileFreeLoop)
            {
                allocater.FreeLoops(this.LoopContainer, this);

                return true;
            }

            if (this.Owner is CompileFunktionsDeklaration fd)
            {
                allocater.VirtuellRegister = fd.VirtuellRegister;

                if (fd.HasArguments) container.RegistersUses.Add(gd.GetRegister(1));
            }

            if (this.Owner is CompileFunktionsEnde fe)
            {
                fe.VirtuellRegister = allocater.VirtuellRegister;
            }

            return false;
        }

        private bool HandleVirtuellSetRegister(CompileContainer container, GenericDefinition generic, Compiler compiler, RegisterMap map)
        {
            CompileAlgo algo = compiler.GetAlgo("RefCallStack", "Set");
            int subtraction = 0;
            if (generic.Name == "arm-t32") subtraction = 1;
            this.Owner.PostAssemblyCommands.Add(algo.AssemblyCommands[0].Replace("[STACKVAR]", (compiler.Definition.CalculationBytes * (map.RegisterId - subtraction)).ToString()).Replace("[NAME]", map.Name));

            string nameReg = generic.GetRegister(generic.FramePointer);
            if (!container.RegistersUses.Contains(nameReg)) container.RegistersUses.Add(nameReg);

            return true;
        }

        private bool MakeAllRefs()
        {
            List<SSACompileLine> newCalls = new List<SSACompileLine>();

            foreach (SSACompileLine phi in this.PhiMap)
            {
                newCalls.AddRange(phi.Calls);
            }

            newCalls.AddRange(this.Calls);
            this.Calls = newCalls;

            return true;
        }

        public bool FindEquals(SSACompileLine line)
        {
            foreach (SSACompileLine refLine in this.PhiMap)
            {
                if (line.Equals(refLine)) return true;
            }

            return false;
        }

        #endregion methods

    }
}