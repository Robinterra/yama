using Yama.Parser;

namespace Yama.Compiler
{

    public class CompilePhi : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "Phi";

        public CompileAlgo? Algo
        {
            get;
            set;
        } = new CompileAlgo() { Name = "Phi" };

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

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

        public SSACompileLine? Line
        {
            get;
            set;
        }
        public IParseTreeNode? Node
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public bool Compile(Compiler compiler, IEnumerable<KeyValuePair<string, SSAVariableMap>> variableMaps, IParseTreeNode node)
        {
            CompileContainer? container = compiler.ContainerMgmt.CurrentMethod;
            if (container is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> variableMap in variableMaps)
            {
                if (variableMap.Value.Reference is null) continue;
                if (!container.VarMapper.ContainsKey(variableMap.Key)) continue;
                SSAVariableMap currentVarMap = container.VarMapper[variableMap.Key];
                if (currentVarMap.Reference is null)
                {
                    currentVarMap.Reference = variableMap.Value.Reference;

                    continue;
                }

                SSACompileLine varLine = new SSACompileLine(this);
                varLine.FlowTask = ProgramFlowTask.Phi;
                varLine.AddArgument(new SSACompileArgument(currentVarMap.Reference));
                varLine.AddArgument(new SSACompileArgument(variableMap.Value.Reference));

                if (variableMap.Value.Value == SSAVariableMap.LastValue.NotSet) currentVarMap.Value = SSAVariableMap.LastValue.NotSet;
                if (variableMap.Value.Value == SSAVariableMap.LastValue.Unknown) currentVarMap.Value = SSAVariableMap.LastValue.Unknown;
                if (variableMap.Value.Value == SSAVariableMap.LastValue.Null && currentVarMap.Value == SSAVariableMap.LastValue.Null) currentVarMap.Value = SSAVariableMap.LastValue.Null;
                else if (variableMap.Value.Value == SSAVariableMap.LastValue.Null) currentVarMap.Value = SSAVariableMap.LastValue.Unknown;

                compiler.AddSSALine(varLine);

                if (currentVarMap.Reference.FlowTask == ProgramFlowTask.Phi)
                {
                    this.ReferenceIsAlreadyPhi(currentVarMap.Reference, variableMap.Value.Reference);
                    currentVarMap.Reference = varLine;

                    continue;
                }

                if (variableMap.Value.Reference.FlowTask == ProgramFlowTask.Phi)
                {
                    this.ReferenceIsAlreadyPhi(variableMap.Value.Reference, currentVarMap.Reference);
                    currentVarMap.Reference = varLine;

                    continue;
                }

                currentVarMap.Reference.PhiMap.Add(variableMap.Value.Reference);
                variableMap.Value.Reference.PhiMap.Add(currentVarMap.Reference);

                currentVarMap.Reference = varLine;

            }

            return true;
        }

        private void ReferenceIsAlreadyPhi(SSACompileLine thePhiLine, SSACompileLine theoriginLine)
        {
            foreach (SSACompileArgument arg in thePhiLine.Arguments)
            {
                if (arg.Reference is null) continue;

                arg.Reference.PhiMap.Add(theoriginLine);
                theoriginLine.PhiMap.Add(arg.Reference);
            }
        }

        public bool InFileCompilen(Compiler compiler)
        {
            return true;
        }

        #endregion methods

    }

}