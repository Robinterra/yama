using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFunktionsEnde : ICompile<MethodeDeclarationNode>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "FunktionsEnde";

        public MethodeDeclarationNode Node { get; private set; }
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

        
        public VektorDeclaration GetNode { get; private set; }
        public VektorDeclaration SetNode { get; private set; }

        public PropertyGetSetDeklaration PGetNode { get; private set; }
        public PropertyGetSetDeklaration PSetNode { get; private set; }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(VektorDeclaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = "default";

            if (mode == "set")
            {
                query.Uses = node.Deklaration.SetUses;
                query.Value = (object)node.Deklaration.AssemblyNameSetMethode;
            }

            if (mode == "get")
            {
                query.Uses = node.Deklaration.GetUses;
                query.Value = (object)node.Deklaration.AssemblyNameGetMethode;
            }

            return query;
        }

        private DefaultRegisterQuery BuildQuery(PropertyGetSetDeklaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = "default";

            if (mode == "set")
            {
                query.Uses = node.Deklaration.SetUses;
                query.Value = (object)node.Deklaration.AssemblyNameSetMethode;
            }

            if (mode == "get")
            {
                query.Uses = node.Deklaration.GetUses;
                query.Value = (object)node.Deklaration.AssemblyNameGetMethode;
            }

            return query;
        }

        private DefaultRegisterQuery BuildQuery(MethodeDeclarationNode node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.Deklaration.ThisUses;
            query.Value = (object)node.Deklaration.Name;

            return query;
        }

        public bool Compile(Compiler compiler, MethodeDeclarationNode node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Node = node;
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

        public bool Compile(Compiler compiler, VektorDeclaration node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            if (mode == "set") this.SetNode = node;
            if (mode == "get") this.GetNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null)  return false;

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

        public bool Compile(Compiler compiler, PropertyGetSetDeklaration node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            if (mode == "set") this.PSetNode = node;
            if (mode == "get") this.PGetNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null)  return false;

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
            Dictionary<string, string> postreplaces = new Dictionary<string, string>();

            int varcount = 0;
            if (this.Node != null) varcount = this.Node.VariabelCounter;
            if (this.SetNode != null) varcount = this.SetNode.SetVariabelCounter;
            if (this.GetNode != null) varcount = this.GetNode.GetVariabelCounter;
            if (this.PSetNode != null) varcount = this.PSetNode.SetVariabelCounter;
            if (this.PGetNode != null) varcount = this.PGetNode.GetVariabelCounter;

            List<string> registerInUse = null;
            if (this.Node != null) registerInUse = this.Node.RegisterInUse;
            if (this.SetNode != null) registerInUse = this.SetNode.SetRegisterInUse;
            if (this.GetNode != null) registerInUse = this.GetNode.GetRegisterInUse;
            if (this.PSetNode != null) registerInUse = this.PSetNode.SetRegisterInUse;
            if (this.PGetNode != null) registerInUse = this.PGetNode.GetRegisterInUse;

            foreach (AlgoKeyCall key in this.Algo.PostKeys)
            {
                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Value = registerInUse;
                if (key.Name == "[VARCOUNT]") query.Value = varcount;

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