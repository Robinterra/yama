using Yama.InformationOutput;
using Yama.InformationOutput.Nodes;
using Yama.Parser;

namespace Yama.Compiler
{
    public class CompilerError
    {

        #region get/set

        public IOutputNode Output
        {
            get;
        }

        #endregion get/set

        #region ctor

        public CompilerError(IParseTreeNode use, string msg)
        {
            this.Output = new CompilerErrorOutput(msg, use);
        }

        public CompilerError(IOutputNode outputNode)
        {
            this.Output = outputNode;
        }

        #endregion ctor

    }
}