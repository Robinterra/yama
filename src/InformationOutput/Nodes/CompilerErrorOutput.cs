using Yama.Lexer;
using Yama.Parser;

namespace Yama.InformationOutput.Nodes
{

    public class CompilerErrorOutput : IOutputNode
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

        public CompilerErrorOutput(string msg, IParseTreeNode parserNode)
        {
            this.Message = msg;
            this.ParserNode = parserNode;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            IdentifierToken token = this.ParserNode.Token;

            string filename = token.Info is null ? "unknown" : token.Info.Origin;

            string printMessage = $"{filename}({token.Line},{token.Column}): Compiler Error - {Message} '{token.Text}'";

            o.Error.Write(printMessage, newLine: true, foreColor: ConsoleColor.Red);

            return true;
        }

        #endregion methods

    }

}