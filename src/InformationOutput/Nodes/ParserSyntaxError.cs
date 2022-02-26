using Yama.Lexer;

namespace Yama.InformationOutput.Nodes
{

    public class ParserSyntaxError : IOutputNode
    {

        #region get/set

        public IdentifierToken Token
        {
            get;
        }

        public string Message
        {
            get;
        }

        #endregion get/set

        #region ctor

        public ParserSyntaxError(string msg, IdentifierToken token)
        {
            this.Message = msg;
            this.Token = token;
        }

        #endregion ctor

        #region methods

        public bool Print(RequestOutput o)
        {
            string filename = this.Token.Info is null ? "unknown" : this.Token.Info.Origin;

            string printMessage = $"{filename}({Token.Line},{Token.Column}): Syntax Error - {Message} '{Token.Text}'";

            o.Error.Write(printMessage, newLine: true, foreColor: ConsoleColor.Red);

            return true;
        }

        #endregion methods

    }

}