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

        public bool Compile(Compiler compiler, Number node, string mode = "default")
        {
            CompileAlgo algo = compiler.GetAlgo(this.AlgoName, mode);

            Dictionary<string, string>[] primaryKeys = new Dictionary<string, string>[algo.AssemblyCommands.Count];

            foreach (string key in algo.Keys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Kategorie = mode;
                query.Value = node.Token.Value;

                List<string> result = compiler.Definition.ZielRegister(query);
                if (result == null) return false; //TODO: Create Error Entry

                for (int i = 0; i < primaryKeys.Length; i++)
                {
                    if (primaryKeys[i] == null) primaryKeys[i] = new Dictionary<string, string>();

                    primaryKeys[i].Add(key, result[i]);
                }
            }

            for (int i = 0; i < primaryKeys.Length; i++)
            {
                compiler.AddLine(algo.AssemblyCommands[i], primaryKeys[i]);
            }

            return true;
        }
    }

}