using Yama.Parser;

namespace Yama.Index
{
    public class IndexError
    {

        #region get/set

        public IParseTreeNode? Use
        {
            get;
        }

        public string Msg
        {
            get;
        }

        #endregion get/set

        #region ctor

        public IndexError(IParseTreeNode? use, string msg)
        {
            this.Use = use;
            this.Msg = msg;
        }

        #endregion ctor
    }
}