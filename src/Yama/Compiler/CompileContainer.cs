using System;
using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileContainer
    {
        public CompileSprungPunkt Begin
        {
            get;
            set;
        }

        public CompileSprungPunkt Ende
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
        }

        public List<CompileContainer> Containers
        {
            get;
            set;
        } = new List<CompileContainer>();

        public string AddDataCall(string jumpPoint, Compiler compiler)
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

        public bool BeginNewContainerVars()
        {
            Dictionary<string, SSAVariableMap> copyMap = new Dictionary<string, SSAVariableMap>();

            foreach (KeyValuePair<string, SSAVariableMap> orgMap in this.VarMapper)
            {
                copyMap.Add(orgMap.Key, new SSAVariableMap(orgMap.Value));
            }

            this.StackVarMapper.Push(copyMap);

            return true;
        }

        public Dictionary<string, SSAVariableMap> PopVarMap()
        {
            return this.StackVarMapper.Pop();
        }
    }

}