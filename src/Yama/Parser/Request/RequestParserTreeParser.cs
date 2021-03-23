using Yama.Lexer;

namespace Yama.Parser.Request
{
    public class RequestParserTreeParser
    {

        #region get/set

        public Parser Parser
        {
            get;
            set;
        }

        public IdentifierToken Token
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public RequestParserTreeParser(Parser parser, IdentifierToken token)
        {
            this.Parser = parser;
            this.Token = token;
        }

        #endregion ctor

    }
}