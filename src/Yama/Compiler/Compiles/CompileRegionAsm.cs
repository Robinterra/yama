using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileRegionAsm : ICompile<ConditionalCompilationNode>
    {

        #region get/set

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

        public string AlgoName
        {
            get;
            set;
        } = "RegionAsm";

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public bool IsUsed
        {
            get
            {
                return false;
            }
        }

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        }

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public SSACompileLine Line
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public bool Compile(Compiler compiler, ConditionalCompilationNode node, string mode = "default")
        {
            this.Node = node;

            compiler.AssemblerSequence.Add(this);

            this.Algo = new CompileAlgo();

            this.Algo.AssemblyCommands.Add(node.Token.Value.ToString());

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            foreach (string str in this.AssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str, false));
            }

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(new RequestAddLine(this, this.Algo.AssemblyCommands[i]));
            }

            foreach (string str in this.PostAssemblyCommands)
            {
                compiler.AddLine(new RequestAddLine(this, str));
            }

            return true;
        }

        #endregion methods

    }

}