using System;
using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileContainer
    {

        #region get/set

        public CompileSprungPunkt? Begin
        {
            get;
            set;
        }

        public CompileSprungPunkt? Ende
        {
            get;
            set;
        }

        public List<DataHold> DataHolds
        {
            get;
            set;
        } = new List<DataHold>();

        public List<SSACompileLine> Lines
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public Dictionary<string, SSAVariableMap> VarMapper
        {
            get
            {
                return this.StackVarMapper.Peek();
            }
        }

        public Stack<Dictionary<string, SSAVariableMap>> StackVarMapper
        {
            get;
            set;
        } = new Stack<Dictionary<string, SSAVariableMap>>();

        public List<string> RegistersUses
        {
            get;
            set;
        } = new();

        public List<CompileContainer> Containers
        {
            get;
            set;
        } = new List<CompileContainer>();

        public List<SSACompileLine> PhiSetNewVar
        {
            get;
            set;
        } = new List<SSACompileLine>();

        public bool HasReturned
        {
            get;
            set;
        }

        public SSACompileLine? LoopLine
        {
            get;
            set;
        }

        public IParseTreeNode? CurrentNode
        {
            get;
            set;
        }

        public Dictionary<string, SSAVariableMap>? NextContext
        {
            get;
            set;
        }

        public SSAVariableMap? ReturnType
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public string? AddDataCall(string jumpPoint, Compiler compiler)
        {
            DataHold dataHold = new DataHold();

            dataHold.JumpPoint = compiler.Definition.GenerateJumpPointName();
            dataHold.DatenValue = jumpPoint;

            this.DataHolds.Add(dataHold);

            return dataHold.JumpPoint;
        }

        public bool DoAllocate(Compiler compiler)
        {
            if (!(compiler.Definition is GenericDefinition t)) return false;
            t.Allocater.Init(t);

            foreach (SSACompileLine line in this.Lines)
            {
                line.DoAllocate(compiler, t, t.Allocater, this);
            }

            return true;
        }

        public Dictionary<string, SSAVariableMap> GetCopyOfCurrentContext()
        {
            Dictionary<string, SSAVariableMap> copyMap = new Dictionary<string, SSAVariableMap>();

            foreach (KeyValuePair<string, SSAVariableMap> orgMap in this.VarMapper)
            {
                copyMap.Add(orgMap.Key, new SSAVariableMap(orgMap.Value));
            }

            return this.NextContext = copyMap;
        }

        public bool BeginNewKontextPath()
        {
            Dictionary<string, SSAVariableMap>? copyMap = this.NextContext;
            if (copyMap is null) copyMap = this.GetCopyOfCurrentContext();

            this.NextContext = null;

            this.StackVarMapper.Push(copyMap);

            return true;
        }

        public Dictionary<string, SSAVariableMap>? PopVarMap()
        {
            if (this.StackVarMapper.Count == 0) return null;

            return this.StackVarMapper.Pop();
        }

        public bool IsReferenceInVarsContains(SSACompileLine reference)
        {
            if (reference == null) return false;
            //if (reference.FlowTask == ProgramFlowTask.Phi) return false;

            foreach (KeyValuePair<string, SSAVariableMap> keyValuePair in this.VarMapper)
            {
                if (keyValuePair.Value.Reference == null) continue;
                //if (keyValuePair.Value.Reference == reference) return true;

                foreach (SSACompileLine line in keyValuePair.Value.Reference.PhiMap)
                {
                    if (reference.Equals(line)) return true;
                }

            }

            return false;
        }

        #endregion methods

    }

}