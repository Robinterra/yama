using System.Collections.Generic;
using Yama.Compiler.Definition;
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

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public IParseTreeNode? Node
        {
            get;
            set;
        }

        public MethodeDeclarationNode? MethodNode
        {
            get;
            set;
        }

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

        public VektorDeclaration? GetNode
        {
            get;
            private set;
        }

        public VektorDeclaration? SetNode
        {
            get;
            private set;
        }

        public PropertyGetSetDeklaration? PGetNode
        {
            get;
            private set;
        }

        public PropertyGetSetDeklaration? PSetNode
        {
            get;
            private set;
        }

        public bool IsUsed
        {
            get
            {
                return true;
            }
        }

        public int ArgsCount
        {
            get;
            set;
        }

        public List<RegisterMap> VirtuellRegister
        {
            get;
            set;
        } = new List<RegisterMap>();

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public SSACompileLine? Line
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(VektorDeclaration node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = "default";

            if (mode == "set" && node.Deklaration is not null)
            {
                query.Uses = node.Deklaration.SetUses;
                query.Value = (object)node.Deklaration.AssemblyNameSetMethode;
            }

            if (mode == "get" && node.Deklaration is not null)
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

            if (mode == "set" && node.Deklaration is not null)
            {
                query.Uses = node.Deklaration.SetUses;
                query.Value = (object)node.Deklaration.AssemblyNameSetMethode;
            }

            if (mode == "get" && node.Deklaration is not null)
            {
                query.Uses = node.Deklaration.GetUses;
                query.Value = (object)node.Deklaration.AssemblyNameGetMethode;
            }

            return query;
        }

        private DefaultRegisterQuery BuildQuery(MethodeDeclarationNode node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            if (node.Deklaration is null) return query;

            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.Deklaration.ThisUses;
            query.Value = (object)node.Deklaration.Name;

            return query;
        }

        public bool Compile(Compiler compiler, MethodeDeclarationNode node, string mode = "default")
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            this.MethodNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            compiler.EndCurrentMethod();

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        public bool Compile(Compiler compiler, VektorDeclaration node, string mode = "default")
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            compiler.EndCurrentMethod();

            if (mode == "set") this.SetNode = node;
            if (mode == "get") this.GetNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null)  return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
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
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            compiler.EndCurrentMethod();

            if (mode == "set") this.PSetNode = node;
            if (mode == "get") this.PGetNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, "default");
            if (this.Algo == null)  return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
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
            if (this.Algo is null) return false;

            Dictionary<string, string> postreplaces = new Dictionary<string, string>();

            int varcount = 0;
            if (this.MethodNode != null) varcount = this.MethodNode.VariabelCounter;
            if (this.SetNode != null) varcount = this.SetNode.SetVariabelCounter;
            if (this.GetNode != null) varcount = this.GetNode.GetVariabelCounter;
            if (this.PSetNode != null) varcount = this.PSetNode.SetVariabelCounter;
            if (this.PGetNode != null) varcount = this.PGetNode.GetVariabelCounter;

            List<string>? registerInUse = null;
            if (this.MethodNode != null) registerInUse = this.MethodNode.RegisterInUse;
            if (this.SetNode != null) registerInUse = this.SetNode.SetRegisterInUse;
            if (this.GetNode != null) registerInUse = this.GetNode.GetRegisterInUse;
            if (this.PSetNode != null) registerInUse = this.PSetNode.SetRegisterInUse;
            if (this.PGetNode != null) registerInUse = this.PGetNode.GetRegisterInUse;

            foreach (AlgoKeyCall key in this.Algo.PostKeys)
            {
                if (key.Name is null) continue;

                DefaultRegisterQuery query = new DefaultRegisterQuery();
                query.Key = key;
                query.Value = registerInUse;
                if (key.Name == "[stackcount]") query.Value = this.ArgsCount;
                if (key.Name == "[virtuelRegister]") query.Value = this.VirtuellRegister.Count;

                string? value = compiler.Definition.PostKeyReplace(query);
                if (value is null) return compiler.AddError("postkeyreplace konnte nicht ermittelt werden", this.Node);

                postreplaces.Add(key.Name, value);
            }

            if (this.VirtuellRegister.Count != 0) this.MakeVirtuelAdvnced(compiler, postreplaces);;

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys, postreplaces));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, this.PrimaryKeys, postreplaces));
            }

            return true;
        }

        private bool MakeVirtuelAdvnced(Compiler compiler, Dictionary<string, string> postreplaces)
        {
            CompileAlgo? algo = compiler.GetAlgo(this.AlgoName, "virtuel");
            if (algo == null) return false;

            foreach (string str in algo.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, this.PrimaryKeys, postreplaces));
            }

            return true;
        }

        #endregion methods

    }

}