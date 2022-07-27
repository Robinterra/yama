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

        public bool CompileBeginLoop(Compiler compiler, IParseTreeNode parseTreeNode)
        {
            CompileContainer? container = compiler.ContainerMgmt.CurrentMethod;
            if (container is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> variableMap in container.VarMapper)
            {
                SSAVariableMap currentVarMap = variableMap.Value;

                SSACompileLine varLine = new SSACompileLine(this);
                varLine.FlowTask = ProgramFlowTask.Phi;
                if (currentVarMap.Reference is not null) varLine.AddArgument(new SSACompileArgument(currentVarMap.Reference));

                compiler.AddSSALine(varLine);
                currentVarMap.Reference = varLine;
            }

            return true;
        }

        public bool Compile(Compiler compiler, IEnumerable<KeyValuePair<string, SSAVariableMap>> variableMaps, IParseTreeNode node)
        {
            CompileContainer? container = compiler.ContainerMgmt.CurrentMethod;
            if (container is null) return true;

            foreach (KeyValuePair<string, SSAVariableMap> variableMap in variableMaps)
            {
                if (variableMap.Value.Reference is null) continue;
                if (!container.VarMapper.ContainsKey(variableMap.Key)) continue;
                SSAVariableMap currentVarMap = container.VarMapper[variableMap.Key];

                if (variableMap.Value.Value == SSAVariableMap.LastValue.NotSet) currentVarMap.Value = SSAVariableMap.LastValue.NotSet;
                if (variableMap.Value.Value == SSAVariableMap.LastValue.Unknown) currentVarMap.Value = SSAVariableMap.LastValue.Unknown;
                if (variableMap.Value.Value == SSAVariableMap.LastValue.Null && currentVarMap.Value == SSAVariableMap.LastValue.Null) currentVarMap.Value = SSAVariableMap.LastValue.Null;
                else if (variableMap.Value.Value == SSAVariableMap.LastValue.Null) currentVarMap.Value = SSAVariableMap.LastValue.Unknown;

                if (variableMap.Value.PhiOnlyValueChecking) continue;

                if (currentVarMap.Reference is null)
                {
                    currentVarMap.Reference = variableMap.Value.Reference;

                    continue;
                }

                SSACompileLine varLine = new SSACompileLine(this);
                varLine.FlowTask = ProgramFlowTask.Phi;
                varLine.AddArgument(new SSACompileArgument(currentVarMap.Reference));
                varLine.AddArgument(new SSACompileArgument(variableMap.Value.Reference));

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

            theoriginLine.PhiMap.Add(thePhiLine);
            thePhiLine.PhiMap.Add(theoriginLine);
        }

        public bool InFileCompilen(Compiler compiler)
        {
            return true;
        }

        public bool CompileLoopEndPhis(Compiler compiler, SSACompileLine phiLoop, List<SSACompileLine> phis, SSACompileLine reference, SSAVariableMap currentVarMap)
        {
            SSACompileLine varLine = new SSACompileLine(this);
            varLine.FlowTask = ProgramFlowTask.Phi;
            varLine.AddArgument(new SSACompileArgument(phiLoop));
            varLine.AddArgument(new SSACompileArgument(reference));
            if (reference != phiLoop)
            {
                phiLoop.AddArgument(new SSACompileArgument(reference));
                phiLoop.PhiMap.Add(reference);
                reference.PhiMap.Add(phiLoop);
            }

            // wird gemacht um IsUsed zu verwenden
            int dex = phiLoop.Calls.IndexOf(varLine);
            phiLoop.Calls[dex] = phiLoop.Calls[0];
            phiLoop.Calls[0] = varLine;

            varLine.Calls.AddRange(phiLoop.Calls.Skip(1));

            compiler.AddSSALine(varLine);
            phis.Add(varLine);

            currentVarMap.Reference = varLine;
            if (!currentVarMap.IsNullable) return true;

            currentVarMap.Value = SSAVariableMap.LastValue.Unknown;

            return true;
        }

        #endregion methods

    }

}