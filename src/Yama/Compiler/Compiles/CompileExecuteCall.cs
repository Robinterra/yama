using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileExecuteCall : ICompile<FunktionsDeklaration>
    {
        public string AlgoName
        {
            get;
            set;
        } = "ExecuteCall";

        public int Counter
        {
            get;
            set;
        } = 2;

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public bool Compile(Compiler compiler, FunktionsDeklaration node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);

            if (this.Algo == null) return false;

            return compiler.Definition.ParaClean();
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], null);
            }

            return true;
        }
    }

}