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

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            CompileAlgo algo = compiler.GetAlgo(this.AlgoName, mode);

            Dictionary<string, string>[] primaryKeys = new Dictionary<string, string>[algo.AssemblyCommands.Count];

            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = algo.Keys[0];
            query.Kategorie = mode;
            query.Value = this.Counter;

            List<string> result = compiler.Definition.ZielRegister(query);
            if (result == null) return false; //TODO: Create Error Entry

            for (int i = 0; i < primaryKeys.Length; i++)
            {
                if (primaryKeys[i] == null) primaryKeys[i] = new Dictionary<string, string>();

                primaryKeys[i].Add(query.Key, result[i]);
            }

            for (int i = 0; i < primaryKeys.Length; i++)
            {
                compiler.AddLine(algo.AssemblyCommands[i], primaryKeys[i]);
            }

            return true;
        }
    }

}