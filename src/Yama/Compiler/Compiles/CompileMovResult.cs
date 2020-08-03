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

        public Dictionary<string, string>[] PrimaryKeys
        {
            get;
            set;
        }

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>[this.Algo.AssemblyCommands.Count];

            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = this.Algo.Keys[0];
            query.Kategorie = mode;
            query.Value = this.Counter;

            List<string> result = compiler.Definition.ZielRegister(query);
            if (result == null) return false; //TODO: Create Error Entry

            for (int i = 0; i < this.PrimaryKeys.Length; i++)
            {
                if (this.PrimaryKeys[i] == null) this.PrimaryKeys[i] = new Dictionary<string, string>();

                this.PrimaryKeys[i].Add(query.Key, result[i]);
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.PrimaryKeys.Length; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys[i]);
            }

            return true;
        }
    }

}