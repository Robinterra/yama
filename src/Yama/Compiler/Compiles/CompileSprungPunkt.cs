using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileSprungPunkt : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "SprungPunkt";

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public List<ICompileRoot> Calls
        {
            get;
            set;
        } = new List<ICompileRoot>();

        public string? JumpPointName
        {
            get;
            set;
        }

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public IParseTreeNode? Node
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get
            {
                if (this.CleanMemoryUseErkenner is not null && this.CleanMemoryLocation is not null)
                {
                    if (this.CleanMemoryUseErkenner.First.TryToClean is null) return this.CleanMemoryLocation.FlowTask == ProgramFlowTask.CleanMemoryReturn;

                    if (this.CleanMemoryUseErkenner.First.TryToClean.Order == this.CleanMemoryLocation.Order) return true;

                    return false;

                    //int order = this.CleanMemoryUseErkenner.ArgumentsCalls.Max(t=>t.Calls.Max(t=>t.Order));

                    //return order < this.CleanMemoryLocation.Order;
                }


                return true;
            }
        }

        public SSACompileLine? CleanMemoryLocation
        {
            get;
            set;
        }

        public SSAVariableMap? CleanMemoryUseErkenner
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public SSACompileLine? Line
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public bool Add(Compiler compiler, ICompileRoot call)
        {
            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();

            this.Calls.Add(call);

            return true;
        }

        public bool Compile(Compiler compiler, IParseTreeNode parent, string mode = "default")
        {
            this.Node = parent;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            if (this.Node is IfKey) line.FlowTask = ProgramFlowTask.IsIfStatementEnde;
            this.Line = line;

            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            if (this.Calls.Count == 0) return true;
            if (this.Algo is null) return false;
            if (!this.IsUsed) return true;

            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], null, new Dictionary<string, string> { { "[NAME]", this.JumpPointName! } }));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        #endregion methods

    }

}