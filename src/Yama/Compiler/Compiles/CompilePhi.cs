using Yama.Parser;

namespace Yama.Compiler
{

    public class CompilePhi : ICompileRoot
    {

        #region get/set

        public string AlgoName
        {
            get;
            set;
        } = "Phi";

        public CompileAlgo? Algo
        {
            get;
            set;
        }

        public List<string> AssemblyCommands
        {
            get;
            set;
        } = new List<string>();

        public List<string> PostAssemblyCommands
        {
            get;
            set;
        } = new List<string>();

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

        public SSACompileLine? Line
        {
            get;
            set;
        }
        public IParseTreeNode? Node
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        public bool InFileCompilen(Compiler compiler)
        {
            return true;
        }

        #endregion methods

    }

}