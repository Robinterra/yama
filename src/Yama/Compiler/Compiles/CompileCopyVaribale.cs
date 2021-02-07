using System.Collections.Generic;
using Yama.Compiler.Definition;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileFreeLoop : ICompile<IParseTreeNode>
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "none";

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

        public bool IsUsed
        {
            get
            {
                return true;
            }
        }

        public int Position
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public bool Compile(Compiler compiler, IParseTreeNode node, string mode = "default")
        {
            this.Node = node;

            SSACompileLine line = new SSACompileLine(this);
            line.LoopContainer = compiler.ContainerMgmt.CurrentContainer;
            compiler.AddSSALine(line);

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {

            return true;
        }

        #endregion methods

    }

}