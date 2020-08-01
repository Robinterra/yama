using System.Collections.Generic;

namespace Yama.Compiler
{
    public class CompileAssemblyLanguage
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public List<CompileAlgo> CompileAlgos
        {
            get;
            set;
        }
    }
}