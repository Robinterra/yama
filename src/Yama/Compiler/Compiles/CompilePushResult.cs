using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompilePushResult : ICompile<ReferenceCall>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "PushResult";

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

        public SSAVariableMap? ParameterType
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(ReferenceCall? node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = node;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public bool Compile(Compiler compiler, ReferenceCall? node, string mode = "default")
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            this.Line = line;
            compiler.AddSSALine(line);

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            if (!this.CheckParameterTypeInfos(this.Line.Arguments.FirstOrDefault(), this.ParameterType, compiler)) return false;

            if (mode == "copy")
            {
                if ( line.Arguments.Count == 0 ) return false;
                SSACompileLine? refere = line.Arguments[0].Reference;
                if (refere is null) return false;

                SSACompileArgument arg = new SSACompileArgument(refere);
                compiler.ContainerMgmt.StackArguments.Push(arg);
            }

            return true;
        }

        private bool CheckParameterTypeInfos(SSACompileArgument? sSACompileArgument, SSAVariableMap? parameterType, Compiler compiler)
        {
            if (sSACompileArgument is null) return true;
            if (parameterType is null) return true;
            if (sSACompileArgument.Map is null) return true;

            SSAVariableMap varaibleType = sSACompileArgument.Map;
            if (parameterType.MutableState == SSAVariableMap.VariableMutableState.NotMutable) return compiler.AddError("The Property/Vektor has no set Statement", parameterType.Deklaration.Use);
            if (!varaibleType.IsNullable) return true;
            if (!parameterType.IsNullable) return compiler.AddError($"It is not possible to put a '{varaibleType.Key}' ReferenceType to a non '{parameterType.Key}' Reference Parameter", parameterType.Deklaration.Use);

            if (varaibleType.Kind == SSAVariableMap.VariableType.OwnerReference && parameterType.Kind == SSAVariableMap.VariableType.OwnerReference)
            {
                varaibleType.OrgMap.Kind = SSAVariableMap.VariableType.BorrowingReference;
                varaibleType.OrgMap.MutableState = SSAVariableMap.VariableMutableState.NotMutable;
                varaibleType.OrgMap.Value = SSAVariableMap.LastValue.NeverCall;

                return true;
            }

            if (varaibleType.Kind == SSAVariableMap.VariableType.BorrowingReference && parameterType.Kind == SSAVariableMap.VariableType.OwnerReference)
                return compiler.AddError("You can not set a Borrowing variable to a owner variable", parameterType.Deklaration.Use);

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

}