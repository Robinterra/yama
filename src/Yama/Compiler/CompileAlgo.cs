using System.Collections.Generic;

namespace Yama.Compiler
{
    public class CompileAlgo
    {

        #region get/set

        public string? Name
        {
            get;
            set;
        }

        public string? Mode
        {
            get;
            set;
        }

        public string? Description
        {
            get;
            set;
        }

        public bool CanBeDominatet
        {
            get;
            set;
        }

        public bool CanBeCompute
        {
            get;
            set;
        }

        public List<AlgoKeyCall> Keys
        {
            get;
            set;
        }

        public List<AlgoKeyCall> PostKeys
        {
            get;
            set;
        }

        public List<string> AssemblyCommands
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CompileAlgo()
        {
            this.Keys = new List<AlgoKeyCall>();
            this.AssemblyCommands = new List<string>();
            this.PostKeys = new List<AlgoKeyCall>();
        }

        #endregion ctor

    }
}