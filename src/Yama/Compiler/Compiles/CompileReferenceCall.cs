using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileReferenceCall : ICompile<ReferenceCall>
    {
        public string AlgoName
        {
            get;
            set;
        } = "ReferenceCall";


        public CompileAlgo Algo
        {
            get;
            set;
        }

        public Dictionary<string, string>[] PrimaryKeys
        {
            get;
            set;
        }

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            return this.Compile(compiler, node.Reference, mode);
        }

        public bool Compile(Compiler compiler, IndexVariabelnReference node, string mode = "default")
        {
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>[this.Algo.AssemblyCommands.Count];

            foreach (string key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Kategorie = mode;
                query.Uses = node.ThisUses;
                query.Value = key == "[REG]" ? (object)1 : (object)node.AssemblyName;

                List<string> result = compiler.Definition.ZielRegister(query);
                if (result == null) return compiler.AddError("Es konnten keine daten zum Keyword geladen werden");

                for (int i = 0; i < result.Count; i++)
                {
                    if (this.PrimaryKeys[i] == null) this.PrimaryKeys[i] = new Dictionary<string, string>();

                    this.PrimaryKeys[i].Add(key, result[i]);
                }
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys[i]);
            }

            return true;
        }
    }

}