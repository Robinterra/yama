using Yama.Lexer;
using Yama.Parser;

namespace Yama.InformationOutput.Nodes
{

    public class IndexCritError : IOutputNode
    {

        #region get/set

        public string Message
        {
            get;
        }

        #endregion get/set

        #region ctor

        public IndexCritError(string msg)
        {
            this.Message = msg;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string printMessage = $"Index Error: {Message}";

            o.Error.Write(printMessage, newLine: true, foreColor: ConsoleColor.Red);

            return true;
        }

        #endregion methods

    }

}