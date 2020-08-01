using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileExecuteCall : ICompile<ReferenceCall>
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

        public bool Compile(Compiler compiler, ReferenceCall node, string mode = "default")
        {
            CompileAlgo algo = compiler.GetAlgo(this.AlgoName, mode);

            for (int i = 0; i < algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(algo.AssemblyCommands[i], null);
            }

            return true;
        }
    }

}