using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileStructAllocation : ICompile<VariabelDeklaration>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "StructAllocation";

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

        public int Position
        {
            get;
            set;
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

        public List<RegisterMap> VirtuellRegister
        {
            get;
            set;
        } = new List<RegisterMap>();

        public CompileContainer? MethodenContainer
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(IParseTreeNode node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = this;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public bool Compile(Compiler compiler, IndexVariabelnDeklaration node, string mode = "default")
        {
            this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            if (compiler.ContainerMgmt.CurrentMethod is null) return compiler.AddError("no current method found", node.Use);
            if (!compiler.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(node.Name))
                return compiler.AddError("Variable not found in mapper", node.Use);
            SSAVariableMap map = compiler.ContainerMgmt.CurrentMethod.VarMapper[node.Name];

            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this);
            compiler.AddSSALine(line);
            map.Reference = line;
            this.Line = line;

            if ( map.IsReference )
            {
                map.Value = SSAVariableMap.LastValue.Unknown;
                if ( map.Reference.Owner is CompileNumConst ) map.Value = SSAVariableMap.LastValue.Null;
                if (map.Deklaration.Use is VariabelDeklaration vd && vd.NullableToken is null) map.Value = SSAVariableMap.LastValue.NotNull;
            }
            else map.Value = SSAVariableMap.LastValue.NotNull;

            //SSACompileArgument arg = new SSACompileArgument(line);
            //compiler.ContainerMgmt.StackArguments.Push(arg);

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node.Use, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0} CSA", key.Name ), this.Node);

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

            Dictionary<string, string> postreplaces = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.PostKeys)
            {
                if (key.Name is null) continue;

                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Value = this;

                string? value = compiler.Definition.PostKeyReplace(query);
                if (value is null) return compiler.AddError("no postkeyreplace found", this.Node);

                postreplaces.Add(key.Name, value);
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys, postreplaces));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        public bool Compile(Compiler compiler, VariabelDeklaration node, string mode = "default")
        {
            return compiler.AddError("this methods is not be allowed to call");
        }

        #endregion methods

    }

}