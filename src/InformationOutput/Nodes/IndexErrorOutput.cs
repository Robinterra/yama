using Yama.Lexer;
using Yama.Parser;

namespace Yama.InformationOutput.Nodes
{

    public class IndexErrorOutput : IOutputNode
    {

        #region get/set

        public IParseTreeNode ParserNode
        {
            get;
        }

        public string Message
        {
            get;
        }

        #endregion get/set

        #region ctor

        public IndexErrorOutput(string msg, IParseTreeNode parserNode)
        {
            this.Message = msg;
            this.ParserNode = parserNode;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            IdentifierToken token = this.ParserNode.Token;

            string filename = token.FileInfo is null ? "unknown" : token.FileInfo.FullName;

            string printMessage = $"{filename}({token.Line},{token.Column}): Index Error - {Message} '{token.Text}'";

            o.Error.Write(printMessage, newLine: true, foreColor: ConsoleColor.Red);

            return true;
        }

        #endregion methods

    }

}