using System.Collections.Generic;
using Yama.Index;
using Yama.Parser;

namespace Yama.Compiler
{

    public class CompileSprungPunkt : ICompileRoot
    {
        public string AlgoName
        {
            get;
            set;
        } = "SprungPunkt";

        public CompileAlgo Algo
        {
            get;
            set;
        }

        public List<ICompileRoot> Calls
        {
            get;
            set;
        } = new List<ICompileRoot>();
        public string JumpPointName { get; set; }

        public bool Add(Compiler compiler, ICompileRoot call)
        {
            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();

            this.Calls.Add(call);

            return true;
        }

        public bool Compile(Compiler compiler, IParseTreeNode parent, string mode = "default")
        {
            compiler.AssemblerSequence.Add(this);

            this.Algo = compiler.GetAlgo(this.AlgoName, mode);
            if (this.Algo == null) return false;

            if (string.IsNullOrEmpty(this.JumpPointName)) this.JumpPointName = compiler.Definition.GenerateJumpPointName();

            return true;
        }

        public bool InFileCompilen(Compiler compiler)
        {
            if (this.Calls.Count == 0) return true;

            for (int i = 0; i < this.Algo.AssemblyCommands.Count; i++)
            {
                compiler.AddLine(this.Algo.AssemblyCommands[i], null, new Dictionary<string, string> { { "[NAME]", this.JumpPointName } });
            }

            return true;
        }
    }

}