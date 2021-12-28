using Yama.Parser;

namespace Yama.Compiler
{
    public class CompilerError
    {

        #region get/set

        public IParseTreeNode? Use
        {
            get;
            set;
        }

        public string Msg
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public CompilerError(string msg, IParseTreeNode? use)
        {
            this.Use = use;
            this.Msg = msg;
        }

        #endregion ctor
    }
}