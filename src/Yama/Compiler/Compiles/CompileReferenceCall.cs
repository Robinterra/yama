using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileReferenceCall : ICompile<ReferenceCall>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "ReferenceCall";


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
        public bool DoNotCompile { get; private set; }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(IndexVariabelnReference node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            if (mode == "point0")
            {
                if (query.Key.Name != "[PROPERTY]") mode = string.Empty;

                query.Kategorie = mode;

                return query;
            }

            string printmode = mode;
            if ("vektorcall" == mode || mode == "setvektorcall") printmode = "methode";
            if (node.Deklaration is IndexPropertyGetSetDeklaration) printmode = "methode";
            if (mode == "funcref") printmode = mode;
            if (mode == "setref") printmode = mode;

            query.Kategorie = printmode;
            query.Uses = node.ThisUses;

            string assemblyName = node.AssemblyName;
            if (node.Deklaration is IndexVektorDeklaration dek)
            {
                if ("vektorcall" == mode) assemblyName = dek.AssemblyNameGetMethode;
                if (mode == "setvektorcall") assemblyName = dek.AssemblyNameSetMethode;
            }
            if (node.Deklaration is IndexPropertyGetSetDeklaration pdek)
            {
                if ("point" == mode) assemblyName = pdek.AssemblyNameGetMethode;
                if (mode == "setpoint") assemblyName = pdek.AssemblyNameSetMethode;
            }

            object queryValue = assemblyName;
            if ("[REG]" == key.Name) queryValue = 1;
            if ("[PROPERTY]" == key.Name) queryValue = node.Deklaration;
            //if ("setpoint" == mode) queryValue = node.Deklaration;

            query.Value = queryValue;

            return query;
        }

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            this.Node = node;
            return this.Compile(compiler, node.Reference, mode);
        }

        public bool CompileData(Compiler compiler, IParseTreeNode node, string adressPorint)
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, "methode");
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("[NAME]", adressPorint);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        public bool CompileDek(Compiler compiler, IndexVariabelnDeklaration node, string mode = "default")
        {
            this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQueryDek(node, key, mode);

                Dictionary<string, string> result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), null);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        private DefaultRegisterQuery BuildQueryDek(IndexVariabelnDeklaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.ThisUses;
            if ("base" == node.Name) query.Uses = node.BaseUsesSet;

            object queryValue = (object)node.Name;
            if ("[REG]" == key.Name) queryValue = 1;

            query.Value = queryValue;

            return query;
        }

        public bool Compile(Compiler compiler, IndexVariabelnReference node, string mode = "default")
        {
            if (this.CheckRelevanz(compiler, node, mode)) return true;

            this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            string printmode = mode;
            if ("vektorcall" == mode || mode == "setvektorcall") printmode = "methode";
            if (node.Deklaration is IndexPropertyGetSetDeklaration) printmode = "methode";
            if ("funcref" == mode) printmode = "point";
            if ("setref" == mode) printmode = "point";

            this.Algo = compiler.GetAlgo(this.AlgoName, printmode);
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

        private bool CheckRelevanz(Compiler compiler, IndexVariabelnReference node, string mode)
        {
            if (mode != "default" && mode != "set") return false;
            if (!(node.Deklaration is IndexVariabelnDeklaration t)) return false;
            if (mode == "set") return t.References.Count == 0;

            ICompileRoot root = compiler.AssemblerSequence.LastOrDefault();
            if (!(root is CompileReferenceCall u)) return false;
            if (root.Node == null) return false;
            if (root.Node.Token.Text != node.Name) return false;
            if (u.Algo.Mode != "set") return false;

            if (t.References.Count != 1) return true;
            u.DoNotCompile = true;

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            if (this.DoNotCompile)
            {
                compiler.toRemove.Add(this);
                return true;
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys));
            }

            return true;
        }

        public bool CompilePoint0(Compiler compiler, string mode = "point")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(null, key, "point0");

                Dictionary<string, string> result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), null);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        #endregion methods

    }

}