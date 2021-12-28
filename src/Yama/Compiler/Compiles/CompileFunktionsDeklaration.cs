using System;
using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFunktionsDeklaration : ICompile<MethodeDeclarationNode>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "FunktionsDeklaration";

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

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

        public MethodeDeclarationNode? MethodNode
        {
            get;
            private set;
        }

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

        public CompileAlgo? Algo
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

        public bool HasArguments
        {
            get;
            set;
        }

        public SSACompileLine? Line
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(MethodeDeclarationNode node, AlgoKeyCall key, string mode)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            if (node.Deklaration is null) return query;

            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.Deklaration.ThisUses;
            query.Value = (object)node.Deklaration.AssemblyName;

            return query;
        }

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

            if (mode == "set"&& node.Deklaration is not null)
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

        public bool Compile(Compiler compiler, MethodeDeclarationNode node, string mode = "default")
        {
            if (node.Deklaration is null) return false;

            this.Node = node;
            this.HasArguments = node.Deklaration.Parameters.Count != 0;
            compiler.AssemblerSequence.Add(this);

            this.MethodNode = node;
            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

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

            this.HasArguments = true;

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

            this.HasArguments = true;

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
            if (this.HasArguments) this.MakeArgumentsSPAdd(compiler);
            if (this.VirtuellRegister.Count != 0) this.MakeVirtuelAdvnced(compiler);

            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

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
                if (key.Name == "[VARCOUNT]") query.Value = varcount;
                if (key.Name == "[virtuelRegister]") query.Value = this.VirtuellRegister.Count;

                string? value = compiler.Definition.PostKeyReplace(query);
                if (value is null) continue;

                postreplaces.Add(key.Name, value);
            }

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

        private bool MakeArgumentsSPAdd(Compiler compiler)
        {
            CompileAlgo? algo = compiler.GetAlgo(this.AlgoName, "stackpushcount");
            if (algo == null) return false;

            this.PostAssemblyCommands.AddRange(algo.AssemblyCommands);

            return true;
        }

        private bool MakeVirtuelAdvnced(Compiler compiler)
        {
            CompileAlgo? algo = compiler.GetAlgo(this.AlgoName, "virtuel");
            if (algo == null) return false;

            this.PostAssemblyCommands.AddRange(algo.AssemblyCommands);

            return true;
        }

        #endregion methods

    }

}