using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Parser;

namespace Yama.Index
{
    public class IndexError
    {

        #region get/set

        public IOutputNode Output
        {
            get;
        }

        #endregion get/set

        #region ctor

        public IndexError(IParseTreeNode use, string msg)
        {
            this.Output = new IndexErrorOutput(msg, use);
        }

        public IndexError(IOutputNode outputNode)
        {
            this.Output = outputNode;
        }

        #endregion ctor

    }
}