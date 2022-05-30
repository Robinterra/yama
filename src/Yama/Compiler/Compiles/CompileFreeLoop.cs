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

        public IParseTreeNode? Node
        {
            get;
            set;
        }

        public CompileAlgo? Algo
        {
            get;
            set;
        } = new CompileAlgo() { Name = "LoopEnde" };

        public Dictionary<string, string> PrimaryKeys
        {
            get;
            set;
        } = new();

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

        public SSACompileLine Begin
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CompileFreeLoop(SSACompileLine begin)
        {
            this.Begin = begin;
        }

        #endregion ctor

        #region methods

        public bool Compile(Compiler compiler, IParseTreeNode node, string mode = "default")
        {
            this.Node = node;

            SSACompileLine line = new SSACompileLine(this, true);
            line.FlowTask = ProgramFlowTask.IsLoopEnde;
            line.LoopContainer = compiler.ContainerMgmt.CurrentContainer;
            if (line.LoopContainer is null) return compiler.AddError("ende des loop konnte nicht gefunden werden", node);

            line.LoopContainer.LoopLine = line;
            this.Line = line;
            line.AddArgument(new SSACompileArgument(this.Begin));
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