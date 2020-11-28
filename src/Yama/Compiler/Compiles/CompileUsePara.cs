using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileUsePara : ICompile<VariabelDeklaration>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "UsePara";

        public int Counter
        {
            get;
            set;
        } = 2;

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();
        public IParseTreeNode Node
        {
            get;
            set;
        }

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

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(IndexVariabelnDeklaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            if (node != null)
            {
                query.Uses = node.ThisUses;
                query.Value = node.Name;
            }

            return query;
        }
        public bool Compile(Compiler compiler)
        {
            return this.CompileIndexNode(compiler, null);
        }

        public bool Compile(Compiler compiler, VariabelDeklaration node, string mode = "default")
        {
            this.Node = node;
            IndexVariabelnDeklaration dek = null;
            if (node != null) dek = node.Deklaration;

            return this.CompileIndexNode(compiler, dek, mode);
        }

        public bool CompileIndexNode(Compiler compiler, IndexVariabelnDeklaration node, string mode = "default")
        {
            if (node != null) this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode);

                Dictionary<string, string> result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), null);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys));
            }

            return true;
        }

        #endregion methods

    }

}