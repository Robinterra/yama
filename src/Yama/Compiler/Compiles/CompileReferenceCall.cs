using System;
using System.Collections.Generic;
using System.Linq;
using Yama.Compiler.Definition;
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

        public bool DoNotCompile
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get;
            set;
        } = true;

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

        public bool IsNullCheck
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        private DefaultRegisterQuery BuildQuery(IndexVariabelnReference? node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            if (mode == "point0")
            {
                if (query.Key.Name != "[PROPERTY]") mode = string.Empty;

                query.Kategorie = mode;

                if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

                return query;
            }

            string printmode = mode;
            if ("vektorcall" == mode || mode == "setvektorcall") printmode = "methode";
            if (node != null) if (node.Deklaration is IndexPropertyGetSetDeklaration) printmode = "methode";
            if (mode == "funcref") printmode = mode;
            if (mode == "setref") printmode = mode;

            query.Kategorie = printmode;
            if (node != null) query.Uses = node.ThisUses;

            string assemblyName = node == null ? string.Empty : node.AssemblyName;
            if (node != null) if (node.Deklaration is IndexVektorDeklaration dek)
            {
                if ("vektorcall" == mode) assemblyName = dek.AssemblyNameGetMethode;
                if (mode == "setvektorcall") assemblyName = dek.AssemblyNameSetMethode;
            }
            if (node != null) if (node.Deklaration is IndexPropertyGetSetDeklaration pdek)
            {
                if ("point" == mode) assemblyName = pdek.AssemblyNameGetMethode;
                if (mode == "setpoint") assemblyName = pdek.AssemblyNameSetMethode;
            }

            object? queryValue = assemblyName;
            if ("[REG]" == key.Name) queryValue = 1;
            if (node != null) if ("[PROPERTY]" == key.Name) queryValue = node.Deklaration;
            //if ("setpoint" == mode) queryValue = node.Deklaration;

            query.Value = queryValue;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            this.Node = node;
            if (node.Reference is null) return false;
            return this.Compile(compiler, node.Reference, mode);
        }

        public bool CompileData(Compiler compiler, IParseTreeNode node, string adressPorint)
        {
            this.Node = node;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, "methode");
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this);
            compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();
            this.PrimaryKeys.Add("[NAME]", adressPorint);

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                if (key.Name == "[NAME]") continue;

                DefaultRegisterQuery query = this.BuildQuery(null, key, "default", line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value ))
                        return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        public bool CompileDek(Compiler compiler, IndexVariabelnDeklaration node, string mode = "default")
        {
            if (mode == "set") return this.SetVariableCompile(compiler, node);

            if (mode == "default") return this.GetVariableCompile(compiler, node, node.Use);

            this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this);
            compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQueryDek(node, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), this.Node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        private bool SetVariableCompile(Compiler compiler, IndexVariabelnDeklaration node)
        {
            if (node.Use == null) return compiler.AddError("Darf nicht null sein");

            if (compiler.ContainerMgmt.CurrentMethod is null) return compiler.AddError("no method found", node.Use);
            if (!compiler.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(node.Name)) return compiler.AddError("variable not in varmapper", node.Use);

            this.Algo = compiler.GetAlgo(this.AlgoName, "set");
            if (this.Algo == null) return false;

            SSAVariableMap map = compiler.ContainerMgmt.CurrentMethod.VarMapper[node.Name];
            //if (map.Calls.Count == 0) return true;
            //SSACompileArgument arg = compiler.ContainerMgmt.StackArguments.Pop();

            SSACompileLine lineset = new SSACompileLine(this);
            this.Line = lineset;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQueryDek(node, key, "set", lineset);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), this.Node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            SSACompileArgument? arg = lineset.Arguments.FirstOrDefault();
            if (arg == null) return compiler.AddError("variable is not set", this.Node);
            if (arg.Reference is null) return compiler.AddError("no reference", this.Node);

            compiler.ContainerMgmt.StackArguments.Push(new SSACompileArgument(arg.Reference));
            if (!compiler.ContainerMgmt.CurrentMethod.IsReferenceInVarsContains(arg.Reference))
            {
                if (map.Reference != null) map.AllSets.Add(arg.Reference);
                map.Reference = arg.Reference;

                if ( map.IsNullable )
                {
                    map.Value = SSAVariableMap.LastValue.Unknown;
                    if ( map.Reference.Owner is CompileNumConst ) map.Value = SSAVariableMap.LastValue.Null;
                }
                else map.Value = SSAVariableMap.LastValue.NotNull;

                arg.Reference.Calls.Remove(lineset);

                return true;
            }

            lineset.ReplaceLine = arg.Reference;
            this.IsUsed = false;
            compiler.AssemblerSequence.Add(this);
            compiler.AddSSALine(lineset);
            if (compiler.ContainerMgmt.CurrentContainer is null) return false;

            compiler.ContainerMgmt.CurrentContainer.PhiSetNewVar.Add(lineset);

            if (map.Reference != null) map.AllSets.Add(lineset);
            map.Reference = lineset;

            if ( map.IsNullable )
            {
                map.Value = SSAVariableMap.LastValue.Unknown;
                if ( map.Reference.Owner is CompileNumConst ) map.Value = SSAVariableMap.LastValue.Null;
            }
            else map.Value = SSAVariableMap.LastValue.NotNull;

            return true;
        }

        private DefaultRegisterQuery BuildQueryDek(IndexVariabelnDeklaration node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Uses = node.ThisUses;
            if ("base" == node.Name) query.Uses = node.BaseUsesSet;

            object queryValue = (object)node.Name;
            if ("[REG]" == key.Name) queryValue = 1;

            query.Value = queryValue;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public bool Compile(Compiler compiler, IndexVariabelnReference node, string mode = "default")
        {
            if (this.CheckRelevanz(compiler, node, mode)) return true;

            if (mode == "set" && node.Deklaration is IndexVariabelnDeklaration vd) return this.SetVariableCompile(compiler, vd);
            if (mode == "default" && node.Deklaration is IndexVariabelnDeklaration ivd) return this.GetVariableCompile(compiler, ivd, node.Use);

            this.Node = node.Use;
            compiler.AssemblerSequence.Add(this);

            string printmode = mode;
            if ("vektorcall" == mode || mode == "setvektorcall") printmode = "methode";
            if (node.Deklaration is IndexPropertyGetSetDeklaration) printmode = "methode";
            if ("funcref" == mode) printmode = "point";
            if ("setref" == mode) printmode = "point";

            this.Algo = compiler.GetAlgo(this.AlgoName, printmode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            this.PrimaryKeys = new Dictionary<string, string>();

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), this.Node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return true;
        }

        private bool GetVariableCompile(Compiler compiler, IndexVariabelnDeklaration deklaration, IParseTreeNode use)
        {
            if (compiler.ContainerMgmt.CurrentMethod is null) return compiler.AddError("no method found", deklaration.Use);
            if (!compiler.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(deklaration.Name)) return compiler.AddError("variable not in varmapper", deklaration.Use);

            SSAVariableMap map = compiler.ContainerMgmt.CurrentMethod.VarMapper[deklaration.Name];

            if ( this.IsNullCheck ) map.Value = SSAVariableMap.LastValue.NotNull;
            if ( map.Value == SSAVariableMap.LastValue.NotSet ) return compiler.AddError ( "variable is not set!", use );
            if ( map.Value == SSAVariableMap.LastValue.Null ) return compiler.AddError ( "variable is null", use );
            if ( map.Value == SSAVariableMap.LastValue.Unknown ) return compiler.AddError ( "null checking for variable is missing", use );

            if (map.Reference == null) return compiler.AddError("variable is not set!", deklaration.Use);

            SSACompileArgument arg = new SSACompileArgument(map.Reference);
            compiler.ContainerMgmt.StackArguments.Push(arg);

            return true;
        }

        private bool CheckRelevanz(Compiler compiler, IndexVariabelnReference node, string mode)
        {
            return false;

            /*if (mode != "default" && mode != "set") return false;
            if (!(node.Deklaration is IndexVariabelnDeklaration t)) return false;
            if (mode == "set") return t.References.Count == 0;

            ICompileRoot root = compiler.AssemblerSequence.LastOrDefault();
            if (!(root is CompileReferenceCall u)) return false;
            if (root.Node == null) return false;
            if (root.Node.Token.Text != node.Name) return false;
            if (u.Algo.Mode != "set") return false;

            //if (t.References.Count != 1) return true;
            //u.DoNotCompile = true;

            return false;*/
        }

        public bool InFileCompilen(Compiler compiler)
        {
            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

            if (this.DoNotCompile || !this.IsUsed)
            {
                compiler.toRemove.Add(this);
                return true;
            }

            if (this.Algo is null) return false;

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        public bool CompilePoint0(Compiler compiler, string mode = "point")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);

            this.PrimaryKeys = new Dictionary<string, string>();

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;
            if (this.Algo is null) return false;

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(null, key, "point0", line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
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