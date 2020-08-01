using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFunktionsEnde : ICompile<FunktionsDeklaration>
    {
        public string AlgoName
        {
            get;
            set;
        } = "FunktionsEnde";

        public int Counter
        {
            get;
            set;
        } = 2;

        public bool Compile(Compiler compiler, FunktionsDeklaration node, string mode = "default")
        {
            CompileAlgo algo = compiler.GetAlgo(this.AlgoName, mode);

            Dictionary<string, string>[] primaryKeys = new Dictionary<string, string>[algo.AssemblyCommands.Count];

            foreach (string key in algo.Keys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Kategorie = mode;
                query.Uses = node.Deklaration.ThisUses;
                query.Value = (object)node.Deklaration.Name;

                List<string> result = compiler.Definition.ZielRegister(query);
                if (result == null) return false; //TODO: Create Error Entry

                for (int i = 0; i < result.Count; i++)
                {
                    if (primaryKeys[i] == null) primaryKeys[i] = new Dictionary<string, string>();

                    primaryKeys[i].Add(key, result[i]);
                }
            }

            for (int i = 0; i < algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(algo.AssemblyCommands[i], primaryKeys[i]);
            }

            return true;
        }
    }

}