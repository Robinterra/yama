using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileMovReg : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "MovReg";

        public CompileAlgo? Algo
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

        private DefaultRegisterQuery BuildQuery(ReturnKey node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = node.Token.Value;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public SSACompileLine? Compile(Compiler compiler, ReturnKey node, string mode = "default")
        {
            this.Node = node;
            //compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return null;

            SSACompileLine line = new SSACompileLine(this, true);
            //compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null)
                {
                    compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden  44 {0}", key.Name ), node);

                    return null;
                }

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value ))
                    {
                        compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);

                        return null;
                    }
                }
            }

            SSACompileArgument? arg = line.Arguments.FirstOrDefault();
            if (arg is null) return line;
            CompileContainer? currentMethode = compiler.ContainerMgmt.CurrentMethod;
            if (currentMethode is null) return line;
            if (currentMethode.ReturnType is null) return line;
            if (!currentMethode.ReturnType.IsReference) return line;

            if (arg.Reference is not null)
            {
                if (arg.Reference.Owner is CompileNumConst && !currentMethode.ReturnType.Deklaration.IsNullable && currentMethode.ReturnType.Deklaration.IsReference)
                {
                    compiler.AddError($"can not return null, Returntype is not nullable", node);

                    return null;
                }
            }
            if (arg.Variable is null) return line;

            if (!currentMethode.ReturnType.Deklaration.IsNullable)
            {
                if (arg.Variable.Value != SSAVariableMap.LastValue.NotNull)
                {
                    compiler.AddError($"can not return null '{arg.Variable.Key}', Returntype is not nullable", node);

                    return null;
                }
            }

            if (currentMethode.ReturnType.Kind == SSAVariableMap.VariableType.BorrowingReference && arg.Variable.Kind == SSAVariableMap.VariableType.OwnerReference)
            {
                compiler.AddError($"can not borrowing from variable '{arg.Variable.Key}', variable will be clear after leaving the scope", node);

                return null;
            }

            if (currentMethode.ReturnType.Kind == SSAVariableMap.VariableType.OwnerReference && arg.Variable.Kind == SSAVariableMap.VariableType.BorrowingReference)
            {
                compiler.AddError($"Expectet a owner variable, but '{arg.Variable.Key}' is a borrowing varaible", node);

                return null;
            }

            if (currentMethode.ReturnType.Kind == SSAVariableMap.VariableType.OwnerReference && arg.Variable.Kind == SSAVariableMap.VariableType.OwnerReference)
            {
                arg.Variable.OrgMap.Kind = SSAVariableMap.VariableType.BorrowingReference;
                arg.Variable.OrgMap.Value = SSAVariableMap.LastValue.NotSet;
                arg.Variable.OrgMap.MutableState = SSAVariableMap.VariableMutableState.NotMutable;

                return line;
            }

            //arg.Map.OrgMap

            return line;
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
                compiler.AddLine(new RequestAddLine(this,this.Algo.AssemblyCommands[i], this.PrimaryKeys));
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