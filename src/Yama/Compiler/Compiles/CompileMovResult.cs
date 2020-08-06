using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileMovResult : ICompile<ReferenceCall>
    {
        public string AlgoName
        {
            get;
            set;
        } = "MovResult";

        public int Counter
        {
            get;
            set;
        } = 2;

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        }

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>();

            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = this.Algo.Keys[0];
            query.Kategorie = mode;
            query.Value = this.Counter;

            Dictionary<string, string> result = compiler.Definition.KeyMapping(query);
            if (result == null) return compiler.AddError("Es konnten keine daten zum Keyword geladen werden", node);

            foreach (KeyValuePair<string, string> pair in result)
            {
                if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError("Es wurde bereits ein Keyword hinzugefügt", node);
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys);
            }

            return true;
        }
    }

}