using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileRegionDefAlgo : ICompile<ConditionalCompilationNode>
    {

        #region get/set

        public string AlgoName
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

        public bool IsUsed
        {
            get
            {
                return true;
            }
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(ConditionalCompilationNode node, AlgoKeyCall key, string mode, SSACompileLine arg)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            if (node != null)
            {
                //query.Uses = node.;
                query.Value = node;
            }

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(arg);

            return query;
        }

        public bool Compile(Compiler compiler, ConditionalCompilationNode node, string mode = "default")
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            string algo = node.Token.Value.ToString (  ).Replace(" ", string.Empty);
            if (algo.Contains(","))
            {
                string[] split = algo.Split(",");
                algo = split[0];
                mode = split[1];
            }

            this.Algo = compiler.GetAlgo (algo , mode );

            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this);
            compiler.AddSSALine(line);

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode, line);

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