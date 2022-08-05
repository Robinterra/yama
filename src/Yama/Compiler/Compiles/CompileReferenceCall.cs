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

        public SSAVariableMap? VariableMap{
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
            this.VariableMap = map;
            if (map.MutableState == SSAVariableMap.VariableMutableState.NotMutable && map.Value != SSAVariableMap.LastValue.NotSet)
                return compiler.AddError($"variable '{map.Key}' is not mutable", node.Use);

            SSACompileLine lineset = new SSACompileLine(this);
            this.Line = lineset;
            lineset.LastSet = map.Reference;

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
                    if (!this.SetVariableByReferenceCheck(compiler, arg, map, node)) return false;
                }
                else map.Value = SSAVariableMap.LastValue.NotNull;

                arg.Reference.Calls.Remove(lineset);
                arg.Reference.LastSet = lineset.LastSet;

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
                if (!this.SetVariableByReferenceCheck(compiler, arg, map, node)) return false;
            }
            else map.Value = SSAVariableMap.LastValue.NotNull;

            return true;
        }

        private bool SetVariableByReferenceCheck(Compiler compiler, SSACompileArgument arg, SSAVariableMap map, IndexVariabelnDeklaration node)
        {
            if (map.Reference is null) return false;

            map.Value = SSAVariableMap.LastValue.Unknown;
            if ( map.Reference.Owner is CompileNumConst ) map.Value = SSAVariableMap.LastValue.Null;
            if (map.Reference.Owner.Node is NewKey && map.Kind == SSAVariableMap.VariableType.BorrowingReference) return compiler.AddError("A new instance can not set to a borrowing variable", map.Reference.Owner.Node);
            //if (this.IsNullCheck) map.Value = SSAVariableMap.LastValue.NotNull;

            if (arg.IndexRef is IndexPropertyDeklaration ipd)
            {
                if (ipd.Use.BorrowingToken is not null)
                {
                    if (map.Kind == SSAVariableMap.VariableType.BorrowingReference) return true;
                    return compiler.AddError("a global borrwoing variable can not set to a owner variable", ipd.Use);
                }
                if (map.Kind != SSAVariableMap.VariableType.OwnerReference) return true;
                if (arg.Reference is null) return compiler.AddError("CompileReferenceCall.cs: darf nicht null sein");
                if (arg.Reference.Owner is null) return compiler.AddError("CompileReferenceCall.cs: darf nicht null sein");
                if (arg.Reference.Owner.Node is not ReferenceCall rc)return compiler.AddError("CompileReferenceCall.cs: muss ein ReferenceCall sein");

                new CompileNumConst().Compile(compiler, new Number { Token = new Lexer.IdentifierToken() { Value = 0 } });
                compiler.ContainerMgmt.StackArguments.Push(arg.Reference.Arguments.First());
                new CompileReferenceCall().Compile(compiler, rc, "setpoint");
            }
            if (arg.Variable is null) return true;

            map.Value = arg.Variable.Value;
            if (arg.Variable.Kind == SSAVariableMap.VariableType.OwnerReference && map.Kind == SSAVariableMap.VariableType.OwnerReference)
            {
                arg.Variable.OrgMap.Kind = SSAVariableMap.VariableType.BorrowingReference;
                arg.Variable.OrgMap.Value = SSAVariableMap.LastValue.NeverCall;
                arg.Variable.OrgMap.MutableState = SSAVariableMap.VariableMutableState.NotMutable;
            }
            if (arg.Variable.Kind == SSAVariableMap.VariableType.BorrowingReference && map.Kind == SSAVariableMap.VariableType.OwnerReference)
            {
                return compiler.AddError("You can not set a Borrowing variable to a owner variable", node.Use);
            }

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

            if (!this.CheckOwnershipForPorperty(node, compiler, line, mode)) return false;

            return true;
        }

        private bool CheckOwnershipForPorperty(IndexVariabelnReference node, Compiler compiler, SSACompileLine line, string mode)
        {
            if (node.Deklaration is not IndexPropertyDeklaration ipd) return true;
            if (mode == "point")
            {
                SSACompileArgument getArg = compiler.ContainerMgmt.StackArguments.Peek();
                getArg.IndexRef = ipd;
            }
            if (mode != "setpoint") return true;
            if (ipd.Type.Deklaration is IndexKlassenDeklaration dk)
            {
                if (dk.MemberModifier != ClassMemberModifiers.None) return true;
            }

            SSACompileArgument? arg = line.Arguments.LastOrDefault();
            if (arg is null) return true;
            if (arg.Reference is null) return true;
            if (arg.Reference.Owner.Node is NewKey && ipd.Use.BorrowingToken is not null && arg.Variable is null) return compiler.AddError("A new instance can not set to a borrowing global variable", arg.Reference.Owner.Node);

            if (arg.Variable is null) return true;

            if (arg.Variable.Kind == SSAVariableMap.VariableType.OwnerReference && ipd.Use.BorrowingToken is null)
            {
                arg.Variable.OrgMap.Kind = SSAVariableMap.VariableType.BorrowingReference;
                arg.Variable.OrgMap.Value = SSAVariableMap.LastValue.NeverCall;
                arg.Variable.OrgMap.MutableState = SSAVariableMap.VariableMutableState.NotMutable;
            }
            if (arg.Variable.Kind == SSAVariableMap.VariableType.BorrowingReference && ipd.Use.BorrowingToken is null)
            {
                return compiler.AddError("You can not set a Borrowing variable to a owner variable", node.Use);
            }

            return true;
        }

        private bool GetVariableCompile(Compiler compiler, IndexVariabelnDeklaration deklaration, IParseTreeNode use)
        {
            if (compiler.ContainerMgmt.CurrentMethod is null) return compiler.AddError("no method found", deklaration.Use);
            if (!compiler.ContainerMgmt.CurrentMethod.VarMapper.ContainsKey(deklaration.Name)) return compiler.AddError("variable not in varmapper", deklaration.Use);

            SSAVariableMap map = compiler.ContainerMgmt.CurrentMethod.VarMapper[deklaration.Name];
            this.VariableMap = map;

            SSAVariableMap.LastValue lastValue = map.Value;

            if ( this.IsNullCheck ) lastValue = SSAVariableMap.LastValue.NotNull;
            if ( lastValue == SSAVariableMap.LastValue.NotSet ) return compiler.AddError ( "variable is not set!", use );
            if ( lastValue == SSAVariableMap.LastValue.NeverCall ) return compiler.AddError ( "variable is unaviable!", use );
            if ( lastValue == SSAVariableMap.LastValue.Null ) return compiler.AddError ( "variable is null", use );
            if ( lastValue == SSAVariableMap.LastValue.Unknown  ) return compiler.AddError ( "null checking for variable is missing", use );

            if (map.Reference == null) return compiler.AddError("variable is not set!", deklaration.Use);

            SSACompileArgument arg = new SSACompileArgument(map.Reference, map);
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