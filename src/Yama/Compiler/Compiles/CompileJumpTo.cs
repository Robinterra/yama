using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileJumpTo : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "JumpTo";

        public PointMode Point
        {
            get;
            set;
        }

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public CompileSprungPunkt Punkt
        {
            get;
            set;
        }
        public Dictionary<string, string> PrimaryKeys { get; private set; }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(CompileSprungPunkt node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = node;

            return query;
        }

        public bool Compile(Compiler compiler, CompileSprungPunkt node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);

            if (this.Algo == null) return false;

            if (this.Point == PointMode.Custom) this.Punkt = node;
            if (this.Point == PointMode.CurrentBegin) this.Punkt = compiler.CurrentContainer.CurrentContainer.Begin;
            if (this.Point == PointMode.CurrentEnde) this.Punkt = compiler.CurrentContainer.CurrentContainer.Ende;
            if (this.Point == PointMode.RootBegin) this.Punkt = compiler.CurrentContainer.RootContainer.Begin;
            if (this.Point == PointMode.RootEnde) this.Punkt = compiler.CurrentContainer.RootContainer.Ende;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(this.Punkt, key, mode);

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
                compiler.AddLine(this.Algo.AssemblyCommands[i], this.PrimaryKeys);
            }

            return true;
        }

        #endregion methods

    }

    public enum PointMode
    {
        Custom,
        RootBegin,
        RootEnde,
        CurrentBegin,
        CurrentEnde
    }

}