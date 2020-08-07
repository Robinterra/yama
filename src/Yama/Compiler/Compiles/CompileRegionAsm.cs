using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileRegionAsm : ICompile<BedingtesCompilierenParser>
    {

        #region get/set

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

        #endregion get/set

        #region methods

        public bool Compile(Compiler compiler, BedingtesCompilierenParser node, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = new CompileAlgo();

            this.Algo.AssemblyCommands.Add(node.Token.Value.ToString());

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], null);
            }

            return true;
        }

        #endregion methods

    }

}