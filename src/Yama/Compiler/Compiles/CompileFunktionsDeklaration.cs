using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFunktionsDeklaration : ICompile<FunktionsDeklaration>
    {
        public string AlgoName
        {
            get;
            set;
        } = "FunktionsDeklaration";

        public int Counter
        {
            get;
            set;
        } = 2;

        public Dictionary<string, string>[] PrimaryKeys
        {
            get;
            set;
        }
        public FunktionsDeklaration Node { get; private set; }
        public CompileAlgo Algo
        {
            get;
            set;
        }

        public bool Compile(Compiler compiler, FunktionsDeklaration node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Node = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null)  return false;

            this.PrimaryKeys = new Dictionary<string, string>[this.Algo.AssemblyCommands.Count];

            foreach (string key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Kategorie = mode;
                query.Uses = node.Deklaration.ThisUses;
                query.Value = (object)node.Deklaration.AssemblyName;

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
            Dictionary<string, string> postreplaces = new Dictionary<string, string>();

            foreach (string key in this.Algo.PostKeys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Value = this.Node.RegisterInUse;

                string value = compiler.Definition.PostKeyReplace(query);

                postreplaces.Add(key, value);
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys[i], postreplaces);
            }

            return true;
        }
    }

}