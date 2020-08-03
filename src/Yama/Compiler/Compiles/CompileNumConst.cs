using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileNumConst : ICompile<Number>
    {
        public string AlgoName
        {
            get;
            set;
        } = "NumConst";

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

        public bool Compile(Compiler compiler, Number node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>[this.Algo.AssemblyCommands.Count];

            foreach (string key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Kategorie = mode;
                query.Value = node.Token.Value;

                List<string> result = compiler.Definition.ZielRegister(query);
                if (result == null) return compiler.AddError("Es konnten keine daten zum Keyword geladen werden", node);

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