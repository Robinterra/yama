using System;
using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileExecuteCall : ICompile<MethodeDeclarationNode>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "ExecuteCall";

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new Dictionary<string, string>();

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

        public bool IsUsed
        {
            get
            {
                return true;
            }
        }

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

        #region ctor

        public CompileExecuteCall()
        {

        }

        #endregion ctor

        #region methods

        private DefaultRegisterQuery BuildQuery(MethodeDeclarationNode? node, AlgoKeyCall key, string mode, SSACompileLine line)
        {
            DefaultRegisterQuery query = new DefaultRegisterQuery();
            query.Key = key;
            query.Kategorie = mode;
            query.Value = node;

            if (key.Name == "[SSAPOP]" || key.Name == "[SSAPUSH]") query.Value = new RequestSSAArgument(line);

            return query;
        }

        public bool Compile(Compiler compiler, MethodeDeclarationNode? node, string mode = "default")
        {
            this.Node = node;

            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            SSACompileLine line = new SSACompileLine(this, true);
            compiler.AddSSALine(line);
            this.Line = line;

            foreach (AlgoKeyCall key in this.Algo.Keys)
            {
                DefaultRegisterQuery query = this.BuildQuery(node, key, mode, line);

                Dictionary<string, string>? result = compiler.Definition.KeyMapping(query);
                if (result == null) return compiler.AddError(string.Format ("Es konnten keine daten zum Keyword geladen werden {0}", key.Name ), node);

                foreach (KeyValuePair<string, string> pair in result)
                {
                    if (!this.PrimaryKeys.TryAdd ( pair.Key, pair.Value )) return compiler.AddError(string.Format ("Es wurde bereits ein Keyword hinzugefügt {0}", key.Name), null);
                }
            }

            return compiler.Definition.ParaClean();
        }

        public bool InFileCompilen(Compiler compiler)
        {
            this.ResultCompile(compiler);

            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }
            if (this.Algo is null) return false;

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i], this.PrimaryKeys));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, this.PrimaryKeys));
            }

            return true;
        }

        private bool ResultCompile(Compiler compiler)
        {
            if (!this.PrimaryKeys.ContainsKey("[SSAPUSH]")) return true;

            string register = this.PrimaryKeys["[SSAPUSH]"];
            if (!(compiler.Definition is GenericDefinition t)) return true;
            if (register == t.GetRegister(t.ResultRegister)) return true;

            CompileAlgo? algo = compiler.GetAlgo(this.AlgoName, "result");
            if (algo == null) return false;

            this.PostAssemblyCommands.AddRange(algo.AssemblyCommands);

            return true;
        }

        #endregion methods

    }

}