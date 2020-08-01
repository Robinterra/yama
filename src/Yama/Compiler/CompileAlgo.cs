using System.Collections.Generic;

namespace Yama.Compiler
{
    public class CompileAlgo
    {
        public string Name
        {
            get;
            set;
        }

        public string Mode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public List<string> Keys
        {
            get;
            set;
        }

        public List<string> AssemblyCommands
        {
            get;
            set;
        }

        public CompileAlgo()
        {
            this.Keys = new List<string>();
            this.AssemblyCommands = new List<string>();
        }
    }
}