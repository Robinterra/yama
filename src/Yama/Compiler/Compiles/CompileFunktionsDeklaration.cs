using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFunktionsDeklaration : ICompile<FunktionsDeklaration>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "FunktionsDeklaration";

        public Dictionary<string, string> PrimaryKeys
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

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(FunktionsDeklaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.Deklaration.ThisUses;
            query.Value = (object)node.Deklaration.AssemblyName;

            return query;
        }

        public bool Compile(Compiler compiler, FunktionsDeklaration node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Node = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null)  return false;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode);

                Dictionary<string, string> result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError("Es konnten keine daten zum Keyword geladen werden", null);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError("Es wurde bereits ein Keyword hinzugefügt", null);
                }
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            Dictionary<string, string> postreplaces = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.PostKeys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Value = this.Node.RegisterInUse;

                string value = compiler.Definition.PostKeyReplace(query);

                postreplaces.Add(key.Name, value);
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys, postreplaces);
            }

            return true;
        }

        #endregion methods

    }

}