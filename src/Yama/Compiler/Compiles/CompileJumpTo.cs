using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileJumpTo : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "JumpTo";

        public PointMode Point
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

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public CompileSprungPunkt? Punkt
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

        public bool IsUsed
        {
            get
            {
                return true;
            }
        }

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

        private DefaultRegisterQuery BuildQuery(CompileSprungPunkt node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = node;

            return query;
        }

        public bool Compile(Compiler compiler, CompileSprungPunkt? node, string mode = "default")
        {
            if (node is not null) this.Node = node.Node;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);

            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            if (this.Point == PointMode.Custom) this.Punkt = node;
            if (this.Point == PointMode.CurrentBegin) this.Punkt = compiler.ContainerMgmt.CurrentContainer?.Begin;
            if (this.Point == PointMode.CurrentEnde) this.Punkt = compiler.ContainerMgmt.CurrentContainer?.Ende;
            if (this.Point == PointMode.RootBegin) this.Punkt = compiler.ContainerMgmt.RootContainer?.Begin;
            if (this.Point == PointMode.RootEnde)
            {
                this.Punkt = compiler.ContainerMgmt.RootContainer?.Ende;
                line.FlowTask = ProgramFlowTask.IsReturn;

                try
                {
                    SSACompileArgument arg = compiler.ContainerMgmt.StackArguments.Pop();
                    arg.Reference!.FlowTask = ProgramFlowTask.IsReturnChild;
                    line.AddReference(arg);
                }
                catch{}
            }
            if (this.Point == PointMode.LoopEnde) this.Punkt = compiler.ContainerMgmt.CurrentLoop?.Ende;

            line.Arguments.Add(new SSACompileArgument(SSACompileArgumentMode.JumpReference) { CompileReference = this.Punkt });
            if (this.Punkt is null) return compiler.AddError("sprungpunkt konnte nicht ermittelt werden");

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(this.Punkt, key, mode);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), null);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            if (this.Algo is null) return false;
        
            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        #endregion methods

    }

    public enum PointMode
    {
        Custom,
        RootBegin,
        RootEnde,
        CurrentBegin,
        CurrentEnde,
        LoopEnde
    }

}